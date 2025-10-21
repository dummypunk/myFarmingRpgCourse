using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : SingletonMonobehaviour<Player>
{
    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds afterPickAnimationPause; 
    private AnimationOverrides animationOverrides;
    private GridCursor gridCursor;
    private Cursor cursor;
    
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
    private WaitForSeconds pickAnimationPause;
    
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
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantColour.none, PartVariantType.hoe);

        //初始化character Attribute List
        characterAttributeCustomisationList = new List<CharacterAttribute>();

        //获取场景内主摄相机的实例
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadFadeOutEvent += DisablePlayerInputAndResetMovement;
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadFadeOutEvent -= DisablePlayerInput;
        EventHandler.AfterSceneLoadFadeInEvent -= EnablePlayerInput;

    }

    private void Start()
    {
        gridCursor = FindObjectOfType<GridCursor>();
        cursor = FindObjectOfType<Cursor>();
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        pickAnimationPause = new WaitForSeconds(Settings.pickAnimationPause);
        afterPickAnimationPause = new WaitForSeconds(Settings.afterPickAnimationPause);  
    }

    
    private void Update()
    {
        #region Player Input

        PlayerTestInput();
        
        if (!PlayerInputIsDisabled)
        {
            ResetAnimationTriggers();

            PlayerMovementInput();

            PlayerWalkInput();

            PlayerClickInput();

            
            
            EventHandler.CallMovementEvent(xInput, yInput, isWalking, isRunnning, isIdle, isCarrying, toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
                isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                false, false, false, false);
        }

        #endregion
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
                if (gridCursor.CursorIsEnabled || cursor.CursorIsEnabled)
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
                        ProcessPlayerClickInputSeed(gridPropertyDetails,itemDetails);
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
                case ItemType.Reaping_tool:
                case ItemType.Collecting_tool:
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

    private Vector3Int GetPlayerDirection(Vector3 cursorPosition, Vector3 playerPosition)
    {
        if (cursorPosition.x > playerPosition.x
            &&
            cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2)
            &&
            cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2))
        {
            return Vector3Int.right;
        }
        else if (cursorPosition.x < playerPosition.x
                 &&
                 cursorPosition.y < (playerPosition.y + cursor.ItemUseRadius / 2)
                 &&
                 cursorPosition.y > (playerPosition.y - cursor.ItemUseRadius / 2)
                )
        {
            return Vector3Int.left;
        }
        else if (cursorPosition.y > playerPosition.y)
        {
            return Vector3Int.up;
        }
        else
        {
            return Vector3Int.down;
        }
    }

    private void ProcessPlayerClickInputSeed(GridPropertyDetails gridPropertyDetails ,ItemDetails itemDetails)
    {
        if (itemDetails.canBeDroped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daySinceDug > -1 &&
            gridPropertyDetails.seedItemCode == -1)
        {
            PlantSeedAtCursor(gridPropertyDetails, itemDetails);
        }
        
        if (itemDetails.canBeDroped && gridCursor.CursorPositionIsValid && gridPropertyDetails.daySinceDug == -1)
        {
            EventHandler.CallDropSelectItemEvent();
        }
    }

    private void PlantSeedAtCursor(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        //仅在有cropdetails中存在seedItem code中执行
        if (GridPropertiesManager.Instance.GetCropDetails(itemDetails.itemCode) != null)
        {
            gridPropertyDetails.seedItemCode = itemDetails.itemCode;
            gridPropertyDetails.growthDays = 0;
        
            GridPropertiesManager.Instance.DisplayPlantedCrop(gridPropertyDetails);
        
            EventHandler.CallRemoveSelectedItemFromInventoryEvent();
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
        Vector3Int playerDirection)
    {
        //Switch based on Item type
        switch (itemDetails.itemType)
        {
            case ItemType.Hoeing_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    HoeGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            
            case ItemType.Watering_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCurosr(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.Reaping_tool:
                if (cursor.CursorPositionIsValid)
                {
                    playerDirection = GetPlayerDirection(cursor.GetWorldPositionForCursor(), GetPlayerCentrePosition());
                    ReapInPlayerDirectionAtCursor(itemDetails, playerDirection);
                }
                break;
            case ItemType.Collecting_tool:
                if (gridCursor.CursorPositionIsValid)
                {
                    CollectInPlayerDirection(gridPropertyDetails,itemDetails ,playerDirection);
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

        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);
        
        yield return afterLiftToolAnimationPause;
        
        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }

    private void CollectInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails , Vector3Int playerDirection)
    {
        StartCoroutine(CollectInPlayerDirectionRoutine(gridPropertyDetails,equippedItemDetails, playerDirection));
    }

    private IEnumerator CollectInPlayerDirectionRoutine(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;
        
        ProcessCropWithEquippedItemInPlayerDirection(gridPropertyDetails, equippedItemDetails, playerDirection);
        
        yield return liftToolAnimationPause;
        
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

    private void ReapInPlayerDirectionAtCursor(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        StartCoroutine(ReapInPlayerDirectionAtCursorRoutine(itemDetails, playerDirection));
    }

    private IEnumerator ReapInPlayerDirectionAtCursorRoutine(ItemDetails itemDetails, Vector3Int playerDirection)
    {
        PlayerInputIsDisabled = true;
        playerToolUseDisabled = true;
        
        //设置工具动画重载动画机
        toolCharacterAttribute.partVariantType = PartVariantType.scythe;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        animationOverrides.ApplyCharacterCustomisationParamters(characterAttributeCustomisationList);
        
        //根据玩家方向进行收割逻辑
        UseToolInPlayerDirection(itemDetails, playerDirection);

        yield return useToolAnimationPause;
        
        playerToolUseDisabled = false;
        PlayerInputIsDisabled = false;
    }

    private void UseToolInPlayerDirection(ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        switch (equippedItemDetails.itemType)
        {
            case ItemType.Reaping_tool:
                if (playerDirection == Vector3Int.right)
                {
                    isSwingingToolRight = true;
                }
                else if (playerDirection == Vector3Int.left)
                {
                    isSwingingToolLeft = true;
                }
                else if (playerDirection == Vector3Int.up)
                {
                    isSwingingToolUp = true;
                }
                else if (playerDirection == Vector3Int.down)
                {
                    isSwingingToolDown = true;
                }
                break;
        }
        
        //根据玩家的方向来定义碰撞测试的中心点
        Vector2 point = new Vector2(GetPlayerCentrePosition().x + playerDirection.x * equippedItemDetails.itemUseRadius/2,
            GetPlayerCentrePosition().y + playerDirection.y * equippedItemDetails.itemUseRadius/2);
        
        //定义碰撞测试所用的方体大小
        Vector2 size = new Vector2(equippedItemDetails.itemUseRadius, equippedItemDetails.itemUseRadius);
        
        //根据中心点和方体，来获得在范围内带有collider2d 的item组件
        Item[] itemArray = HelperMethods.GetComponentsAtBoxLocationNonAlloc<Item>(Settings.maxCollidersToTestPerReapSwing, point, size, 0);

        int reapableItemCount = 0;
        
        //遍历所有检索到的item
        for (int i = itemArray.Length - 1; i >= 0; i--)
        {
            if (itemArray[i] != null)
            {
                //如果物品为可收割，销毁物品
                if (InventoryManager.Instance.GetItemDetails(itemArray[i].ItemCode).itemType == ItemType.Reapable_sceneary)
                {
                    //产生effect
                    Vector3 effectPosition = new Vector3(itemArray[i].transform.position.x,itemArray[i].transform.position.y + Settings.gridCellSize/2f, itemArray[i].transform.position.z);

                    EventHandler.CallHarvestActionEffectEvent(effectPosition, HarvestActionEffect.reaping);
                    
                    Destroy(itemArray[i].gameObject);
                
                    reapableItemCount++;

                    if (reapableItemCount >= Settings.maxCollidersToTestPerReapSwing)
                    {
                        break;
                    }
                }
            }
        }
    }

    private void ProcessCropWithEquippedItemInPlayerDirection(GridPropertyDetails gridPropertyDetails, ItemDetails equippedItemDetails, Vector3Int playerDirection)
    {
        switch (equippedItemDetails.itemType)
        {
            case ItemType.Collecting_tool:
                if (playerDirection == Vector3Int.right)
                    isPickingRight = true;
                if(playerDirection == Vector3Int.left)
                    isPickingLeft = true;
                if(playerDirection == Vector3Int.up)
                    isPickingUp = true;
                if(playerDirection == Vector3Int.down)
                    isPickingDown = true;
                break;
        }

        //在指针位置找到庄稼
        Crop crop = GridPropertiesManager.Instance.GetCropObjectAtLocation(gridPropertyDetails);

        if (crop != null)
        {
            switch (equippedItemDetails.itemType)
            {
                case ItemType.Collecting_tool:
                    crop.ProcessToolAction(equippedItemDetails, isPickingRight, isPickingLeft, isPickingUp, isPickingDown);
                    break;
            }
        }
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

    public Vector3 GetPlayerCentrePosition()
    {
        return new Vector3(transform.position.x, transform.position.y + Settings.playerCentreYOffset, transform.position.z);
    }

    public void EnablePlayerInput()
    {
        PlayerInputIsDisabled = false;
    }

    public void DisablePlayerInput()
    {
        PlayerInputIsDisabled = true;
    }


    
    
    private void PlayerTestInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }
    
        if (Input.GetKeyDown(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }
    }
    
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