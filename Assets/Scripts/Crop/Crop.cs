using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Crop : MonoBehaviour
{
    private int harvestActionCount = 0;
    [HideInInspector]
    public Vector2Int cropGridPosition;

    public void ProcessToolAction(ItemDetails equippedItemDetails)
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
        
        //通过以上if判断，增加收割次数
        harvestActionCount++;
        
        //获取庄稼需要的收割次数
        int requiredHarvestActions = cropDetails.RequiredHarvestActionsForTool(equippedItemDetails.itemCode);
        if (requiredHarvestActions == -1)
            return;

        if (harvestActionCount >= requiredHarvestActions)
        {
            HarvestCrop(cropDetails, gridPropertyDetails);
        }
    }

    private void HarvestCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
    {
        //删除网格上的crop
        gridPropertyDetails.seedItemCode = -1;
        gridPropertyDetails.growthDays = -1;
        gridPropertyDetails.daysSinceLastHarvest = -1;
        gridPropertyDetails.daySinceWatered = -1;
        
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        
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
