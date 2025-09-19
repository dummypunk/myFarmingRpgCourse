using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Transform cursorRectTransform;
    [SerializeField] private Sprite greenCursorSprite;
    [SerializeField] private Sprite redCursorSprite;

    private bool _cursorPositionIsValid = false;
    public bool CursorPositionIsValid { get => _cursorPositionIsValid; set => _cursorPositionIsValid = value; }

    private int _itemUseGridRadius = 0;
    public int ItemUseGridRadius { get => _itemUseGridRadius; set => _itemUseGridRadius = value; }
    
    private ItemType _selectedItemType;
    public ItemType SelectedItemType { get => _selectedItemType; set => _selectedItemType = value; }

    private bool _cursorIsEnabled = false;
    public bool CursorIsEnabled { get => _cursorIsEnabled; set => _cursorIsEnabled = value; }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = gameObject.GetComponent<Canvas>();
    }


    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    private Vector3Int DisplayCursor()
    {
        if (grid != null)
        {
            //获取grid坐标
            Vector3Int gridPosition = GetGridPositionForCursor();

            //获取顽疾grid坐标
            Vector3Int playerGridPosition = GetGridPositionForPlayer();
            
            //设置cursor的可见性
            SetCursorValidity(gridPosition, playerGridPosition);
            
            //获取rectTransform Position
            cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);
            
            return gridPosition;
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();
        
        //检查物品放置半径是否在规定范围内
        if (Math.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius ||
            Math.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }
        
        //获取选中物品的物品详细
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }
        
        //在当前指针位置获取gridPropertyDetails
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        if (gridPropertyDetails != null)
        {
            //根据选中的inventory Item和girdPropertiesDetails来决定cursor vaild
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (!IsCursorValidForSeed(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Commodity:
                    if (!IsCursorValidForCommodity(gridPropertyDetails))
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                
                case ItemType.none:
                    break;
                
                case ItemType.count:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }


    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }
    
    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }
    
    private void SceneLoaded()
    {
        grid = FindObjectOfType<Grid>();
    }

    public void DisableCursor()
    {
        cursorImage.color = Color.clear;
        CursorIsEnabled = false;
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1, 1, 1, 1f);
        CursorIsEnabled = true;
    }
    
    public Vector3Int GetGridPositionForCursor()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScenePosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScenePosition, cursorRectTransform, canvas);
    }

    
    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItems;
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItems;
    }
    
}
