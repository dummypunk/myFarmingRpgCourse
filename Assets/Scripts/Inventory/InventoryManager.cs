using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetalsDictionary;

    private int[] selectedInventoryItem; //该数组中的索引为Inventory List，数组值为Item code

    [SerializeField] private SO_ItemList so_itemList = null;

    public List<InventoryItem>[] inventoryLists;//根据数组的0,1顺序区分player和chest的枚举

    [HideInInspector] public int[] inventoryListCapacityIntArray;//该列表的index表示Inventory list（根据inventoryLocation的枚举值），数值则为inventoryList的容量

    protected override void Awake()
    {
        base.Awake();

        CreateItemDetailsDictionary();

        CreateInventoryLists();

        //初始化选中物品的数组
        selectedInventoryItem = new int[(int)InventoryLocation.count];

        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];//创建有两个元素数组的List

        //遍历数组，并为数组中每个元素创建新的Item存储列表
        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();//玩家list会是第零个元素，箱子list会是第二个元素
        }

        //初始化inventoryList容量数组
        inventoryListCapacityIntArray = new int[(int)InventoryLocation.count];

        //初始化player Inventory list容量
        inventoryListCapacityIntArray[(int)InventoryLocation.player] = Settings.playerInitialInventoryCapacity;
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject gameObjectToDelete)
    {
        AddItem(inventoryLocation, item);

        Destroy(gameObjectToDelete);
    }

    /// <summary>
    /// 根据枚举类型InventoryLocation向InventoryList中添加物品
    /// </summary>
    public void AddItem(InventoryLocation inventoryLocation, Item item)
    {
        int itemCode = item.ItemCode;
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //检查inventory中是否含有某物品
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        //根据列表中是否存在某物品，进行添加和叠加
        if (itemPosition != -1)
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        else
            AddItemAtPosition(inventoryList, itemCode);

        //调用事件更新Inventory
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    /// <summary>
    /// 将物品添加进inventory的末尾
    /// </summary>
    /// <param name="inventoryList"></param>
    /// <param name="itemCode"></param>
    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();

        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);

        //DebugPrintInventoryList(inventoryList);
    }

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode,int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity + 1;
        inventoryItem.itemQuantity = quantity;
        inventoryItem.itemCode = itemCode;
        inventoryList[position] = inventoryItem;

        //DebugPrintInventoryList(inventoryList);
    }


    /// <summary>
    /// 根据itemCode在Inventory中寻找某一存在物品，如果物品存在，根据InventoryList返回itemPosition
    /// 若不存在返回-1
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    /// <returns></returns>
    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 利用fromItem参数与toItem参数进行inventory item交换位置
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="fromItem"></param>
    /// <param name="toItem"></param>
    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
    {
        //判断fromIndex参数与toItem参数在inventoryList的边界范围内，以及二者是否相等和大于等于零
        if(fromItem< inventoryLists[(int)inventoryLocation].Count && toItem<inventoryLists[(int)inventoryLocation].Count
            &&fromItem != toItem && fromItem >=0 && toItem >= 0)
        {
            InventoryItem fromInventoryItem = inventoryLists[(int)inventoryLocation][fromItem];
            InventoryItem toInventoryItem = inventoryLists[(int)inventoryLocation][toItem];

            inventoryLists[(int)inventoryLocation][fromItem] = toInventoryItem;
            inventoryLists[(int)inventoryLocation][toItem] = fromInventoryItem;

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]); 
        }
    }

    /// <summary>
    /// clear
    /// </summary>
    /// <param name="inventoryLocation"></param>
    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    /// <summary>
    /// 从可脚本化实例Item列表获取itemdetails补充字典
    /// </summary>
    private void CreateItemDetailsDictionary()  
    {
        itemDetalsDictionary = new Dictionary<int, ItemDetails>();
        foreach(ItemDetails itemDetals in so_itemList.itemDetals)
        {
            itemDetalsDictionary.Add(itemDetals.itemCode, itemDetals);
        }
    }

    /// <summary>
    /// 从SO_itemList中的itemDetails获取itemCode
    /// <summary>
    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;
        if (itemDetalsDictionary.TryGetValue(itemCode, out itemDetails))
            return itemDetails;
        else
            return null;
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.Breaking_tool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.Chopping_tool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.Hoeing_tool:
                itemTypeDescription = Settings.HoeingTool;
                break;
            case ItemType.Reaping_tool:
                itemTypeDescription = Settings.ReapingTool;
                break;
            case ItemType.Watering_tool:
                itemTypeDescription = Settings.WateringTool;
                break;
            case ItemType.Collecting_tool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }
        return itemTypeDescription;
    }

    /// <summary>
    /// 从inventory中移除item，并在地图中相应位置创建item
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        //检查inventory是否包含该物品
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if(itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        //发送inventory更新事件
        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryList);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int position)
    {
        InventoryItem inventoryItem = new InventoryItem();

        int quantity = inventoryList[position].itemQuantity - 1;

        if(quantity > 0)
        {
            inventoryItem.itemQuantity = quantity;
            inventoryItem.itemCode = itemCode;
            inventoryList[position] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(position); 
        }
    }

    /// <summary>
    /// 将选定的Inventory Item设置为inventoryLocation中的itemCode
    /// </summary>
    /// <param name="inventoryLocation"></param>
    /// <param name="itemCode"></param>
    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation,int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem inventoryItem in inventoryList)
    //    {
    //        Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "   ItemQuantity:" + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    //}
}
