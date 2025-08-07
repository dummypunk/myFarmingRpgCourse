using UnityEngine;
using System.Collections.Generic;

public class Item : MonoBehaviour
{
    [ItemCodeDescription]

    [SerializeField] private int _itemCode;

    private SpriteRenderer spriteRenderer;

    public int ItemCode { get { return _itemCode; } set { _itemCode = value; } }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if(ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCodeParam)
    {
        if(itemCodeParam != 0)
        {
            ItemCode = itemCodeParam;
            //从InventoryManager实例中获得物品详细
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

            spriteRenderer.sprite = itemDetails.itemSprite;

            //若该item为可收割的，添加摇曳效果组件
            if (itemDetails.itemType == ItemType.Reapable_sceneary)
            {
                gameObject.AddComponent<ItemNudge>();
            }
        }
    }
}
