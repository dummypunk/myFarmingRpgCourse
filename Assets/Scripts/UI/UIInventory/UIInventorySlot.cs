using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Camera mainCamera;
    private Canvas parentCanvas;
    private Transform parentItem;
    private GameObject draggedItem;

    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [SerializeField] private UIInventoryBar inventoryBar = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemDetails itemDetails;
    [SerializeField] private GameObject itemPrefab = null;
    [HideInInspector] public int itemQuantity;
    [SerializeField] private int slotNumber = 0;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }



    /// <summary>
    /// 设置该Inventory slot Item 被选中
    /// </summary>
    private void SetSelectedItem()
    {
        //清除当前已经选中物品高亮状态
        inventoryBar.ClearHighLightOnInventorySlots();

        //高亮选中物品
        isSelected = true;

        //设置高亮Inventory slot
        inventoryBar.SetHighlightedInventorySlots();

        //在Inventory中设置被选中物品
        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.player, itemDetails.itemCode);

        if(itemDetails.canBeCarried == true)
        {
            //show player carrying Item
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }

    private void ClearSelectedItem()
    {
        //清楚当前高亮物品
        inventoryBar.ClearHighLightOnInventorySlots();

        isSelected = false;

        //设置没有物品在Inventory中被选中
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.player);

        Player.Instance.ClearCarriedItem();
    }

    /// <summary>
    /// 丢弃inventorybar中可拖拽物品到鼠标位置，被dropitemEvent引用
    /// </summary>
    private void DropSelectedItemAtMousePosition()
    {
        if(itemDetails != null && isSelected)
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
            
            //根据tilemap的gridProperty判断物品是否能放置在这里
            Vector3Int gridPosition = GridPropertiesManager.Instance.grid.WorldToCell(worldPosition);
            GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(gridPosition.x, gridPosition.y);

            if (gridPropertyDetails != null && gridPropertyDetails.canDropItems)
            {
                //在鼠标位置生成prefab物品
                GameObject itemGameObject = Instantiate(itemPrefab,
                    new Vector3(worldPosition.x, worldPosition.y - Settings.gridCellSize / 2, worldPosition.z),
                    Quaternion.identity, parentItem);
                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;
            
                //Remove item from inventory
                InventoryManager.Instance.RemoveItem(InventoryLocation.player, item.ItemCode);

                //如果当前位置没有物品，取消高亮设置
                if(InventoryManager.Instance.FindItemInInventory(InventoryLocation.player,item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }
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

            SetSelectedItem();
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

                DestoryInventoryTextBox();

                ClearSelectedItem();
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemQuantity != 0)
        {
            //实例化inventory textbox
            inventoryBar.inventoryTextBoxGameobject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameobject.transform.SetParent(parentCanvas.transform, false);

            UIInventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameobject.GetComponent<UIInventoryTextBox>();

            //获取itemtype描述
            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            //填充textbox
            inventoryTextBox.SetTextBoxText(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            if (inventoryBar.IsInventoryBarPositionBottom)
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y + 50f, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameobject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                inventoryBar.inventoryTextBoxGameobject.transform.position = new Vector3(transform.position.x, transform.position.y - 50f, transform.position.z);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //左键点击
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if(isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if(itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestoryInventoryTextBox();
    }

    public void DestoryInventoryTextBox()
    {
        if(inventoryBar.inventoryTextBoxGameobject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameobject);
        }
    }

    public void SceneLoaded()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemParentTransform).transform;
    }
}