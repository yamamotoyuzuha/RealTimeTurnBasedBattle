using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BuddyMonster : MonoBehaviour, Status, ICommand
{
    private BuddyMonsterInput buddyMonsterInput;

    [Header("参照")]
    [Header("Player")]
    [SerializeField] private Player player;

    [Header("バディモンデータ")]
    [SerializeField] private BuddyMonsterData buddyMonsterData;
    public CharacterBaseData GetData()
    {
        return buddyMonsterData;
    }
    public BuddyMonsterData BuddyMonsterData => buddyMonsterData;
    
    [Header("ライド位置")]
    [SerializeField] private Transform ridePosition;
    public Transform RidePosition => ridePosition;
    
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
    
    /// <summary>
    /// Characterのステータス
    /// </summary>
    private CharacterBaseStatus characterBaseStatus;
    public CharacterBaseStatus GetCharacterStatus()
    {
        return characterBaseStatus;
    }
    public int GetMp()
    {
        return characterBaseStatus.Mp;
    }
    public int GetSpeed()
    {
        return characterBaseStatus.Speed;
    }
    public CharacterAttributesType GetAttributes()
    {
        return buddyMonsterData.AttributesType;
    }
    
    private MagicPanel magicPanel;
    private GameObject mainCamera;
    private Rigidbody rb;

    private Vector2 moveInput; //移動入力を保持
    private Vector3 moveOutPut; //カメラと移動入力を含めたベクトル
    private Vector2 massMoveInput; //マス移動入力を保持

    private void Awake()
    {
        characterBaseStatus = new CharacterBaseStatus
            (buddyMonsterData.Hp, buddyMonsterData.Mp, buddyMonsterData.Attack, buddyMonsterData.Defense, 
                buddyMonsterData.Speed, this.gameObject, 0, buddyMonsterData.SpecialMove);
    }

    void Start()
    {
        //InputSystemを使えるようにする
        buddyMonsterInput = new BuddyMonsterInput();
        buddyMonsterInput.Enable();
        
        magicPanel = GetComponent<MagicPanel>();
        
        mainCamera = GameObject.Find("Main Camera");
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        /*
        if (buddyMonsterInput.Player.Jump.triggered && player.IsRide)
        {
            //プレイヤーが乗るとジャンプができなくなる
            //プレイヤーをどかすと、ジャンプが出来るようになるからプレイヤーが頭上にいることが原因かも

            Jump();
            Debug.Log("バディモンがジャンプ");
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
        //if (!player.IsRide) return;

        //カメラの向きを取得する
        var cameraForward = mainCamera.transform.forward;
        var cameraRight = mainCamera.transform.right;

        //カメラの向きに合わせたベクトルを作成
        moveOutPut = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
        rb.velocity = new Vector3(moveOutPut.x * buddyMonsterData.BuddyMonMoveSpeed, rb.velocity.y, moveOutPut.z * buddyMonsterData.BuddyMonMoveSpeed);
    }

    /// <summary>
    /// 移動Action（BuddyMonsterInputから呼ばれる）
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
        if (!IsGroudCheck()) return;
        rb.AddForce(Vector3.up * buddyMonsterData.JumpingAbility, ForceMode.Impulse);
    }

    /// <summary>
    /// 地面にいるかの判定
    /// </summary>
    /// <returns>Rayが地面に当たれば、tureを返す</returns>
    private bool IsGroudCheck()
    {
        return Physics.Raycast(transform.position, Vector3.down, buddyMonsterData.RayDistance);
    }
    
    /// <summary>
    /// マスの移動Action（BuddyMonsterInputから呼ばれる）
    /// </summary>
    public void OnMassMove(InputAction.CallbackContext context)
    {
        //マジックパネルを開いていないときは、処理をしない
        if (!magicPanel.IsPanelOpen) return;
        
        massMoveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// プレイヤーがバディモンに乗っている状態で行える処理
    /// </summary>
    private void RideAction()
    {

    }
}
