using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(GenerateGUID))]

public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDict;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray;
    [SerializeField] private Tile[] dugGround = null;

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

    private void ClearDisplayGroundDecorations()
    {
        groundDecoration1.ClearAllTiles();
        groundDecoration2.ClearAllTiles();
    }

    private void ClearDisplayGridPropertiesDetails()
    {
        ClearDisplayGroundDecorations();
    }

    public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daySinceDug > -1)
        {
            ConnectDugGround(gridPropertyDetails);
        }
    }

    private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
    {
        //根据四周的tiles dug情况，来选择填充的tile
        Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);
        
        //在中心tile dug后，随后根据中心的tile来设置上下左右四个tile
        GridPropertyDetails adjanceGridPropertyDetails;
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile1);
        }
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile1);
        }
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceDug > -1)
        {
            Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile1);
        }
    }

    private Tile SetDugTile(int xGrid, int yGrid)
    {
        //判断中心tile四周tile是否被dig
        bool upDug    = IsGridSquareDug(xGrid, yGrid + 1);
        bool downDug  = IsGridSquareDug(xGrid, yGrid - 1);
        bool rightDug = IsGridSquareDug(xGrid + 1, yGrid);
        bool leftDug  = IsGridSquareDug(xGrid - 1, yGrid);

        #region 根据周围tile是否被dig来选择合适的tile

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug) 
        {
            return dugGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return dugGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return dugGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return dugGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return dugGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return dugGround[8]; 
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return dugGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return dugGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return dugGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return dugGround[15];
        }

        return null;

        #endregion
    }

    private bool IsGridSquareDug(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daySinceDug > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DisplayGridPropertyDetails()
    {
        //遍历所有grid items
        foreach (KeyValuePair<string,GridPropertyDetails> item in gridPropertyDict)
        {
            GridPropertyDetails gridPropertyDetails = item.Value;
            
            DisplayDugGround(gridPropertyDetails);
        }
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
        
        //在场景加载后获取tilemap
        groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
        groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
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

            //如果gird Property存在
            if (gridPropertyDict.Count > 0)
            {
                //销毁当前场景存在的decoration 
                ClearDisplayGroundDecorations();
                
                //在当前场景下初始化grid properties
                DisplayGridPropertyDetails();
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