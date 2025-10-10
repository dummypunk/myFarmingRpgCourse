using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonMonobehaviour<PoolManager>
{
    private Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();
    [SerializeField] private pool[] pools = null;
    [SerializeField] private Transform objectPoolTransform = null;
    
    [System.Serializable]
    public struct pool
    {
        public int poolSize;
        public GameObject prefab;
    }

    private void Start()
    {
        //start方法中创建对象池
        for (int i = 0; i < pools.Length; i++)
        {
            CreatePool(pools[i].prefab, pools[i].poolSize);
        }
    }

    private void CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        string prefabName = prefab.name;

        GameObject parentGameObject = new GameObject(prefabName + "Anchor");
        
        parentGameObject.transform.SetParent(objectPoolTransform);

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());

            for (int i = 0; i < poolSize; i++)
            {
                GameObject newGameObject = Instantiate(prefab, parentGameObject.transform) as GameObject;
                newGameObject.SetActive(false);
                
                poolDictionary[poolKey].Enqueue(newGameObject);
            }
        }
    }

    public GameObject ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            //从队列中获取Object
            GameObject objectToReuse = GetObjectFromPool(poolKey);

            ResetObject(position, rotation, objectToReuse, prefab);
            
            return objectToReuse;
        }
        else 
        {
            Debug.Log("No object pool for" + prefab);
            return null;
        }
    }

    private GameObject GetObjectFromPool(int poolKey)
    {
        GameObject objectToReuse = poolDictionary[poolKey].Dequeue();
        poolDictionary[poolKey].Enqueue(objectToReuse);

        if (objectToReuse.activeSelf == true)
        {
            objectToReuse.SetActive(false);
        }
        
        return objectToReuse;
    }

    private static void ResetObject(Vector3 position, Quaternion rotation, GameObject objectToReuse, GameObject prefab)
    {
        objectToReuse.transform.position = position;
        objectToReuse.transform.rotation = rotation;
        
        objectToReuse.transform.localScale = prefab.transform.localScale;
    }
}