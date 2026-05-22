using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Status, ICommand, IAnimationCharacter
{
    private PlayerInput playerInput;

    [Header("プレイヤーデータ")]
    [SerializeField] private PlayerData playerData;
    
    [Header("PlayerRideCheck")]
    [SerializeField] private PlayerRideCheck playerRideCheck;
    [Header("現在のバディモン")]
    [SerializeField] private GameObject currentBuddyMonster;
    public GameObject CurrentBuddyMonster => currentBuddyMonster;
    
    [Header("現在のコマンド")]
    [SerializeField] private CommandState currentCommandState;
    public CommandState GetCommand()
    {
        return currentCommandState;
    }
    public void SetCommand(CommandState commandState)
    {
        currentCommandState = commandState;
    }
    
    [Header("コマンドUI")]
    [SerializeField] private CommandUI commandUI;
    public void ShowCommandUI(bool flag)
    {
        commandUI.ToggleCommandUI(flag);
    }
    [Header("プレイヤーキャラクター固有のエフェクト情報")]
    [SerializeField] private List<EffectData> _effectData;
    
    public CharacterAttributesType GetAttributes()
    {
        return playerData.AttributesType;
    }

    public bool IsRide { get; private set; } //バディモンに乗っている

    private MagicPanel magicPanel;
    private BuddyMonster buddyMonster;
    private Animator animator;
    private GameObject mainCamera;
    private Rigidbody rb;

    private Vector2 moveInput; //移動入力を保持
    private Vector3 moveOutPut; //カメラと移動入力を含めたベクトル
    private Vector2 massMoveInput; //マス移動入力を保持
    
    private Character _character;
    public Character GetCharacter(){ return _character;}

    void Awake()
    {
        // キャラクターの情報を作成
        _character = new Character(playerData, this.gameObject, true, commandUI,GetComponent<CharacterCameraSettings>());
        _character.EventsSystem.onHitEffect += Hit;
        _character.EventsSystem.onDeathEffect += Death;
    }

    void Start()
    {
        //InputSystemを使えるようにする
        playerInput = new PlayerInput();
        playerInput.Enable();

        magicPanel = GetComponent<MagicPanel>();
        animator = GetComponent<Animator>();

        mainCamera = GameObject.Find("Main Camera");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        /*
        //ライドボタンが押されてて、バディモンに乗れる距離にいるとき
        if (playerInput.Player.MonsterRide.triggered && playerRideCheck.IsCanRide)
        {
            if (!IsRide) //乗る
            {
                IsRide = true;
                Debug.Log("バディモンに乗る");

                //バディモンを取得する
                if(playerRideCheck.BuddyMonObj != null)
                {
                    buddyMonster = playerRideCheck.BuddyMonObj.GetComponent<BuddyMonster>();
                }
            }
            else //降りる
            {
                IsRide = false;
                Debug.Log("バディモンから降りる");
            }

            BuddyMonRide(IsRide);
        }

        if (playerInput.Player.Jump.triggered && !IsRide) //ジャンプ
        {
            Jump();
        }

        if (playerInput.Player.MagicPanel.triggered) //マジックパネルを開く
        {
            magicPanel.MagicPanelToggle();
        }
        */

        //マジックパネルを開いている状態でのマス移動
        if (magicPanel.IsPanelOpen && massMoveInput.magnitude > 0)
        {
            magicPanel.PanelMassMovement(massMoveInput);

            //入力を0にする
            massMoveInput = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        //if (IsRide || EncounterManager.instance.IsFieldSettingsComplete) return;

        //カメラの向きを取得
        var cameraForward = mainCamera.transform.forward;
        var cameraRight = mainCamera.transform.right;

        //カメラの向きに合わせたベクトルを作成
        moveOutPut = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        rb.velocity = new Vector3(moveOutPut.x * playerData.MoveSpeed, rb.velocity.y, moveOutPut.z * playerData.MoveSpeed);
    }

    /// <summary>
    /// 移動Action（PlayerInputから呼ばれる）
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        //マジックパネルを開いているときは、動けないようにする
        if (magicPanel.IsPanelOpen) return;

        moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    private void Jump()
    {
        //地面にいないときは、処理をしない
        if (!IsGroudCheck() || magicPanel.IsPanelOpen) return;
        rb.AddForce(Vector3.up * playerData.JumpAbility, ForceMode.Impulse);
    }

    /// <summary>
    /// 地面にいるかの判定
    /// </summary>
    /// <returns>Rayが地面に当たれば、tureを返す</returns>
    private bool IsGroudCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, playerData.RayDistance);
    }

    /// <summary>
    /// マスの移動Action（PlayerInputから呼ばれる）
    /// </summary>
    public void OnMassMove(InputAction.CallbackContext context)
    {
        //マジックパネルを開いていないときは、処理をしない
        if (!magicPanel.IsPanelOpen) return;
        
        massMoveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// バディモンに乗る
    /// </summary>
    private void BuddyMonRide(bool isRide)
    {
        if (isRide)
        {
            //バディモンの子オブジェクトになり、追従するようにする
            rb.isKinematic = true;
            transform.SetParent(playerRideCheck.BuddyMonObj.transform);
            transform.position = buddyMonster.RidePosition.position;
        }
        else
        {
            //子オブジェクトを解除する
            rb.isKinematic = false;
            transform.SetParent(null);
        }
    }

    /// <summary>
    /// ダメージを受けた時
    /// </summary>
    private void Hit()
    {
        SetAnimationPlay("Hit");
    }
    
    /// <summary>
    /// 死亡した時
    /// </summary>
    private void Death()
    {
        animator.SetBool("IsDeath", true);
    }
    
    public void SetAnimationPlay(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    public Transform GetEffectTransform(string effectPosName)
    {
        Transform pos = null;
        foreach (var data in _effectData)
        {
            if (data._effectName == effectPosName)
            {
                pos = data._effectTransform;
            }
        }

        return pos;
    }
}
