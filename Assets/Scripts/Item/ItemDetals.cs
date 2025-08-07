using UnityEngine;

[System.Serializable]
public class ItemDetails
{
    public int itemCode;
    public ItemType itemType;
    public string itemDescription;
    public Sprite itemSprite;
    public string itemLongDescription;
    public short itemUseGridRadius;//物品作用格子半径，例如镐子，斧子等
    public float itemUseRadius;//不基于游戏grid而基于unity单位长度的的物品使用半径，如生长的草等
    public bool IsStartingItem;
    public bool canBePickedUp;
    public bool canBeDroped;
    public bool canBeEated;
    public bool canBeCarried;
}
