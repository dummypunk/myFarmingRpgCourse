using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;
    
    [Tooltip("Should be populated from child game object")]
    [SerializeField] private SpriteRenderer cropHarvestedSpriteRenderer = null;
    
    [HideInInspector]
    public Vector2Int cropGridPosition;

    public void ProcessToolAction(ItemDetails equippedItemDetails, bool isToolRight, bool isToolLeft, bool isToolUp, bool isToolDown)
    {
        //获取gird Property Detailss
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
        
        if (gridPropertyDetails == null)
            return;
        
        //获取种子item details
        ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
        if(seedItemDetails == null)
            return;
        
        //获取Crop Details
        CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
        if(cropDetails == null)
            return;
        
        //如果庄稼动画机存在，获取其动画机
        Animator animator = GetComponentInChildren<Animator>();
        
        //触发动画机工具动画
        if (animator == null)
        {
            if (isToolRight || isToolUp)
            {
                animator.SetTrigger("usetoolright");
            }
            else if (isToolDown || isToolLeft)
            {
                animator.SetTrigger("usetoolleft"); 
            }
        }
        
        //获取庄稼需要的收割次数
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
            return;

        //通过以上if判断，增加收割次数
        harvestActionCount++;

        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(isToolRight, isToolUp, cropDetails, gridPropertyDetails, animator);
        }
    }

    private void HarvestCrop(bool isUsingToolRight,bool isUsingToolUp,CropDetails cropDetails, GridPropertyDetails gridPropertyDetails,Animator animator)
    {
        //判断是否有收获动画
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            //如果有Harvest Sprite则添加至Sprite render
            if (cropDetails.harvestedSprite != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            }
            else
            {
                animator.SetTrigger("harvestleft");
            }
        }
        
        //删除网格上的crop
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daySinceWatered = -1;
        
        //在收获动画完成前隐藏Crop,
        if (cropDetails.hideCropBeforeHarvestedAnimation)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        
        //如果庄稼有收获动画，播放完动画之后再删除Crop GameObject
        if (cropDetails.isHarvestedAnimation && animator != null)
        {
            StartCoroutine(ProcessHarvestActionAfterAnimation(cropDetails,gridPropertyDetails ,animator));
        }
        else
        {
            HarvestAction(cropDetails, gridPropertyDetails);
        }
    }

    private IEnumerator ProcessHarvestActionAfterAnimation(CropDetails cropDetails,
        GridPropertyDetails gridPropertyDetails, Animator animator)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
        {
            yield return null;
        }
        
        HarvestAction(cropDetails, gridPropertyDetails);
    }
    
    private void HarvestAction(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        SpawnHarvestedItems(cropDetails);
        
        Destroy(gameObject);
    }

    private void SpawnHarvestedItems(CropDetails cropDetails)
    {
        for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
        {
            int cropsToProduce;
            
            if(cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i]||
               cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
                cropsToProduce = cropDetails.cropProducedMinQuantity[i];
            else 
                cropsToProduce = UnityEngine.Random.Range(cropDetails.cropProducedMinQuantity[i], cropDetails.cropProducedMaxQuantity[i] + 1);

            for (int j = 0; i < cropsToProduce; i++)
            {
                Vector3 spawnPosition;
                if (cropDetails.spawnCropProducedAtPlayerPosition)
                {
                    InventoryManager.Instance.AddItem(InventoryLocation.player, cropDetails.cropProducedItemCode[i]);
                }
                else
                {
                    //随机生成
                    spawnPosition = new Vector3(transform.position.x + UnityEngine.Random.Range(-1f, 1f),
                        transform.position.y + UnityEngine.Random.Range(-1f, 1f), 0);
                    SceneItemManager.Instance.InstantiateScenItem(cropDetails.cropProducedItemCode[i], spawnPosition);
                }
            }
        }
    }
}
