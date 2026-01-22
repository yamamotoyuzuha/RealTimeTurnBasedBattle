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
    
   　/*TODO：プレイヤーキャラクターとEnemyのUIを追加したが、生成するタイミングをどこかで通知
   　  　　　　できたほうが楽かもしれない
   　  　　　　エンカウントした際に呼ばれるところで、UIも生成してしまおうと考えているがなにかいい方法がないか思考中
   　  　　　　これは"Start()"で生成して、非表示にしておくのがいいかも
    */

    void Awake()
    {
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
    
    //TODO：これはエンカウント用のため使わないから削除する
    /// <summary>
    /// キャラクターの行動順を計算する
    /// </summary>
    /// <param name="player">プレイヤー</param>>
    /// <param name="buddyMon">バディモン</param>>
    /// <param name="enemy">敵</param>>
    public void ActionOrderCalculation(GameObject player, GameObject buddyMon, GameObject enemy)
    {
        //戦闘するキャラクターを格納
        List<GameObject> fieldCharacter = new List<GameObject>();
        fieldCharacter.Add(player);
        fieldCharacter.Add(buddyMon);
        fieldCharacter.Add(enemy);

        //速度順にソートをする
        fieldCharacter = fieldCharacter.OrderByDescending(i => i.GetComponent<Status>().GetSpeed()).ToList();
        
        //TurnManagerにデータを渡す
        var index = fieldCharacter.Count - 1;
        var lastTurnCharacter = fieldCharacter[index];
        turnManager.GetBattleManagerData(fieldCharacter, lastTurnCharacter);
    }

    /// <summary>
    /// Titleの戦闘情報を元に必要な処理を行う
    /// ・戦闘を行うキャラクターの生成
    /// ・速度順にソート
    /// </summary>
    private void SetCombatInformation()
    {
        //戦闘を行うキャラクターを格納する
        List<GameObject> fieldCharacter = new List<GameObject>();
        fieldCharacter.AddRange(CombatInformationCharacterGenerate());
        
        //速度順にソート
        fieldCharacter = fieldCharacter.OrderByDescending(i => i.GetComponent<Status>().GetSpeed()).ToList();
        //TurnManagerにデータを渡す
        var index = fieldCharacter.Count - 1;
        var lastTurnCharacter = fieldCharacter[index];
        turnManager.GetBattleManagerData(fieldCharacter, lastTurnCharacter);
    }
    /// <summary>
    /// 戦闘情報にあるキャラクターを生成する
    /// </summary>
    /// <returns>戦闘を行うキャラクターのリストを返す</returns>
    private List<GameObject> CombatInformationCharacterGenerate()
    {
        //情報の取得
        var infoC = CombatInformationManager.Instance.CombatInfoPlayerCharacter;
        var infoE = CombatInformationManager.Instance.CombatInfoEnemyCharacter;
        
        List<GameObject> infoObj =  new List<GameObject>();
        foreach (var info in infoC)
        {
            var objC = Instantiate(info.CharacterPrefab);
            infoObj.Add(objC);
            
            //パーティーステータスUIにパーティーキャラクターのオブジェクトを渡す
            _partyStatusUI.SetPartyCharacter(objC);
        }
        var objE = Instantiate(infoE.CharacterPrefab);
        infoObj.Add(objE);
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
        //if(turnManager.FriendOrFoe[character].Item1 == "Player") return true;
        if (turnManager.PlayableCharacterJudgment(character)) return true;

        return false;
    }

    /// <summary>
    /// 行動開始
    /// </summary>
    private async void CharacterStartAction()
    {
        commandInputManager.SetCommandSelected(false);
        Debug.Log("行動開始");
        
        //キャラクターの行動
        //キャラクターがプレイヤーの操作キャラクターの場合、マジックパネルを表示する
        if (CurrentCharacterPlayerJudgment())
        {
            Debug.Log("プレイヤーの操作です");
            
            //ターンキャラクターを取得
            var character = turnManager.CurrentTurnCharacter;
            
            //コマンドを取得し、キャラクターが何のコマンドを選択したのか判定する
            var command = turnManager.CharacterInfos[character].command.GetCommand();
            Debug.Log(command);
            switch (command)
            {
                case CommandState.Magic: 
                    await MagicAction(character); //魔法行動が終わるまで待機
                    CharacterEndAction();
                    break;
                
                case CommandState.Attack:
                    AttackAction();
                    CharacterEndAction();
                    break;
                
                case CommandState.Item:
                    ItemAction();
                    CharacterEndAction();
                    break;
            }
        }
        else //敵の行動
        {
            Debug.Log("敵の行動です");
            
            var enemy = turnManager.CharacterInfos[turnManager.CurrentTurnCharacter].enemyBase;
            var enemyAttack = turnManager.CharacterInfos[turnManager.CurrentTurnCharacter].status;
            //登録を行う前に登録解除し、Actionを登録する
            enemy.OnEnemyTurnEnd -= CharacterEndAction;
            enemy.UnsubscribeActioAttackDamage(() => EnemyAttackDamageCalculation(enemy, enemyAttack));
            enemy.OnEnemyTurnEnd += CharacterEndAction;
            enemy.RegisterActionAttackDamage(() => EnemyAttackDamageCalculation(enemy, enemyAttack));
            
            enemy.OnEnemyTurnAction?.Invoke();
        }
    }

    /// <summary>
    /// 行動終了
    /// </summary>
    private void CharacterEndAction()
    {
        commandInputManager.SetCommandSelected(true);
        Debug.Log("行動終了");
        
        turnManager.onNextTurnSetUp?.Invoke();
    }

    /// <summary>
    /// 魔法攻撃の処理
    /// </summary>
    private async UniTask MagicAction(GameObject character)
    {
        //MPを減らす
        var status = turnManager.CharacterInfos[character].status.GetCharacterStatus();
        status.ReduceMp(commandInputManager.CurrentMagic.ConsumptionMp);
        
        //キャラクターの魔法パネルを取得し、表示する
        var magicPanel = character.GetComponent<MagicPanel>();
        magicPanel.MagicPanelToggle();
        
        //魔法パネルの表示待機時間を取得し、待機する
        var magic = commandInputManager.CurrentMagic;
        var disPlayTime = magic.MagicPanelDisplayTime;
        await UniTask.Delay(TimeSpan.FromSeconds(disPlayTime));
        
        //表示時間以内に魔法パネルをクリア出来ていなかったら非表示にする
        if (!magicPanel.IsMagicPanelClear) magicPanel.MagicPanelToggle();
        
        Debug.Log("攻撃を行う");
        
        //TODO:この状態だと、ダメージと魔法効果がバラバラになっているため一個の方が簡潔でいいかも
        //TODO:魔法効果を行うところでダメージも与えたい方がきれいかな
        //キャラの攻撃力を取得して、ダメージを与える
        var enemy = turnManager.Enemy;
        var damage = status.Attack;
        var enemyStatus = enemy.GetCharacterStatus();
        enemyStatus.Damage(damage);
        Debug.Log("攻撃を行った");
        //魔法特有の効果を相手に付与する
        magic.GetMagicBaseData().MagicAction(enemyStatus);
        
        //TODO：上のダメージを与える処理だけど、出来ればアニメーションに合わせたようにしたいEnemyみたいに
        //TODO：まあ、時間があればの話だけど
        
        //仮、アニメーションの時間分待機する
        await UniTask.Delay(TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    private void AttackAction()
    {
        
    }

    /// <summary>
    /// アイテムの処理
    /// </summary>
    private void ItemAction()
    {
        
    }
    
    
    //ここからがEnemyの処理
    /// <summary>
    /// Enemyがプレイヤーに与えるダメージの計算
    /// <param name="enemyBase">行動をするEnemy</param>>
    /// <param name="enemyStatus">行動するEnemyの攻撃力</param>>
    /// </summary>
    private void EnemyAttackDamageCalculation(EnemyBase enemyBase, Status enemyStatus)
    {
        //TODO：ここでEnemyの攻撃が単体攻撃か判定を行う
        if (enemyBase.IsIndividualOrWhole())
        {
            //プレイヤーキャラクターだけを残して、ステータスを取得する（これだと敵が複数いる場合、敵にもダメージをあたえてしまう）
            var players = turnManager.CharacterInfos
                .Where(kv => kv.Key != turnManager.CurrentTurnCharacter)
                .Select(kv => kv.Value);
            var targetList = players.ToList();
            var random = UnityEngine.Random.Range(0, targetList.Count);
            AttackDamageTarget(targetList[random].status, enemyStatus);
        }
        else
        {
            //フィールドにいるプレイヤーキャラクターにダメージを与える
            var status = turnManager.CharacterInfos;
            foreach (var charaStatus in status)
            {
                //Enemyの場合、処理を飛ばす
                if(charaStatus.Key == turnManager.CurrentTurnCharacter) continue;
                AttackDamageTarget(charaStatus.Value.status, enemyStatus);
            }
        }
    }

    /// <summary>
    /// ターゲットにダメージを与える
    /// <param name="targetStatus">ターゲットのステータス</param>
    /// <param name="enemyStatus">Enemyのステータス</param>>
    /// </summary>
    private void AttackDamageTarget(Status targetStatus, Status enemyStatus)
    {
        //ターゲットが防御アクションを行っているのか判定
        var dAction = targetStatus.GetCharacterStatus().DefenseActionJudgment();
        switch (dAction)
        {
            case DefenseActionType.Parry:
                Debug.Log("パリィではじかれた");
                targetStatus.GetCharacterStatus().ParrySuccessProcessing(1);
                return;
            case DefenseActionType.JustGuard:
                Debug.Log("ジャストガードで受け流された");
                targetStatus.GetCharacterStatus().JustGuardProcessing();
                return;
        }
        
        
        //攻撃力を取得し、ターゲットにダメージを与える
        var enemyAttack = enemyStatus.GetCharacterStatus().Attack;
        targetStatus.GetCharacterStatus().Damage(enemyAttack);
        
        //TODO：ここでまた分岐が必要かも
        //TODO：Enemyの攻撃がすべて、魔法とは限らないから
        var magic = enemyStatus.GetCharacterStatus().CharacterCommandActionData.GetMagicBaseData();
        magic.MagicAction(targetStatus.GetCharacterStatus());
        
        Debug.Log("Enemyがプレイヤーにダメージを与える");
    }
}
