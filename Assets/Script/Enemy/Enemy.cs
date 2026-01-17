using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class Enemy : MonoBehaviour, Status, EnemyBase
{
    [Header("データ")] 
    [SerializeField] private EnemyData enemyData;
    public CharacterBaseData GetData()
    {
        return enemyData;
    }
    /// <summary>
    /// キャラクターのステータス
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
        return enemyData.AttributesType;
    }
    public Action OnEnemyTurnAction{ get; private set; }
    public Action OnEnemyTurnEnd { get; set; }
    public Action OnEnemyAttackDamage { get; private set; }
    public void RegisterActionAttackDamage(Action action)
    {
        OnEnemyAttackDamage += action;
    }
    public void UnsubscribeActioAttackDamage(Action action)
    {
        OnEnemyAttackDamage -= action;
    }

    /// <summary>
    /// 現在のステートを保持
    /// </summary>
    private CharacterStateType characterState;
    /// <summary>
    /// 現在の状態による行動種類
    /// </summary>
    private EnemyBehaviorChangeState behaviorChangeState;

    //攻撃パターン
    private int currentAttackPatten;

    void Awake()
    {
        //キャラクターの情報を取得
        characterState = GetComponent<CharacterState>().characterState;
        characterBaseStatus = new CharacterBaseStatus
            (enemyData.Hp, 0, enemyData.Attack, enemyData.Defense, enemyData.Speed, this.gameObject);
    }

    void Start()
    {
        //Enemyの状態による行動変化（通常）
        behaviorChangeState = EnemyBehaviorChangeState.Normal;
        //攻撃パターンを設定
        currentAttackPatten = 0;
        
        //Enemyのターン開始に行うイベントを登録
        OnEnemyTurnAction += EnemyStatusChangeAction;
    }

    /// <summary>
    /// 状態に応じて、行動を変化させる
    /// </summary>
    private async void EnemyStatusChangeAction()
    {
        //TODO；Enemy用の新しいステートを用意して、モードを切り替えることが出来るようにするとか？

        switch (behaviorChangeState)
        {
            case EnemyBehaviorChangeState.Normal:
                ActionSelected(enemyData.EnemyAttackNormal);
                await BehavioralChoice();
                break;
            case EnemyBehaviorChangeState.Anger:
                ActionSelected(enemyData.EnemyAttackAnger);
                await BehavioralChoice();
                break;
        }
    }

    /// <summary>
    /// 攻撃パターンに応じた選択をする
    /// <param name="actionData">攻撃パターン</param>>
    /// </summary>
    private void ActionSelected(CharacterCommandActionData[] actionData)
    {
        //配列外だった場合、一番最初に戻す
        if (currentAttackPatten < actionData.Length)
        {
            currentAttackPatten = 0;
        }
        
        //現在の攻撃パターンと一致した攻撃を行う
        for (int i = 0; i < actionData.Length; i++)
        {
            if (currentAttackPatten == i)
            {
                //CurrentActionData = actionData[i];
                characterBaseStatus.SetActionData(actionData[i]);
                currentAttackPatten++;
                break;
            }
        }
    }

    /// <summary>
    /// 行動の実行
    /// </summary>
    private async UniTask BehavioralChoice()
    {
        //アニメーションを再生し、アニメーション時間分待機する
        //TODO：アニメーションの再生
        //var magicData = CurrentActionData.GetMagicBaseData();
        var magicData = characterBaseStatus.CharacterCommandActionData.GetMagicBaseData();
        var animTime = magicData.AnimationTime;
        await UniTask.Delay(TimeSpan.FromSeconds(animTime));
        
        //TODO：ここでは仮に待機を使ってデバッグを行う
        //TODO：ダメージを与えることが確認できたOK
        Debug.Log("１回目の攻撃を開始");
        EnemyAttack();
        await UniTask.Delay(TimeSpan.FromSeconds(animTime));
        Debug.Log("２回目の攻撃を開始");
        EnemyAttack();
        Debug.Log("攻撃を終了");
        
        
        //アニメーションが終了した後にターンを進める
        OnEnemyTurnEnd?.Invoke();
    }
    
    public bool IsIndividualOrWhole()
    {
        var data = characterBaseStatus.CharacterCommandActionData;
        switch (data.GetCommandType())
        {
            case CharacterCommandActionType.None:
                Debug.Log("None");
                break;
            case CharacterCommandActionType.Magic:
                Debug.Log("魔法");
                var magicData = data.GetMagicBaseData();
                Debug.Log(magicData);
                return magicData.MagicRangeType == MagicRangeType.Standalone;
        }

        return true;
    }

    /// <summary>
    /// アニメーションイベント
    /// ・ダメージを与える
    /// </summary>
    public void EnemyAttack()
    {
        OnEnemyAttackDamage?.Invoke();
    }
}
