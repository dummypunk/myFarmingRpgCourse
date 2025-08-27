using System;
using UnityEngine;
using System.Collections.Generic;

public class Player : SingletonMonobehaviour<Player>
{
    private AnimationOverrides animationOverrides;
    
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

    private Rigidbody2D rigidbody2D;

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