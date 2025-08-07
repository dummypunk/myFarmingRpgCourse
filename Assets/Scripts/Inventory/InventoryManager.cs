using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonobehaviour<InventoryManager>
{
    private Dictionary<int, ItemDetails> itemDetalsDictionary;

    [SerializeField] private SO_ItemList itemList = null;

    public List<InventoryItem>[] inventoryLists;//根据数组的0,1顺序区分player和chest的枚举

    [HideInInspector] public int[] inventoryListCapacityIntArray;//该列表的index表示Inventory list（根据inventoryLocation的枚举值），数值则为inventoryList的容量

    protected override void Awake()
    {
        base.Awake();

        CreateItemDetailsDictionary();

        CreateInventoryLists();
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
    /// 从可脚本化实例Item列表获取itemdetails补充字典
    /// </summary>
    private void CreateItemDetailsDictionary()  
    {
        itemDetalsDictionary = new Dictionary<int, ItemDetails>();
        foreach(ItemDetails itemDetals in itemList.itemDetals)
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

    //private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
    //{
    //    foreach (InventoryItem inventoryItem in inventoryList)
    //    {
    //        Debug.Log("Item Description:" + InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription + "   ItemQuantity:" + inventoryItem.itemQuantity);
    //    }
    //    Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    //}
}
