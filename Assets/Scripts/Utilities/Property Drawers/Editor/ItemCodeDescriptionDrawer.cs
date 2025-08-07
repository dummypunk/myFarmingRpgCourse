using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//通过 自定义属性提取器和属性封装器来
[CustomPropertyDrawer(typeof(ItemCodeDescriptionAttribute))]
public class ItemCodeDescriptionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //改变返回属性高度（为原来的两倍），用来满足我们提取的ItemDescription所需空间
        return EditorGUI.GetPropertyHeight(property) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        //通过begin和endproperty创造新的属性封装器，用来
        EditorGUI.BeginProperty(position, label, property);
        //提取ItemCode
        var newValue = EditorGUI.IntField(new Rect(position.x, position.y, position.width, position.height / 2), label, property.intValue);
        //提取ItemDescription
        EditorGUI.LabelField(new Rect(position.x, position.y + position.height / 2, position.width, position.height / 2), "Item Description",
            GetItemDescription(property.intValue));

        if (EditorGUI.EndChangeCheck())
        {
            property.intValue = newValue;
        }

        EditorGUI.EndProperty();
    }

    private string GetItemDescription(int itemCode)
    {
        SO_ItemList so_ItemList;

        so_ItemList = AssetDatabase.LoadAssetAtPath("Assets/ScriptableObjectAssets/Item/so_ItemList.asset", typeof(SO_ItemList)) as SO_ItemList;

        List<ItemDetails> itemDetailsList = so_ItemList.itemDetals;

        ItemDetails itemDetails = itemDetailsList.Find(x => x.itemCode == itemCode);

        if (itemDetails != null)
            return itemDetails.itemDescription;
        else
            return "";
    }
}

