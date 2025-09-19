using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UIInventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite blank16x16Sprite = null;
    [FormerlySerializedAs("inventorySlot")] [SerializeField] private UIInventorySlot[] UIInventorySlot = null;
    public GameObject inventoryBarDraggedItem;
    [HideInInspector] public GameObject inventoryTextBoxGameobject;


    private RectTransform rectTransform;

    private bool _isInventoryBarPositionBottom = true;

    public bool IsInventoryBarPositionBottom { get => _isInventoryBarPositionBottom; set => _isInventoryBarPositionBottom = value; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    /// <summary>
    /// 清除Inventory bar中所有高亮物品
    /// </summary>
    public void ClearHighLightOnInventorySlots()
    {
        if (UIInventorySlot.Length > 0)
        {
            for (int i = 0; i < UIInventorySlot.Length; i++)
            {
                if (UIInventorySlot[i].isSelected)
                {
                    UIInventorySlot[i].isSelected = false;
                    UIInventorySlot[i].inventorySlotHighlight.color = new Color(0, 0, 0,0);
                    //更新Inventory显示没有物品被选中
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);
                }
            }
        }
    }

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += InventoryUpdated;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= InventoryUpdated;
    }

    private void ClearInventorySlots()
    {
        if (UIInventorySlot.Length > 0)
        {
            //遍历Inventory的所有slots然后利用空白sprite进行更新
            for (int i = 0; i < UIInventorySlot.Length; i++)
            {
                UIInventorySlot[i].inventorySlotImage.sprite = blank16x16Sprite;
                UIInventorySlot[i].textMeshProUGUI.text = "";
                UIInventorySlot[i].itemDetails = null;
                UIInventorySlot[i].itemQuantity = 0;
                SetHighlightedInventorySlots(i);
            }
        }
    }



    private void InventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.player)
        {
            ClearInventorySlots();

            if (UIInventorySlot.Length > 0 && inventoryList.Count > 0)
            {
                //遍历所有Inventory格子并用相应的InventoryList进行更新列表中的物品
                for (int i = 0; i < UIInventorySlot.Length; i++)
                {
                    if (i < inventoryList.Count)
                    {
                        int itemCode = inventoryList[i].itemCode;

                        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                        UIInventorySlot[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                        UIInventorySlot[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                        UIInventorySlot[i].itemDetails = itemDetails;
                        UIInventorySlot[i].itemQuantity = inventoryList[i].itemQuantity;
                        SetHighlightedInventorySlots(i);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 遍历所有slot，根据条件设置高亮显示
    /// </summary>
    public void SetHighlightedInventorySlots()
    {
        if (UIInventorySlot.Length > 0)
        {
            for (int i = 0; i < UIInventorySlot.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    /// <summary>
    /// 重载方法：检查InventorySlot的每个格子是否被选中，如果被选中，则设置高光
    /// </summary>
    /// <param name="itemCode"></param>
    public void SetHighlightedInventorySlots(int itemPosition)
    {
        if(UIInventorySlot.Length > 0 && UIInventorySlot[itemPosition].itemDetails != null)
        {
            if (UIInventorySlot[itemPosition].isSelected)
            {
                UIInventorySlot[itemPosition].inventorySlotHighlight.color = new Color(1f, 1f, 1f, 1f);
                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, UIInventorySlot[itemPosition].itemDetails.itemCode);
            }
        }
    }


    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewPortPosition = Player.Instance.GetPlayerViewportPosition();

        if(playerViewPortPosition.y > 0.3f && IsInventoryBarPositionBottom == false)
        {
            rectTransform.pivot = new Vector2(0.5f, 0f);
            rectTransform.anchorMin = new Vector2(0.5f,0f);
            rectTransform.anchorMax = new Vector2(0.5f, 0f);
            rectTransform.anchoredPosition = new Vector2(0f, 2.5f);

            IsInventoryBarPositionBottom = true;
        }
        else if(playerViewPortPosition.y <= 0.3f && IsInventoryBarPositionBottom == true)
        {
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0f, -2.5f);

            IsInventoryBarPositionBottom = false;
        }
    }
}
