using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperMethods
{
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
}
