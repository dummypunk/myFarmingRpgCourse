using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : SingletonMonobehaviour<SaveLoadManager>
{
    public List<ISaveable> iSaveableObjectsList;

    protected override void Awake()
    {
        base.Awake();
        
        iSaveableObjectsList = new List<ISaveable>();
    }

    public void StoreCurrentSceneDate()
    {
        //遍历所有实现ISaveable接口的gameObject，然后对每个物品触发store scene data方法
        foreach (ISaveable iSaveableObject in iSaveableObjectsList)
        {
            iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
        }
    }
    
    public void ReStoreCurrentSceneDate()
    {
        //遍历所有实现ISaveable接口的gameObject，然后对每个物品触发REstore scene data方法
        foreach (ISaveable iSaveableObject in iSaveableObjectsList)
        {
            iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
        }
    }
}