using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Security.Cryptography;

[RequireComponent(typeof(GenerateGUID))]

public class GridPropertiesManager : SingletonMonobehaviour<GridPropertiesManager>, ISaveable
{
    private Transform cropParentTransform;
    private Tilemap groundDecoration1;
    private Tilemap groundDecoration2;
    private Grid grid;
    private Dictionary<string, GridPropertyDetails> gridPropertyDict;
    [SerializeField] private SO_CropDetailsList so_CropDetailsList;
    [SerializeField] private SO_GridProperties[] so_gridPropertiesArray;
    [SerializeField] private Tile[] dugGround = null;
    [SerializeField] private Tile[] wateredGround = null;   

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
        EventHandler.AdvanceGameDay += AdvanceDay;
    }

    private void OnDisable()
    {
        ISaveableDeregister();
        
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.AdvanceGameDay -= AdvanceDay;
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

    private void ClearDisplayAllPlantedCrops()
    {
        Crop[] cropArray;
        cropArray = FindObjectsOfType<Crop>();

        foreach (Crop crop in cropArray)
        {
            Destroy(crop.gameObject);
        }
    }
    
    private void ClearDisplayGridPropertiesDetails()
    {
        ClearDisplayGroundDecorations();

        ClearDisplayAllPlantedCrops();
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

    public void DisplayWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.daySinceWatered > -1)
        {
            ConnectWateredGround(gridPropertyDetails);
        }
    }

    private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
    {
        //根据四周的tiles dug情况，来选择填充的tile
        Tile dugTile0 = SetWateredGround(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
        groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0), dugTile0);
        
        //在中心tile dug后，随后根据中心的tile来设置上下左右四个tile
        GridPropertyDetails adjanceGridPropertyDetails;
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceWatered > -1)
        {
            Tile dugTile1 = SetWateredGround(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1, 0), dugTile1);
        }
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceWatered > -1)
        {
            Tile dugTile1 = SetWateredGround(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1, 0), dugTile1);
        }
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceWatered > -1)
        {
            Tile dugTile1 = SetWateredGround(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX + 1, gridPropertyDetails.gridY, 0), dugTile1);
        }
        
        adjanceGridPropertyDetails = GetGridPropertyDetails(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
        if (adjanceGridPropertyDetails != null && adjanceGridPropertyDetails.daySinceWatered > -1)
        {
            Tile dugTile1 = SetWateredGround(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX - 1, gridPropertyDetails.gridY, 0), dugTile1);
        }
    }

    private Tile SetWateredGround(int xGrid, int yGrid)
    {
        bool upDug    = IsGridSquareWatered(xGrid, yGrid + 1);
        bool downDug  = IsGridSquareWatered(xGrid, yGrid - 1);
        bool rightDug = IsGridSquareWatered(xGrid + 1, yGrid);
        bool leftDug  = IsGridSquareWatered(xGrid - 1, yGrid);

        #region 根据周围tile是否被watered来选择合适的tile

        if (!upDug && !downDug && !rightDug && !leftDug)
        {
            return wateredGround[0];
        }
        else if (!upDug && downDug && rightDug && !leftDug) 
        {
            return wateredGround[1];
        }
        else if (!upDug && downDug && rightDug && leftDug)
        {
            return wateredGround[2];
        }
        else if (!upDug && downDug && !rightDug && leftDug)
        {
            return wateredGround[3];
        }
        else if (!upDug && downDug && !rightDug && !leftDug)
        {
            return wateredGround[4];
        }
        else if (upDug && downDug && rightDug && !leftDug)
        {
            return wateredGround[5];
        }
        else if (upDug && downDug && rightDug && leftDug)
        {
            return wateredGround[6];
        }
        else if (upDug && downDug && !rightDug && leftDug)
        {
            return wateredGround[7];
        }
        else if (upDug && downDug && !rightDug && !leftDug)
        {
            return wateredGround[8]; 
        }
        else if (upDug && !downDug && rightDug && !leftDug)
        {
            return wateredGround[9];
        }
        else if (upDug && !downDug && rightDug && leftDug)
        {
            return wateredGround[10];
        }
        else if (upDug && !downDug && !rightDug && leftDug)
        {
            return wateredGround[11];
        }
        else if (upDug && !downDug && !rightDug && !leftDug)
        {
            return wateredGround[12];
        }
        else if (!upDug && !downDug && rightDug && !leftDug)
        {
            return wateredGround[13];
        }
        else if (!upDug && !downDug && rightDug && leftDug)
        {
            return wateredGround[14];
        }
        else if (!upDug && !downDug && !rightDug && leftDug)
        {
            return wateredGround[15];
        }

        return null;

        #endregion

    }

    private bool IsGridSquareWatered(int xGrid, int yGrid)
    {
        GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

        if (gridPropertyDetails == null)
        {
            return false;
        }
        else if(gridPropertyDetails.daySinceWatered >-1)
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
            
            DisplayWateredGround(gridPropertyDetails);
            
            DisplayPlantedCrop(gridPropertyDetails);
        }
    }

    public void DisplayPlantedCrop(GridPropertyDetails gridPropertyDetails)
    {
        if (gridPropertyDetails.seedItemCode > -1)
        {
            // 获取作物详情
            CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

            if (cropDetails != null)
            {
                // 要使用的预制体
                GameObject cropPrefab;
    
                // 在网格位置实例化作物预制体
                int growthStages = cropDetails.growthDays.Length;
            
                int currentGrowthStage = 0;
    
                for (int i = growthStages - 1; i >= 0; i--)
                {
                    if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                    {
                        currentGrowthStage = i;
                        break;
                    }
                }

                cropPrefab = cropDetails.growthPrefab[currentGrowthStage];
            
                Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                Vector3 worldPosition = groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
            
                worldPosition = new Vector3(worldPosition.x + Settings.gridCellSize / 2, worldPosition.y, worldPosition.z);

                GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);
            
                cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                cropInstance.transform.SetParent(cropParentTransform);
                cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            }
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
        if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
        {
            cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
        }
        else
        {
            cropParentTransform = null;
        }
         
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

    public Crop GetCropObjectAtLocation(GridPropertyDetails gridPropertyDetails)
    {
        Vector3 worldPosition = grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
        Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);
        
        //遍历碰撞器数组
        Crop crop = null;

        for (int i = 0; i < collider2DArray.Length; i++)
        {
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
            
            crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
            if (crop != null && crop.cropGridPosition == new Vector2Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY))
                break;
        }
        
        return crop;
    }

    public CropDetails GetCropDetails(int seedItemCode)
    {
        return so_CropDetailsList.GetCropDetails(seedItemCode);
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
    
    private void AdvanceDay(int arg1, Season arg2, int arg3, string arg4, int arg5, int arg6, int arg7)
    {
        //清除刷新所有gird Property Details
        ClearDisplayGridPropertiesDetails();
        
        //遍历所有场景
        foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
        {
            //根据场景名字获取当前场景的save data
            if (GameObjectSave.sceneDataDict.TryGetValue(so_GridProperties.sceneName.ToString(),
                    out SceneSave sceneSave))
            {
                if (sceneSave.gridPropertyDetailsDict != null)
                {
                    for (int i = sceneSave.gridPropertyDetailsDict.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<string,GridPropertyDetails> item = sceneSave.gridPropertyDetailsDict.ElementAt(i);
                        
                        GridPropertyDetails gridPropertyDetails = item.Value;

                        //更新庄稼生长时间
                        if (gridPropertyDetails.growthDays > -1)
                        {
                            gridPropertyDetails.growthDays +=1;
                        }
                        
                        //天数更新后更新土地浇灌状态
                        if (gridPropertyDetails.daySinceWatered > -1)
                        {
                            gridPropertyDetails.daySinceWatered = -1;
                        }
                        
                        SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails, sceneSave.gridPropertyDetailsDict);
                    }
                }
            }
        }
        DisplayGridPropertyDetails();
    }
}