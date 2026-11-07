using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BattleManager : MonoBehaviour
{
    [Header("参照")]
    [Header("TurnManager")]
    [SerializeField] private TurnManager turnManager;
    [Header("CommandInputManager")]
    [SerializeField] private CommandInputManager commandInputManager;
    [Header("PartyStatusUI")]
    [SerializeField] private PartyStatusUI _partyStatusUI;
    [Header("EnemyStatusUI")]
    [SerializeField] private EnemyStatusUI _enemyStatusUI;
    [Header("BattleCommandUI")]
    [SerializeField] private BattleCommandUI _battleCommandUI;
    [Header("BattleCameraAngleManager")]
    [SerializeField] private BattleCameraAngleManager _battleCameraAngleManager;
    [Header("BehaviorDisplayUI")]
    [SerializeField] private BehaviorDisplayUI _behaviorDisplayUI;
    [Header("VictoryOrDefeatUI")]
    [SerializeField] private VictoryOrDefeatUI _victoryOrDefeatUI;
    [Header("FieldSettings")]
    [SerializeField] private FieldSettings _fieldSettings;
    [SerializeField] private BattleDamage _battleDamage;
    
    // 戦闘関連のクラス
    private BattleCharacter _battleCharacter;
    private BattleAction _battleAction;
    private BattleResult _battleResult;

    void Awake()
    {
        InitializeBattleSettings();
        InitializeBattleAction();
        
        //コマンドの選択が完了したときのイベントを登録
        commandInputManager.onCommandInputComplete += CharacterStartAction;
        //Enemyのターンが開始したときのイベントを登録
        turnManager.onEnemyTurnStart += CharacterStartAction;
        SetCombatInformation();
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
            turnManager.onUpdateTurnOrder?.Invoke();
            Debug.LogWarning("ターン順の再計算を行った");
        }
    }
    
    private void InitializeBattleSettings()
    {
        _battleCharacter = new BattleCharacter();
        _battleAction = new BattleAction();
        _battleResult = new BattleResult(_battleCharacter);
        _battleDamage.GetBattleCharacter(_battleCharacter);
    }
    
    /// <summary>
    /// コマンドアクションを登録する
    /// </summary>
    private void InitializeBattleAction()
    {
        _battleAction.RegisterCommand(CommandState.Magic, ExecuteMagic);
        _battleAction.RegisterCommand(CommandState.Attack, ExecuteAttack);
        _battleAction.RegisterCommand(CommandState.Item, ExecuteItem);
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
    /// Titleの戦闘情報を元に必要な処理を行う
    /// ・戦闘を行うキャラクターの生成
    /// ・速度順にソート
    /// </summary>
    private void SetCombatInformation()
    {
        // 戦闘キャラクターを生成
        _battleCharacter.ClearCharacters();
        // 戦闘を行うキャラクターを格納する
        List<GameObject> fieldCharacter = new List<GameObject>();
        fieldCharacter.AddRange(CombatInformationCharacterGenerate());

        // 速度順にソート
        fieldCharacter = fieldCharacter.OrderByDescending(i =>
                i.GetComponent<Status>().GetCharacter().BaseStatus.Speed).ToList();

        // TurnManagerにデータを渡す
        var maxIndex = fieldCharacter.Count - 1;
        var lastTurnCharacter = fieldCharacter[maxIndex];
        turnManager.GetBattleManagerData(fieldCharacter, lastTurnCharacter);

        // 必殺技の登録
        foreach (var character in _battleCharacter.GetPlayerCharacters())
        {
            character.EventsSystem.onUltimateActivated += ExecuteUltimate;
        }
    }
    
    /// <summary>
    /// 戦闘情報にあるキャラクターを生成する
    /// </summary>
    /// <returns>戦闘を行うキャラクターのリストを返す</returns>
    private List<GameObject> CombatInformationCharacterGenerate()
    {
        // 戦闘を行うプレイヤー、エネミーを取得する
        var infoP = CombatInformationManager.Instance.CombatInfoPlayerCharacter;
        var infoE = CombatInformationManager.Instance.CombatInfoEnemyCharacter;

        List<GameObject> infoObj = new List<GameObject>();
        foreach (var info in infoP)
        {
            var objP = Instantiate(info.CharacterPrefab);
            infoObj.Add(objP);

            // BattleCharacterに登録
            var character = objP.GetComponent<Status>().GetCharacter();
            _battleCharacter.RegisterCharacter(character);

            // パーティーステータスUIに登録
            _partyStatusUI.SetPartyCharacter(objP);
        }

        var objE = Instantiate(infoE.CharacterPrefab);
        infoObj.Add(objE);

        // Enemyも登録
        var enemyCharacter = objE.GetComponent<Status>().GetCharacter();
        _battleCharacter.RegisterCharacter(enemyCharacter);

        return infoObj;
    }

    /// <summary>
    /// 現在のターンのキャラクターがプレイヤーが操作可能キャラクターかを判定する
    /// </summary>
    /// <returns>true：プレイヤーの操作キャラクター　false：Enemy</returns>
    private bool CurrentCharacterPlayerJudgment()
    {
        //現在のターンのキャラクターを取得する
        var character = turnManager.CurrentTurnCharacter;

        //キャラクターが操作キャラクターなのかを判定する
        if (turnManager.PlayableCharacterJudgment(character)) return true;

        return false;
    }

    /// <summary>
    /// 行動開始
    /// </summary>
    private async void CharacterStartAction()
    {
        turnManager.SetTurnStartingFlag(true);
        commandInputManager.SetCommandSelected(false);
        Debug.Log("行動開始");
        
        //キャラクターの行動
        //キャラクターがプレイヤーの操作キャラクターの場合、マジックパネルを表示する
        if (CurrentCharacterPlayerJudgment())
        {
            // 現在行動するキャラクターを取得
            var characterObject = turnManager.CurrentTurnCharacter;
            var character = turnManager.Characters[characterObject];

            // 登録されているコマンド処理を実行
            await _battleAction.ExecuteCommand(character);

            // 行動終了
            CharacterEndAction();
        }
        else //敵の行動
        {
            //少し待機してから、敵の行動を開始させる
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            var enemy = turnManager.CurrentTurnCharacter.GetComponent<EnemyBase>();
            var enemyChara = turnManager.Characters[turnManager.CurrentTurnCharacter];
            enemyChara.EnemyOnlySystem.ResetResultDefenseActionType();
            //登録を行う前に登録解除し、Actionを登録する
            if(enemy == null) return;
            enemy.OnEnemyTurnEnd -= CharacterEndAction;
            enemy.UnsubscribeActioAttackDamage(() => EnemyAttackDamageCalculation(enemy, enemyChara));
            enemy.OnEnemyTurnEnd += CharacterEndAction;
            enemy.RegisterActionAttackDamage(() => EnemyAttackDamageCalculation(enemy, enemyChara));
            enemy.OnDefenseAction -= DefenseActionAdditionalAction;
            enemy.OnDefenseAction += DefenseActionAdditionalAction;
            enemy.OnEnemyAction -= _behaviorDisplayUI.SetActionUI;
            enemy.OnEnemyAction += _behaviorDisplayUI.SetActionUI;
            
            enemy.OnEnemyTurnAction?.Invoke();
        }
    }

    /// <summary>
    /// 行動終了
    /// </summary>
    private void CharacterEndAction()
    {
        // 戦闘結果を取得
        var result = _battleResult.CheckBattleResult();
        if (result != ResultType.None)
        {
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

            return;
        }

        turnManager.SetTurnStartingFlag(false);
        commandInputManager.SetCommandSelected(true);

        Debug.Log("行動終了");

        turnManager.onNextTurnSetUp?.Invoke();

        // ターン終了時の状態異常などによる死亡を再判定
        IsDeathCheck();
    }
    
    /// <summary>
    /// 魔法コマンドを実行
    /// </summary>
    /// <param name="character">コマンドを行うキャラクター</param>
    /// <returns>魔法コマンド</returns>
    private UniTask ExecuteMagic(Character character)
    {
        return MagicAction(character.BaseStatus.CharaObject);
    }

    /// <summary>
    /// 通常攻撃コマンドを実行
    /// </summary>
    /// <param name="character">コマンドを行うキャラクター</param>
    /// <returns>通常攻撃コマンド</returns>
    private UniTask ExecuteAttack(Character character)
    {
        return AttackAction(character.BaseStatus.CharaObject);
    }

    private UniTask ExecuteItem(Character character)
    {
        ItemAction();
        return UniTask.CompletedTask;
    }

    /// <summary>
    /// 死亡したかの判定を行う
    /// </summary>
    private void IsDeathCheck()
    {
        // 戦闘結果を取得
        var result = _battleResult.CheckBattleResult();
        if (result == ResultType.None) return;

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
    }
    
    /// <summary>
    /// 戦闘に関係のあるUIの非表示を行う
    /// </summary>
    private void BattleUIHidden()
    {
        turnManager.onTurnIconDisplay?.Invoke(false);
        _partyStatusUI.onPartyStatusDisplay?.Invoke(false);
        _enemyStatusUI.onEnemyStatusDisplay?.Invoke(false);
        _enemyStatusUI.onStatusAbnormalityDisplay?.Invoke(false);
        _battleCommandUI.OnConfirmedCommandUIDisplay?.Invoke(false);
        _behaviorDisplayUI.OnActionUIDisplay?.Invoke("", false);
    }
    
    /// <summary>
    /// 魔法攻撃の処理
    /// </summary>
    private async UniTask MagicAction(GameObject character)
    {
        //MPを減らす
        var status = turnManager.Characters[character].BaseStatus;
        status.ReduceMp(commandInputManager.CurrentMagic.ConsumptionMp);
        
        //キャラクターの魔法パネルを取得し、表示する
        var magicPanel = character.GetComponent<MagicPanel>();
        magicPanel.MagicPanelToggle();
        
        //魔法パネルの表示待機時間を取得
        var magic = commandInputManager.CurrentMagic;
        var disPlayTime = magic.MagicPanelDisplayTime;
        //魔法パネルの表示時間経過かクリアされるのどちらか早い方を待つ
        await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(disPlayTime)),
            magicPanel.MagicPanelCompleted());
        //表示時間以内に魔法パネルをクリア出来ていなかったら非表示にする
        if (!magicPanel.IsMagicPanelClear) magicPanel.MagicPanelToggle();
        
        //カメラアングルの切り替え
        var cameraSet = turnManager.Characters[character].CameraSettings;
        _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.StartAction,
            cameraSet.StartActionCamPosF, cameraSet.StartActionCamPosL);
        
        //アニメーション関連 TODO：アニメーション周り
        //var animChara = turnManager.CharacterInfos[character].animationCharacter;
        //animChara.SetAnimationPlay(magic.CommandAnimationData._animationTriggerName);
        await UniTask.Delay(TimeSpan.FromSeconds(magic.AnimationTime));
        //エフェクトを生成
        var effect = Instantiate(magic.ParticleObj, _fieldSettings.EnemyCharaPos.position, Quaternion.identity);
        Destroy(effect, 1);
        //キャラの攻撃力を取得して、ダメージを与える
        var damage = status.Attack;
        var enemyStatus = GetEnemyCharacter();
        enemyStatus.CombatSystem.TakeDamage(damage, magic.StatusEffect);
        enemyStatus.StatusEffectSystem.IsWaterAbsorption(damage, status);
        //魔法パネルでバフがかかっていたのを元に戻す
        status.UndoAttackStatus();
        //魔法特有の効果を相手に付与する
        magic.GetMagicBaseData().MagicAction(enemyStatus.StatusEffectSystem);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    private async UniTask AttackAction(GameObject character)
    {
        //通常攻撃のため、MPを増やす
        var playerStatus = turnManager.Characters[character].BaseStatus;
        playerStatus.AddMp(1);
        
        //カメラアングルの切り替え
        var cameraSet = turnManager.Characters[character].CameraSettings;
        _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.StartAction,
            cameraSet.StartActionCamPosF, cameraSet.StartActionCamPosL);
        var enemyStatus = GetEnemyCharacter();
        //アニメーション関連 TODO：ここも
        //var animChara = turnManager.CharacterInfos[character].animationCharacter;
        //animChara.SetAnimationPlay("Attack");
        await UniTask.Delay(TimeSpan.FromSeconds(2)); //アニメーション時間分待機
        enemyStatus.CombatSystem.TakeDamage(playerStatus.Attack, null);
        enemyStatus.StatusEffectSystem.IsWaterAbsorption(playerStatus.Attack, playerStatus);
        
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// アイテムの処理
    /// </summary>
    private void ItemAction()
    {
        
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

    /// <summary>
    /// 必殺技を実行する
    /// </summary>
    /// <param name="ult">実行する必殺技</param>
    /// <param name="baseStatus">必殺技を実行するキャラクターの<c>CharacterBaseStatus</c>></param>
    private async UniTask ExecuteUltimate(UltimateBaseData ult, CharacterBaseStatus baseStatus)
    {
        // 現在のターンキャラクターが行動を開始している場合は、必殺技を発動せず終了後に発動
        if (turnManager.IsTurnStarting)
        {
            turnManager.SetInterruptCharacterUlt(async () =>
            {
                turnManager.SetTimingCalled(true);
                turnManager.TurnCharacterCommandUI(baseStatus.CharaObject, false);
                turnManager.onChangeTurnCharacter?.Invoke(baseStatus.CharaObject);
                await UniTask.Delay(TimeSpan.FromSeconds(2)); // TODO：仮演出であり、2秒待つ
                await ult.Execute(GetEnemyCharacter().CombatSystem, baseStatus);
                if (turnManager.onNextTurnSetUp != null)
                {
                    await turnManager.onNextTurnSetUp.Invoke();
                }
                turnManager.SetTimingCalled(false);
            });
        }
        else // 即発動
        {
            turnManager.SetTimingCalled(false);
            turnManager.TurnCharacterCommandUI(baseStatus.CharaObject, false);
            turnManager.onChangeTurnCharacter?.Invoke(baseStatus.CharaObject);
            await UniTask.Delay(TimeSpan.FromSeconds(2)); // TODO：仮演出であり、2秒待つ
            await ult.Execute(GetEnemyCharacter().CombatSystem, baseStatus);
            if (turnManager.onNextTurnSetUp != null)
            {
                await turnManager.onNextTurnSetUp.Invoke();
            }
        }
    }
}
