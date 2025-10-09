using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public static class HelperMethods
{
    //获取在指针检查位置的t泛型组件
    public static  bool GetComponentsAtCursorLocation<T>(out List<T> componentsAtPositionList, Vector3 positionToCheck)
    {
        bool found = false;
        
        List<T> componentList = new List<T>();
        
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(positionToCheck);
        
        T tComponent = default(T);

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            tComponent = collider2DArray[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentList.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArray[i].gameObject.GetComponentInChildren<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                }
            }
        }
        
        componentsAtPositionList = componentList;
        
        return found;
    }
    
    public static bool GetcomponentsAtBoxLoaction<T>(out List<T> listComponentsAtBoxPosition, Vector2 point, Vector2 size, float angle)
    {
        bool found = false;
        List<T> componentsAtBoxPosition = new List<T>();
        
        Collider2D[] collider2DArrays = Physics2D.OverlapBoxAll(point, size, angle);
        
        //遍历colliders获取T类型的gameobject
        for (int i = 0; i < collider2DArrays.Length; i++)
        {
            T tComponent = collider2DArrays[i].gameObject.GetComponentInParent<T>();
            if (tComponent != null)
            {
                found = true;
                componentsAtBoxPosition.Add(tComponent);
            }
            else
            {
                tComponent = collider2DArrays[i].gameObject.GetComponentInChildren<T>();
                if(tComponent != null){
                    found = true;
                    componentsAtBoxPosition.Add(tComponent);
                }
            }
        }

        listComponentsAtBoxPosition = componentsAtBoxPosition;
        
        return found;
    }
    
    //
    public static T[] GetComponentsAtBoxLocationNonAlloc<T>(int numberOfColliderToTest, Vector2 point, Vector2 size,
        float angle)
    {
        Collider2D[] collider2DArray = new Collider2D[numberOfColliderToTest];
        
        Physics2D.OverlapBoxNonAlloc(point, size, angle, collider2DArray);
        
        T tComponent = default(T);
        
        T[] componentArray = new T[collider2DArray.Length];

        for (int i = collider2DArray.Length - 1; i >= 0 ; i--)
        {  
            if (collider2DArray[i] != null)
            {
                tComponent = collider2DArray[i].gameObject.GetComponent<T>();

                if (tComponent != null)
                {
                    componentArray[i] = tComponent;
                }
            }
        }
        return componentArray;
    }
}
