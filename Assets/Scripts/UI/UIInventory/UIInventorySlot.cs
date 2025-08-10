using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera mainCamera;
    private Transform parentItem;
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Start()
    {
        mainCamera = Camera.main;
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemParentTransform).transform;
    }

    /// <summary>
    /// 丢弃inventorybar中可拖拽物品到鼠标位置，被dropitemEvent引用
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.y));

            //在鼠标位置生成prefab物品
            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;
            
            //Remove item from inventory
            InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        //当玩家拖拽的物品slot中itemdetail不为空时，进行逻辑
        if (itemDetails != null)
        {
            //禁止玩家输入
            Player.Instance.DisablePlayerInput();

            //实例化玩家鼠标拖拽的物品
            draggedItem = Instantiate(inventoryBar.inventoryBarDraggedItem, inventoryBar.transform);

            //获取拖拽物品的图像
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //拖拽时如果拖拽物品不为空，则设置拖拽物品位置为鼠标输入位置
        if(draggedItem != null)
        {
            draggedItem.transform.position = Input.mousePosition; 
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //松开鼠标左键时，销毁拖拽物品
        if(draggedItem != null)
        {
            Destroy(draggedItem);

            //如果拖拽的物品超出inventory bar，直接空过，如果物品放在可放置位置，进行逻辑
            if(eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>() != null)
            {
                //获取鼠标拖动结束时对应的slotNumber
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<UIInventorySlot>().slotNumber;

                //交换inventory List中的inventory items
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.player, slotNumber, toSlotNumber);
            }
            else
            {
                if (itemDetails.canBeDroped)
                {
                    DropSelectedItemAtMousePosition();  
                }
            }

            Player.Instance.EnablePlayerInput();
        }
    }

}