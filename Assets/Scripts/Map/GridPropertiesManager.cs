using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GenerateGUID))]

public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    public Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDict;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray;

    private string iSaveableUniqueID;
    public string ISaveableUniqueID { get { return iSaveableUniqueID;} set{ iSaveableUniqueID = value;} }
    
    private GameObjectSave gameObjectSave;
    
    public GameObjectSave GameObjectSave { get { return gameObjectSave;} set{ gameObjectSave = value;} }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
    }

    private void Start()
    {
        InitializeGridProperties();
    }

    private void InitializeGridProperties()
    {
        //遍历数组中所有的GridProperty
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //创建gridPropertyDetails和坐标字符串为key的字典
            Dictionary<string, GridPropertyDetails> gridPropertyDict = new Dictionary<string, GridPropertyDetails>();

            foreach (GridProperty gridProperty in so_GridProperties.gridPropertyList)
            {
                GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y,gridPropertyDict);
            
                if (gridPropertyDetails == null)
                {
                    gridPropertyDetails = new GridPropertyDetails();
                }

                switch (gridProperty.gridBoolProperty)
                {
                    case GridBoolProperty.diggable:
                        gridPropertyDetails.isDiggable = gridProperty.girdBoolValue;
                        break;
                    case GridBoolProperty.isPath:
                        gridPropertyDetails.isPath = gridProperty.girdBoolValue;
                        break;
                    case GridBoolProperty.canDropItem:
                        gridPropertyDetails.canDropItems = gridProperty.girdBoolValue;
                        break;
                    case GridBoolProperty.canPlaceFurniture:
                        gridPropertyDetails.canPlaceFurniture = gridProperty.girdBoolValue;
                        break;
                    case GridBoolProperty.isNPCObstacle:
                        gridPropertyDetails.isNPCObstacle = gridProperty.girdBoolValue;
                        break;
                    default:
                        break;
                }

                SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y,
                    gridPropertyDetails, gridPropertyDict);
            }
            
            //创建sceneSave变量
            SceneSave sceneSave = new SceneSave();
            
            //将gridPropertyDetail添加进sceneSave
            sceneSave.gridPropertyDetailsDict = gridPropertyDict;

            //如果在当前迭代中在startingScene中设置gridPropertyDict变量
            if (so_GridProperties.sceneName == SceneControllerManager.Instance.startingSceneName)
            {
                this.gridPropertyDict = gridPropertyDict;
            }
            
            GameObjectSave.sceneDataDict.Add(so_GridProperties.sceneName.ToString(), sceneSave);
        }
    }

    private void AfterSceneLoaded()
    {
        //获取grid
        grid = GameObject.FindObjectOfType<Grid>();
    }


    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectsList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectsList.Remove(this);
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //清除sceneSave
        gameObjectSave.sceneDataDict.Remove(sceneName);
        
        //创建新sceneSave变量    
        SceneSave sceneSave = new SceneSave();

        sceneSave.gridPropertyDetailsDict = gridPropertyDict;
        
        GameObjectSave.sceneDataDict.Add(sceneName, sceneSave);
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //根据sceneName在字典中获取SceneSave
        if (GameObjectSave.sceneDataDict.TryGetValue(sceneName, out SceneSave sceneSave))
        {
            if (sceneSave.gridPropertyDetailsDict != null)
            {
                gridPropertyDict = sceneSave.gridPropertyDetailsDict;
            }
        }
    }

    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
    {
        return GetGridPropertyDetails(gridX, gridY,gridPropertyDict);
    }
    
    public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY,
        Dictionary<string, GridPropertyDetails> gridPropertyDetailsDict)
    {
        //通过坐标来确定字符串KEY
        string key = "x" + gridX + "y" + gridY;
        
        GridPropertyDetails gridPropertyDetails;
        
        //根据坐标字符串key来检索字典中存在元素
        if (!gridPropertyDetailsDict.TryGetValue(key, out gridPropertyDetails))
        {
            return null;
        }
        else
        {
            return gridPropertyDetails;
        }
    }

    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
    {
        SetGridPropertyDetails(gridX, gridY, gridPropertyDetails,gridPropertyDict);
    }
    
    public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails,
        Dictionary<string, GridPropertyDetails> gridPropertyDetailsDict)
    {
        string key = "x" + gridX + "y" + gridY;
        
        gridPropertyDetails.gridX = gridX;
        gridPropertyDetails.gridY = gridY;
        
        gridPropertyDetailsDict[key] = gridPropertyDetails;
    }
}