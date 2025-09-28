using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : SingletonMonobehaviour<Player>
{
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private AnimationOverrides animationOverrides;
    private GridCursor gridCursor;
    
    //Movement Parameters
    private float xInput;
    private float yInput;
    private bool isCarrying = false;
    private bool isIdle;
    private bool isLiftingToolRight;
    private bool isLiftingToolLeft;
    private bool isLiftingToolUp;
    private bool isLiftingToolDown;
    private bool isRunnning;
    private bool isUsingToolRight;
    private bool isUsingToolLeft;
    private bool isUsingToolUp;
    private bool isUsingToolDown;
    private bool isSwingingToolRight;
    private bool isSwingingToolLeft;
    private bool isSwingingToolUp;
    private bool isSwingingToolDown;
    private bool isWalking;
    private bool isPickingRight;
    private bool isPickingLeft;
    private bool isPickingUp;
    private bool isPickingDown;
    private ToolEffect toolEffect = ToolEffect.none;

    private Camera mainCamera;
    private bool playerToolUseDisabled = false;

    private Rigidbody2D rigidbody2D;
    private WaitForSeconds useToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;
    
    private Direction playerDirection;

    private List<CharacterAttribute> characterAttributeCustomisationList;
    private float movementSpeed;

    [Tooltip("当装备物品时应当填充该spriterender")]
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    //可变换的玩家attributes
    private CharacterAttribute armsCharacterAttribute;
    private CharacterAttribute toolCharacterAttribute;


    private bool _playerInputIsDisabled = false;
    public bool PlayerInputIsDisabled { get => _playerInputIsDisabled; set => _playerInputIsDisabled = value; }

    protected override void Awake()
    {
        base.Awake();

        rigidbody2D = GetComponent<Rigidbody2D>();

        animationOverrides = GetComponentInChildren<AnimationOverrides>();

        //初始化可转换character Attribute
        armsCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.arms, PartVariantColour.none, PartVariantType.none);

        //初始化character Attribute List
        characterAttributeCustomisationList = new List<CharacterAttribute>();

        //获取场景内主摄相机的实例
        mainCamera = Camera.main;
    }

    private void Update()
    {
        #region Player Input

        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();

            PlayerMovementInput();

            PlayerWalkInput();

            PlayerClickInput();
            
            //PlayerTestInput();
            
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunnning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
        }

        #endregion
    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movementSpeed * Time.deltaTime, yInput * movementSpeed * Time.deltaTime);
        rigidbody2D.MovePosition(rigidbody2D.position + move);
    }    


    private void PlayerWalkInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isWalking = true;
            isRunnning = false;
            isIdle = false;
            movementSpeed = Settings.walkingSpeed;
        }
        else
        {
            isWalking = false;
            isRunnning = true;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;
        }
    }

    private void PlayerClickInput()
    {
        if (!playerToolUseDisabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (gridCursor.CursorIsEnabled)
                {
                    //获取指针girdPosition
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                    
                    //获取玩家gridPosition
                    Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
                        
                    ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
                }
            } 
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition,Vector3Int playerGridPosition)
    {
        ResetMovement();
        
        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        
        //GetSelectedItem Details
        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.player);

        if (itemDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (Input.GetMouseButton(0))
                    {
                        ProcessPlayerClickInputSeed(itemDetails);
                    }
                    break;
                
                case ItemType.Commodity:
                    if (Input.GetMouseButton(0))
                    {
                        ProcessPlayerClickInputCommodity(itemDetails);
                    }
                    break;
                
                case ItemType.Hoeing_tool:
                case ItemType.Watering_tool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;
                
                case ItemType.none:
                    break;
                
                case ItemType.count:
                    break;
            }
        }
    }

    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else if (cursorGridPosition.x < playerGridPosition.x)
        {
            return Vector3Int.left;
        }
        else if(cursorGridPosition.y > playerGridPosition.y)
        {
            return Vector3Int.up;   
        }
        else
        {
            return Vector3Int.down;
        }
        
    }

    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDroped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectItemEvent();
        }
    }
    
    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDroped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails,
        Vector3Int playerGridPosition)
    {
        //Switch based on Item type
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerGridPosition);
                }
                break;
            
            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCurosr(gridPropertyDetails, playerGridPosition);
                }
                break;
        }
    }

    private void WaterGroundAtCurosr(GridPropertyDetails gridPropertyDetails, Vector3Int playerGridPosition)
    {
        StartCoroutine(WaterGroundAtCursorRoutine(gridPropertyDetails, playerGridPosition));
    }
    
    private void HoeGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        //触发动画
        StartCoroutine(HoeGroundAtCursorRoutine(gridPropertyDetails, playerDirection));
    }

    private IEnumerator WaterGroundAtCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;

        //设置Animation重载，将arm动画机切换到工具动画
        toolCharacterAttribute.partVariantType = PartVariantType.wateringCan;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParamters(characterAttributeCustomisationList);

        toolEffect = ToolEffect.watering;
        
        if (playerDirection == Vector3Int.right)
        {
            isLiftingToolRight = true;
        }else if (playerDirection == Vector3Int.left)
        {
            isLiftingToolLeft = true;
        }else if (playerDirection == Vector3Int.up)
        {
            isLiftingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isLiftingToolDown = true;
        }

        yield return liftToolAnimationPause;

        if (gridPropertyDetails.daySinceWatered == -1)
        {
            gridPropertyDetails.daySinceWatered = 0;
        }
        
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);

        yield return afterLiftToolAnimationPause;
        
        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }
    
    private IEnumerator HoeGroundAtCursorRoutine(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;
        
        //设置Animation重载，将arm动画机切换到工具动画
        toolCharacterAttribute.partVariantType = PartVariantType.hoe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParamters(characterAttributeCustomisationList);

        if (playerDirection == Vector3Int.right)
        {
            isUsingToolRight = true;
        }else if (playerDirection == Vector3Int.left)
        {
            isUsingToolLeft = true;
        }else if (playerDirection == Vector3Int.up)
        {
            isUsingToolUp = true;
        }
        else if (playerDirection == Vector3Int.down)
        {
            isUsingToolDown = true;
        }

        yield return useToolAnimationPause;
        
        //设置当前grid已经被挖掘
        if (gridPropertyDetails.daySinceDug == -1)
        {
            gridPropertyDetails.daySinceDug = 0;
        }
        
        //更新被挖掘后的gridProperty
        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY,gridPropertyDetails);

        //display dug gird tiles
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails); 
        
        //避免动画更新过快
        yield return afterUseToolAnimationPause;
        
        PlayerInputIsDisabled = false;
        playerToolUseDisabled = false;
    }
    
    private void PlayerMovementInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        if (xInput != 0 && yInput != 0)
        {
            xInput = xInput * 0.71f;
            yInput = yInput * 0.71f;
        }

        if (xInput != 0 || yInput != 0)
        {
            isRunnning = true;
            isWalking = false;
            isIdle = false;
            movementSpeed = Settings.runningSpeed;

            //获取玩家朝向，用于保存游戏功能
            if (xInput < 0)
            {
                playerDirection = Direction.left;
            }
            else if (xInput > 0)
            {
                playerDirection = Direction.right;
            }
            else if (yInput < 0)
            {
                playerDirection = Direction.down;
            }
            else
            {
                playerDirection = Direction.up;
            }
        }

        if (xInput == 0 && yInput == 0)
        {
            isRunnning = false;
            isWalking = false;
            isIdle = true;
        }

    }

    private void ResetAnimationTriggers()
    {
        isPickingRight = false;
        isPickingLeft = false;
        isPickingUp = false;
        isPickingDown = false;
        isLiftingToolRight = false;
        isLiftingToolLeft = false;
        isLiftingToolUp = false;
        isLiftingToolDown = false;
        isUsingToolRight = false;
        isUsingToolLeft = false;
        isUsingToolUp = false;
        isUsingToolDown = false;
        isSwingingToolRight = false;
        isSwingingToolLeft = false;
        isSwingingToolUp = false;
        isSwingingToolDown = false;
        toolEffect = ToolEffect.none;
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(0f, 0f, 0f, 0f);

        //设置基础角色arms customisation
        armsCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(armsCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParamters(characterAttributeCustomisationList);

        isCarrying = false;
    }


    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

        if(itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            //设置“carry”角色 arms customisation
            armsCharacterAttribute.partVariantType = PartVariantType.carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(armsCharacterAttribute);
            animationOverrides.ApplyCharacterCustomisationParamters(characterAttributeCustomisationList);

            isCarrying = true;
        }
    }


    public Vector3 GetPlayerViewportPosition()
    {
        //获取玩家在摄像机中的位置坐标
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }

    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }


    // private void PlayerTestInput()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         TimeManager.Instance.TestAdvanceGameDay();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.G))
    //     {
    //         TimeManager.Instance.TestAdvanceGameMinute();
    //     }
    //
    //     if (Input.GetKeyDown(KeyCode.L))
    //     {
    //         SceneControllerManager.Instance.FadeAndLoadScene(SceneName.Scene1_Farm.ToString(),transform.position);
    //     }
    // }
    //
    private void ResetMovement()
    {
        xInput = 0;
        yInput = 0;
        isRunnning = false;
        isWalking = false;
        isIdle = true;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();


        //调用事件给所有监听者更新玩家input
        EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunnning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
    }
}