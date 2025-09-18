using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GenerateGUID))]
public class SceneItemManager : SingletonMonobehaviour<SceneItemManager>,ISaveable
{
    private Transform parentItem;
    [SerializeField] private GameObject itemPrefab = null;
    
    private string _iSaveableUniqueID; 
    
    public string ISaveableUniqueID { get { return _iSaveableUniqueID;} set { _iSaveableUniqueID = value; } }
    
    private GameObjectSave _gameObjectSave;
    
    public GameObjectSave GameObjectSave { get { return _gameObjectSave;} set { _gameObjectSave = value; } }

    private void AfterSceneLoad()
    {
        parentItem = GameObject.FindGameObjectWithTag(Tags.ItemParentTransform).transform;
    }

    
    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }


    public void ISaveableStoreScene(string sceneName)
    {
        //如果当前物品存在，移除上个场景中的old scene save，（通过GameObjectSave类中的字典移除旧场景的sceneSave）
        GameObjectSave.sceneDataDict.Remove(sceneName);
          
        //获取场景中所有的item
        List<SceneItem> sceneItemsList = new List<SceneItem>();
        Item[] itemsInScene = FindObjectsOfType<Item>();

        foreach (Item item in itemsInScene)
        {
            SceneItem sceneItem = new SceneItem();
            sceneItem.itemCode = item.ItemCode;
            sceneItem.position = new Vector3Serializable(item.transform.position.x, item.transform.position.y, item.transform.position.z);
            sceneItem.itemName = item.name;
            
            //添加sceneItem to List
            sceneItemsList.Add(sceneItem); 
        }
        
        //在sceneSave中创建sceneItem列表
        SceneSave sceneSave = new SceneSave();
        sceneSave.listSceneItem = sceneItemsList;
        
        //添加sceneSave至GameObject
        GameObjectSave.sceneDataDict.Add(sceneName, sceneSave);
    } 

    public void ISaveableRestoreScene(string sceneName)
    {
        if (GameObjectSave.sceneDataDict.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if(sceneSave.listSceneItem != null)
            {
                //如果发现sceneItemLists，销毁当前存在于scene中的items
                DestroySceneItems();
                
                //随后实例化新的scene items的list
                InstantiateScenItems(sceneSave.listSceneItem);
            }
        }
    }

    private void DestroySceneItems()
    {
        //获取场景中所有item
        Item[] itemsInScene = FindObjectsOfType<Item>();
        
        //遍历scene中所有物品并销毁物品
        for (int i = itemsInScene.Length - 1; i >-1; i--)
        {
            Destroy(itemsInScene[i].gameObject);
        }
    }

    private void InstantiateScenItems(List<SceneItem> sceneItemList)
    {
        GameObject itemGameObject;

        foreach (SceneItem sceneItem in sceneItemList)
        {
            itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y, sceneItem.position.z), Quaternion.identity, parentItem);

            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = sceneItem.itemCode;
            item.name = sceneItem.itemName;
        }
    }
    
    private void OnDisable()
    {
        ISaveableDeregister();
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }


    private void OnEnable()
    {
        ISaveableRegister();
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }
    
    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectsList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectsList.Remove(this); 
    }

}
