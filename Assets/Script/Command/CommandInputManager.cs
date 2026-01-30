using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandInputManager : MonoBehaviour
{
    public static CommandInputManager Instance;
    private CommandInput commandInput;
    
    [Header("TurnManager")]
    [SerializeField] private TurnManager turnManager;
    [Header("BattleUI")]
    [SerializeField] private BattleCommandUI _battleCommandUI;
    [Header("BattleCameraAngleManager")]
    [SerializeField] private BattleCameraAngleManager _battleCameraAngleManager;
    
    /// <summary>
    /// コマンドの入力が完了
    /// </summary>
    public Action onCommandInputComplete;
    public bool isCommandSelected;
    /// <summary>
    /// コマンド入力の可否を変更する
    /// </summary>
    /// <param name="flag">true：可能　false：不可能</param>
    public void SetCommandSelected(bool flag)
    {
        isCommandSelected = flag;
    }
    //各コマンドのデータを保持
    public MagicBaseData CurrentMagic { get; private set; }
    public ItemBaseData CurrentItem { get; private set; }
    
    /// <summary>
    /// 魔法コマンドを選択
    /// true：選択　false：選択していない
    /// </summary>
    private bool isMagicUISelected;
    /// <summary>
    /// 攻撃コマンドを選択
    /// true：選択　false：選択していない
    /// </summary>
    private bool isAttackUISelected;
    /// <summary>
    /// アイテムコマンドを選択
    /// true：選択　false：選択していない
    /// </summary>
    private bool isItemUISelected;
    /// <summary>
    /// コマンドを確定されている状態
    /// true；確定　false：未確定
    /// </summary>
    private bool isConfirmCommand;
    private CommandUI currentCharacterCommandUI; //現在のターンのキャラのCommandUIを保持
    
    //CommandInputManagerでやること
    //TODO：コマンドを選択し終わって、行動が終わったらＮｏｎｅにする
    //TODO：どこかのタイミングでコマンドのデータをリセットするようにする　ターン終了後とか
    //TODO：↑バトルマネージャー側でリセットコマンド関数を読んであげる

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        //InputSystemを使えるようにする
        commandInput = new CommandInput();
        commandInput.Enable();
    }
    
    private void Update()
    {
        DefenseActionInput();
        
        //コマンド選択が不可能の時は処理をしない
        if(!isCommandSelected) return;
        
        MagicCommandInput();
        CommandSelected();

        //デバッグ用　コマンド決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            /*
            //コマンドが確定している状態ならターンを開始する
            if (isConfirmCommand)
            {
                var cameraSet = turnManager.CharacterInfos[turnManager.CurrentTurnCharacter].cameraSettings;
                _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.MagicPanelStart,
                    cameraSet.MagicPanelStartCamPosF, cameraSet.MagicPanelStartCamPosL);
                _battleCommandUI.UndoMagicUIText();
                BattleOperatingInstructionsUI.Instance.HiddenUI();
                turnManager.TurnCharacterCommandUI(turnManager.CurrentTurnCharacter, false);
                ResetCommandFlag();
                onCommandInputComplete?.Invoke();
                isConfirmCommand = false; //コマンドを確定していない状態にする
            }
            */
            ConfirmCommand();
        }

        //デバック用　バトル開始
        if (Input.GetKeyDown(KeyCode.K))
        {
            turnManager.TurnCharacterCommandUI(turnManager.CurrentTurnCharacter, false);
            ResetCommandFlag();
            onCommandInputComplete?.Invoke();
        }
    }

    /// <summary>
    /// どのコマンドで確定したかを判定してターンを開始する
    /// </summary>
    private void ConfirmCommand()
    {
        if(!isConfirmCommand) return;

        var cameraSet = turnManager.CharacterInfos[turnManager.CurrentTurnCharacter].cameraSettings;
        //魔法の場合、魔法パネルがあるためそのカメラアングルに移動させる
        if (isMagicUISelected)
        {
            _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.MagicPanelStart,
                cameraSet.MagicPanelStartCamPosF, cameraSet.MagicPanelStartCamPosL);
        }
        else
        {
            _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.StartAction,
                cameraSet.StartActionCamPosF, cameraSet.StartActionCamPosL);
        }
        _battleCommandUI.UndoMagicUIText();
        BattleOperatingInstructionsUI.Instance.HiddenUI();
        turnManager.TurnCharacterCommandUI(turnManager.CurrentTurnCharacter, false);
        ResetCommandFlag();
        onCommandInputComplete?.Invoke();
        isConfirmCommand = false; //コマンドを確定していない状態にする
    }

    /// <summary>
    /// 入力したコマンドに応じて、UIの表示を変更する
    /// </summary>
    private void CurrentCommandInput()
    {
        if (isMagicUISelected) //魔法を入力
        {
            //選択した魔法がなかったら処理を行わない
            if(CurrentMagic == null) return;
            
            //選択した魔法コマンドをBattleCanvasに表示する
            _battleCommandUI.SetMagicUIText(CurrentMagic);
            isConfirmCommand = true;
        }
        else if (isItemUISelected)
        {
            
        }
    }

    /// <summary>
    /// 現在のターンのキャラのCommandUIを取得する
    /// </summary>
    /// <param name="commandUI">キャラのCommandUI</param>
    public void GetCommandUI(CommandUI commandUI)
    {
        currentCharacterCommandUI = commandUI;
    }

    /// <summary>
    /// 魔法、攻撃、アイテムの選択
    /// </summary>
    private void CommandSelected()
    {
        //まだ、何も選択していない状態かの判定
        var isInput = (!isMagicUISelected && !isAttackUISelected && !isItemUISelected);
        var battle = _battleCameraAngleManager;
        var cameraSet = turnManager.CharacterInfos[turnManager.CurrentTurnCharacter].cameraSettings;

        if (commandInput.Player.Magic.triggered && isInput)
        {
            turnManager.CharacterCommandStateChanged(CommandState.Magic);
            currentCharacterCommandUI.ShowCommandUI(CommandState.Magic);
            isMagicUISelected = true;
            battle.BattleCameraAngleChange(BattleCameraActiveType.ActionConfirmed, 
                cameraSet.ActionConfirmedCamPosF, cameraSet.ActionConfirmedCamPosL);
            BattleOperatingInstructionsUI.Instance.CommandSelectionUI();
            
            Debug.Log("魔法");
        }
        else if (commandInput.Player.Attack.triggered && isInput)
        {
            turnManager.CharacterCommandStateChanged(CommandState.Attack);
            currentCharacterCommandUI.ShowCommandUI(CommandState.Attack);
            isAttackUISelected = true;
            battle.BattleCameraAngleChange(BattleCameraActiveType.CommandConfirmed,
                cameraSet.CommandConfirmedCamPosF, cameraSet.CommandConfirmedCamPosL);
            BattleOperatingInstructionsUI.Instance.CommandConfirmedUI();
            //通常攻撃は選ぶ内容がないため、確定状態にする
            isConfirmCommand = true;
            Debug.Log("攻撃");
        }
        else if (commandInput.Player.Item.triggered && isInput)
        {
            turnManager.CharacterCommandStateChanged(CommandState.Item);
            currentCharacterCommandUI.ShowCommandUI(CommandState.Item);
            isItemUISelected = true;
            battle.BattleCameraAngleChange(BattleCameraActiveType.ActionConfirmed, 
                cameraSet.ActionConfirmedCamPosF, cameraSet.ActionConfirmedCamPosL);
            BattleOperatingInstructionsUI.Instance.CommandSelectionUI();
            Debug.Log("アイテム");
        }
        
        //選択している状態だったら、”戻る”を入力出来るようにする
        if (commandInput.Player.Back.triggered && !isInput)
        {
            //コマンド確定画面から戻る場合
            if (isConfirmCommand)
            {
                _battleCommandUI.UndoMagicUIText();
                isConfirmCommand = false;
                CurrentMagic = null; //魔法コマンドを選択してない状態にする
                battle.BattleCameraAngleChange(BattleCameraActiveType.ActionConfirmed, 
                    cameraSet.ActionConfirmedCamPosF, cameraSet.ActionConfirmedCamPosL);
                BattleOperatingInstructionsUI.Instance.CommandSelectionUI();
                
                return;
            }
            
            ResetCommandFlag();
            currentCharacterCommandUI.CommandUIChangeHidden(true);
            currentCharacterCommandUI.MagicUIHidden();
            //TODO：この下に「攻撃」「アイテム」のUIを非表示にする
            
            //コマンドをNoneに変更
            turnManager.CharacterCommandStateChanged(CommandState.None);
            //カメラアングルを元に戻す
            battle.BattleCameraAngleChange(BattleCameraActiveType.ActionConfirmed, 
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
            BattleOperatingInstructionsUI.Instance.HiddenUI();
            
            Debug.Log("戻る");
        }
    }

    /// <summary>
    /// コマンド選択フラグをリセットする
    /// </summary>
    private void ResetCommandFlag()
    {
        isMagicUISelected = false;
        isAttackUISelected = false;
        isItemUISelected = false;
    }

    /// <summary>
    /// 選択したコマンドをリセットする
    /// </summary>
    public void ResetCurrentCommand()
    {
        CurrentMagic = null;
        CurrentItem = null;
    }

    /// <summary>
    /// 魔法コマンドの入力
    /// </summary>
    private void MagicCommandInput()
    {
        //魔法が選択されていない、確定コマンドUIが表示されている状態は処理をしない
        if (!isMagicUISelected || isConfirmCommand) return;
        
        var battle =  _battleCameraAngleManager;
        var cameraSet = turnManager.CharacterInfos[turnManager.CurrentTurnCharacter].cameraSettings;
        //魔法を選択
        if (commandInput.Player.MagicCommand0.triggered)
        {
            SetMagicBaseData(0);
            battle.BattleCameraAngleChange(BattleCameraActiveType.CommandConfirmed,
                cameraSet.CommandConfirmedCamPosF, cameraSet.CommandConfirmedCamPosL);
            BattleOperatingInstructionsUI.Instance.CommandConfirmedUI();
        }
        else if (commandInput.Player.MagicCommand1.triggered)
        {
            SetMagicBaseData(1);
            battle.BattleCameraAngleChange(BattleCameraActiveType.CommandConfirmed,
                cameraSet.CommandConfirmedCamPosF, cameraSet.CommandConfirmedCamPosL);
            BattleOperatingInstructionsUI.Instance.CommandConfirmedUI();
        }
        else if (commandInput.Player.MagicCommand2.triggered)
        {
            SetMagicBaseData(2);
            battle.BattleCameraAngleChange(BattleCameraActiveType.CommandConfirmed,
                cameraSet.CommandConfirmedCamPosF, cameraSet.CommandConfirmedCamPosL);
            BattleOperatingInstructionsUI.Instance.CommandConfirmedUI();
        }
        
        //魔法コマンドの左右を変更する
        if (commandInput.Player.MagicChange.triggered)
        {
            currentCharacterCommandUI.ChangeCommandUI(CommandState.Magic);
        }
    }

    /// <summary>
    /// 魔法コマンドを設定する
    /// </summary>
    /// <param name="index">設定する魔法のインデックス</param>
    private void SetMagicBaseData(int index)
    {
        //indexが範囲外にアクセスしないようにする
        if (index > currentCharacterCommandUI.EachMagicLeft.Count
            && index > currentCharacterCommandUI.EachMagicRight.Count
            || index < 0) return;

        if (currentCharacterCommandUI.IsCurrentSelectedMagic) //左側が表示されている状態
        {
            var magic = currentCharacterCommandUI.EachMagicLeft[index];
            CurrentMagic = magic;
            CurrentCommandInput();
        }
        else //右側が表示されている状態
        {
            var magic = currentCharacterCommandUI.EachMagicRight[index];
            CurrentMagic = magic;
            CurrentCommandInput();
        }
    }

    /// <summary>
    /// 魔法が単体の場合、ターゲットの選択を行う
    /// </summary>
    private void TargetSelection()
    {
        //TODO：なんかしらの入力でターゲットを選択
        //TODO：ターゲットの決定
    }

    /// <summary>
    /// 防御アクションの入力
    /// </summary>
    private void DefenseActionInput()
    {
        //防御アクションの状態ではない時は処理を行わない
        foreach (var chara in turnManager.CharacterInfos)
        {
            if(chara.Value.characterName == "Enemy") continue;
            var state = chara.Value.state.characterState;
            if(state != CharacterStateType.BeforeAttack) return;
        }
        
        //入力有効時間のタイマーを更新
        foreach (var status in GetPlayerCharacterBaseStatus())
        {
            status.UpdateDefenseActionTimer(Time.deltaTime);
        }
        
        //TODO：ここはいずれinputSystemでバインドしたもの使用する
        //TODO：パリィ出来ていることが確認できたOK
        //ここは仮の入力を指定している
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (var status in GetPlayerCharacterBaseStatus())
            {
                if(status.IsInputDefenseAction()) continue;
                status.ParryInput(0.5f);
                Debug.Log("パリィ開始");
            }
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (var status in GetPlayerCharacterBaseStatus())
            {
                if(status.IsInputDefenseAction()) continue;
                status.JustGuardInput(0.5f);
            }
        }
    }

    /// <summary>
    /// プレイヤー操作キャラのCharacterBaseStatusを取得
    /// ・IEnumerableで返すためforeachで使える
    /// </summary>
    /// <returns>プレイヤーのみのCharacterBaseStatus</returns>
    private IEnumerable<CharacterBaseStatus> GetPlayerCharacterBaseStatus()
    {
        //Enemyを除いた
        return turnManager.CharacterInfos
            .Where(kv => kv.Value.characterName != "Enemy")
            .Select(kv => kv.Value.status.GetCharacterStatus());
    }
}
