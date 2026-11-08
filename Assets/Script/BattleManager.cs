using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BattleManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private CommandInputManager _commandInputManager;
    [SerializeField] private PartyStatusUI _partyStatusUI;
    [SerializeField] private EnemyStatusUI _enemyStatusUI;
    [SerializeField] private BattleCommandUI _battleCommandUI;
    [SerializeField] private BattleCameraAngleManager _battleCameraAngleManager;
    [SerializeField] private BehaviorDisplayUI _behaviorDisplayUI;
    [SerializeField] private VictoryOrDefeatUI _victoryOrDefeatUI;
    [SerializeField] private FieldSettings _fieldSettings;
    [Header("戦闘関連")]
    [SerializeField] private BattleDamage _battleDamage;
    [SerializeField] private BattleCharacterSetUp _battleCharacterSetUp;

    #region 戦闘関連のクラス

    private BattleCharacter _battleCharacter;
    private BattleAction _battleAction;
    private BattleResult _battleResult;
    private BattleCommandExecutor _battleCommandExecutor;
    private BattleUltimateManager _battleUltimateManager;

    #endregion
    // 敵関連
    private EnemyBase _registeredEnemy;
    

    void Awake()
    {
        InitializeBattleSettings();
        InitializeBattleAction();
        
        //コマンドの選択が完了したときのイベントを登録
        _commandInputManager.onCommandInputComplete += CharacterStartAction;
        //Enemyのターンが開始したときのイベントを登録
        _turnManager.onEnemyTurnStart += CharacterStartAction;
        
        InitializeBattle();
    }

    private void Start()
    {
        //UIの表示
        //TODO：これだと直ぐ表示してしまっているため、数秒待ってからバトルを開始するのと同時に表示を行う
        _partyStatusUI.onPartyStatusDisplay?.Invoke(true);
        _enemyStatusUI.onEnemyStatusDisplay?.Invoke(true);
        if(_enemyStatusUI.onEnemyStatusDisplay == null) Debug.Log("null");
    }

    private void Update() //TODO：デバック用に追加
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            _turnManager.onUpdateTurnOrder?.Invoke();
            Debug.LogWarning("ターン順の再計算を行った");
        }
    }

    /// <summary>
    /// 戦闘の初期化
    /// </summary>
    private void InitializeBattle()
    {
        _battleCharacterSetUp.BattleInitialize();
    }
    
    /// <summary>
    /// 戦闘関連クラスのインスタンス化と設定
    /// </summary>
    private void InitializeBattleSettings()
    {
        _battleCharacter = new BattleCharacter();
        _battleAction = new BattleAction();
        _battleResult = new BattleResult(_battleCharacter);
        _battleCommandExecutor = new BattleCommandExecutor(_turnManager, _commandInputManager,
            _battleCameraAngleManager, _fieldSettings, _battleCharacter);
        _battleUltimateManager = new BattleUltimateManager(_turnManager, _battleCharacter);
        
        _battleDamage.GetBattleSystem(_battleCharacter);
        _battleCharacterSetUp.GetBattleSystem(_battleCharacter, _battleUltimateManager);
    }
    
    /// <summary>
    /// コマンドアクションを登録する
    /// </summary>
    private void InitializeBattleAction()
    {
        _battleAction.RegisterCommand(CommandState.Magic, _battleCommandExecutor.ExecuteMagic);
        _battleAction.RegisterCommand(CommandState.Attack, _battleCommandExecutor.ExecuteAttack);
    }

    //TODO：これはエンカウント用のため使わないから削除する
    /// <summary>
    /// キャラクターの行動順を計算する
    /// </summary>
    /// <param name="player">プレイヤー</param>>
    /// <param name="buddyMon">バディモン</param>>
    /// <param name="enemy">敵</param>>
    public void ActionOrderCalculation(GameObject player, GameObject buddyMon, GameObject enemy)
    {
        /*
        //戦闘するキャラクターを格納
        List<GameObject> fieldCharacter = new List<GameObject>();
        fieldCharacter.Add(player);
        fieldCharacter.Add(buddyMon);
        fieldCharacter.Add(enemy);

        //速度順にソートをする
        //fieldCharacter = fieldCharacter.OrderByDescending(i => i.GetComponent<Status>().GetSpeed()).ToList();
        
        //TurnManagerにデータを渡す
        var index = fieldCharacter.Count - 1;
        var lastTurnCharacter = fieldCharacter[index];
        turnManager.GetBattleManagerData(fieldCharacter, lastTurnCharacter);
        */
    }

    /// <summary>
    /// 現在のターンのキャラクターがプレイヤーが操作可能キャラクターかを判定する
    /// </summary>
    /// <returns>true：プレイヤーの操作キャラクター　false：Enemy</returns>
    private bool CurrentCharacterPlayerJudgment()
    {
        //現在のターンのキャラクターを取得する
        var character = _turnManager.CurrentTurnCharacter;

        //キャラクターが操作キャラクターなのかを判定する
        if (_turnManager.PlayableCharacterJudgment(character)) return true;

        return false;
    }

    /// <summary>
    /// 行動開始
    /// </summary>
    private async void CharacterStartAction()
    {
        _turnManager.SetTurnStartingFlag(true);
        _commandInputManager.SetCommandSelected(false);
        Debug.Log("行動開始");
        
        // 現在行動するキャラクターを取得
        var characterObject = _turnManager.CurrentTurnCharacter;
        var character = _battleCharacter.GetCharacter(characterObject);
        
        // キャラクターの行動
        // キャラクターがプレイヤーの操作キャラクターの場合、マジックパネルを表示する
        if (CurrentCharacterPlayerJudgment())
        {
            // 登録されているコマンド処理を実行
            await _battleAction.ExecuteCommand(character);

            // 行動終了
            CharacterEndAction();
        }
        else //敵の行動
        {
            // TODO：ここ、マジックナンバーになっているから修正する
            // 少し待機してから、敵の行動を開始
            await UniTask.Delay(TimeSpan.FromSeconds(1));

            var enemy = characterObject.GetComponent<EnemyBase>();
            if(enemy == null) return;

            character.EnemyOnlySystem.ResetResultDefenseActionType();
            RegisterEnemyEvents(enemy, character);
            enemy.OnEnemyTurnAction?.Invoke();
        }
    }

    /// <summary>
    /// 行動終了
    /// </summary>
    private void CharacterEndAction()
    {
        if(IsCheckBattleEndCondition()) return;

        _turnManager.SetTurnStartingFlag(false);
        _commandInputManager.SetCommandSelected(true);

        Debug.Log("行動終了");

        _turnManager.onNextTurnSetUp?.Invoke();

        // ターン終了時の状態異常などによる死亡を再判定
        IsCheckBattleEndCondition();
    }

    /// <summary>
    /// 戦闘終了判定と終了処理を行う
    /// </summary>
    /// <returns>true：戦闘終了　false：戦闘中</returns>
    private bool IsCheckBattleEndCondition()
    {
        // 戦闘結果を取得
        var result = _battleResult.CheckBattleResult();
        if (result == ResultType.None) return false;
        
        Time.timeScale = 0;
        BattleUIHidden();

        switch (result)
        {
            case ResultType.Victory:
                _victoryOrDefeatUI.OnVictoryUIDisplay?.Invoke();
                Debug.Log("Enemyの死亡");
                break;

            case ResultType.Defeat:
                _victoryOrDefeatUI.OnDefeatUIDisplay?.Invoke();
                Debug.Log("プレイヤーキャラクターが全員死亡");
                break;
        }

        return true;
    }

    /// <summary>
    /// 敵のイベントを登録する
    /// ・イベント登録を行う敵を引数に指定する
    /// </summary>
    private void RegisterEnemyEvents(EnemyBase enemyBase, Character enemyCharacter)
    {
        // 前回登録したイベントを解除
        if (_registeredEnemy != null)
        {
            _registeredEnemy.OnEnemyTurnEnd -= CharacterEndAction;
            _registeredEnemy.OnDefenseAction -= DefenseActionAdditionalAction;
            _registeredEnemy.OnEnemyAction -= _behaviorDisplayUI.SetActionUI;
            _registeredEnemy.UnsubscribeActioAttackDamage(() => EnemyAttackDamageCalculation(enemyBase, enemyCharacter));
        }

        _registeredEnemy = enemyBase;
        _registeredEnemy.OnEnemyTurnEnd += CharacterEndAction;
        // 敵の攻撃ダメージ処理
        _registeredEnemy.RegisterActionAttackDamage(() => EnemyAttackDamageCalculation(enemyBase, enemyCharacter));
        // 防御成功時の追加行動
        _registeredEnemy.OnDefenseAction += DefenseActionAdditionalAction;
        // 敵の行動表示
        _registeredEnemy.OnEnemyAction += _behaviorDisplayUI.SetActionUI;
    }
    
    /// <summary>
    /// 戦闘に関係のあるUIの非表示を行う
    /// </summary>
    private void BattleUIHidden()
    {
        _turnManager.onTurnIconDisplay?.Invoke(false);
        _partyStatusUI.onPartyStatusDisplay?.Invoke(false);
        _enemyStatusUI.onEnemyStatusDisplay?.Invoke(false);
        _enemyStatusUI.onStatusAbnormalityDisplay?.Invoke(false);
        _battleCommandUI.OnConfirmedCommandUIDisplay?.Invoke(false);
        _behaviorDisplayUI.OnActionUIDisplay?.Invoke("", false);
    }

    /// <summary>
    /// EnemyのCharacterを取得する
    /// </summary>
    /// <returns>EnemyのCharacter</returns>
    private Character GetEnemyCharacter()
    {
        return _battleCharacter.GetEnemyCharacters().FirstOrDefault();
    }
    
    /// <summary>
    /// Enemyがプレイヤーに与えるダメージの計算
    /// </summary>
    /// <param name="enemyBase">行動するEnemy</param>
    /// <param name="enemyStatus">行動するEnemyのStatus</param>
    private void EnemyAttackDamageCalculation(EnemyBase enemyBase, Character enemyStatus)
    {
        _battleDamage.EnemyAttackDamageCalculation(enemyBase, enemyStatus);
    }
    
    /// <summary>
    /// 防御アクションが成功した時に行う追加行動処理
    /// </summary>
    /// <param name="action">成功した防御アクション</param>
    private async UniTask DefenseActionAdditionalAction(DefenseActionType action)
    {
        switch (action)
        {
            case DefenseActionType.Parry:
                Debug.Log("プレイヤーキャラクターの一斉攻撃");
                var text = "AllAttack";
                _behaviorDisplayUI.OnActionUIDisplay?.Invoke(text, true);
                await AllCharacterAttack();
                _behaviorDisplayUI.OnActionUIDisplay?.Invoke("", false);
                break;
            case DefenseActionType.JustGuard:
                Debug.Log("カウンターアクション！");
                
                break;
        }
    }
    
    /// <summary>
    /// プレイヤーキャラクターの一斉攻撃処理
    /// </summary>
    private async UniTask AllCharacterAttack()
    {
        // TODO：アニメーション処理を入れる（全体攻撃用のやつ）
        // TODO：アニメーション再生インターフェイスをリストで持っておいて再生みたいな感じにする
        
        var enemyStatus = GetEnemyCharacter();
        var status = new List<CharacterBaseStatus>();
        // 生存しているプレイヤーを取得
        foreach (var character in _battleCharacter.GetPlayerCharacters())
        {
            status.Add(character.BaseStatus);
        }

        if (enemyStatus == null || status.Count == 0) return;

        // カメラアングルの切り替え
        _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.DAction,
            _battleCameraAngleManager.DActionPosF,
            _battleCameraAngleManager.DActionPosL);

        await UniTask.Delay(TimeSpan.FromSeconds(2));

        // ダメージリストを作成
        List<float> damages = new List<float>();
        foreach (var playerStatus in status)
        {
            damages.Add(playerStatus.Attack);
        }

        // ダメージUIに反映
        await enemyStatus.CombatSystem.DamageUIAsync(damages);
    }
}
