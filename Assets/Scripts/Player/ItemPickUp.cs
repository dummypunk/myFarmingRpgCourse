using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //设置局部变量来判断玩家是否触发带有Item组件的gameObject
        Item item = collision.GetComponent<Item>();

        if (item != null)
        {
            //当局部变量item不为空，获取itemDetails      //inventoryManager在场景中被创建，可以通过实例访问内部方法 其并非静态类，需要通过实例化来进行访问内部方法
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            //check物品是否可以捡起
            if(itemDetails.canBePickedUp == true)
            {
                //将物品添加至Inventory
                InventoryManager.Instance.AddItem(InventoryLocation.player, item, collision.gameObject);
            }
        }
    }
}
