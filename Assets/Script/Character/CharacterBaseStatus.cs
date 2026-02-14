using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterBaseStatus
{
    public float MaxHp{ get; private set; }
    public float Hp { get; private set; }
    public int MaxMp{ get; private set; }
    public int Mp { get; private set; }
    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public int Speed { get; private set; }
  
    //TODO：これ引数にCharacterBaseStatusを置いてるけど、要らない気がする
    /// <summary>
    /// HPが変動した時に呼ぶAction
    /// （HPが変動したキャラ、現在のHP、最大HP）
    /// </summary>
    public Action<CharacterBaseStatus, float, float> onHpChanged;
    /// <summary>
    /// MPが増えた時に呼ぶAction
    /// （MPが増えたキャラ、増加後のMP、増減後のMP）
    /// </summary>
    public Action<CharacterBaseStatus, int, int> onMpAdd;
    /// <summary>
    /// MPが減った時に呼ぶAction
    /// （MPが減ったキャラ、増減後のMP、増減前の）
    /// </summary>
    public Action<CharacterBaseStatus, int, int> onMpReduce;
    /// <summary>
    /// 死亡
    /// </summary>
    public Action<GameObject> onDeath;
    /// <summary>
    /// パリィが成功した時に呼ぶAction
    /// （増えるMP量）
    /// </summary>
    public Action<int> onParrySuccess;
    public Action onJustGuardSuccess;
    /// <summary>
    /// プレイヤー操作キャラクターのみ
    /// ステータスUIの表示、非表示を行う
    /// </summary>
    public Action<CharacterBaseStatus, bool> onStatusDisplay;
    /// <summary>
    /// 攻撃を受けた時の演出
    /// </summary>
    public Action onHitEffect;
    /// <summary>
    /// 死亡した時の演出
    /// </summary>
    public Action onDeathEffect;

    #region 状態異常関連のAction
    /// <summary>
    /// 状態異常が発生
    /// </summary>
    public Action<StatusAbnormalityInfo, CharacterBaseStatus> onStatusAbnormalityOccurrence;
    /// <summary>
    /// 状態異常が経過
    /// </summary>
    public Action<CharacterBaseStatus, StatusAbnormalityType> onStatusAbnormalityProgress;
    /// <summary>
    /// 状態異常が終了
    /// </summary>
    public Action<CharacterBaseStatus, StatusAbnormalityType> onStatusAbnormalityEnd;
    #endregion

    /// <summary>
    /// 被ダメージの倍率計算
    /// </summary>
    public DamageTakenCalculation DamageTakenCalculation { get; } = new DamageTakenCalculation();
    /// <summary>
    /// 現在の状態異常
    /// </summary>
    private List<StatusAilment> statusAilments = new List<StatusAilment>();
        
    //バフ、デバフ前のステータスを保持しておく
    private float originalHp;
    private int originalMp;
    private float originalAttack;
    private float originalDefense;
    private int originalSpeed;
    
    //キャラクター
    private GameObject charaObject;

    #region Enemyのみが使用するもの
    /// <summary>
    /// Enemyの行動データを設定する
    /// </summary>
    /// <param name="data">行動するデータ</param>
    public void SetActionData(CharacterCommandActionData data)
    {
        CharacterCommandActionData = data;
    }
    /// <summary>
    /// Enemyの行動データ
    /// </summary>
    public CharacterCommandActionData CharacterCommandActionData {get; private set;}
    /// <summary>
    /// Enemyが受けた防御アクション
    /// </summary>
    public DefenseActionType ResultDefenseActionType {get; private set;}
    /// <summary>
    /// Enemyが受けた防御アクションの設定
    /// </summary>
    /// <param name="defenseActionType">防御アクションの種類</param>
    public void SetResultDefenseActionType(DefenseActionType defenseActionType)
    {
        ResultDefenseActionType = defenseActionType;
    }
    /// <summary>
    /// 受けた防御アクションのリセット
    /// </summary>
    public void ResetResultDefenseActionType()
    {
        ResultDefenseActionType = DefenseActionType.None;
    }
    #endregion

    #region 防御アクション関連
    public bool IsParry { get; private set; }
    public bool IsJustGuard { get; private set; }
    public bool IsJump { get; private set; }
    public bool IsEvasion { get; private set; }
    //各防御アクションのタイマー
    private float parryTimer;
    private float justGuardTimer;
    private float jumpTimer;
    private float evasionTimer;
    #endregion

    /// <summary>
    /// 初期ステータスを設定する
    /// </summary>
    /// <param name="hp">CharacterBaseData：HP</param>
    /// <param name="mp">CharacterBaseData：MP</param>
    /// <param name="attack">CharacterBaseData：Attack</param>
    /// <param name="defense">CharacterBaseData：Defense</param>
    /// <param name="speed">CharacterBaseData：Speed</param>
    /// <param name="obj">キャラクター本体</param>>
    public CharacterBaseStatus(float hp, int mp, float attack, float defense, int speed, GameObject obj)
    {
        MaxHp = hp;
        Hp = hp;
        MaxMp = mp;
        Mp = mp;
        Attack = attack;
        Defense = defense;
        Speed = speed;

        charaObject = obj;
    }

    /// <summary>
    /// キャラクターのHPを回復する
    /// </summary>
    /// <param name="heal">回復量</param>
    public void Heal(float heal)
    {
        //最大HPを上回らないようにする
        Hp = Math.Min(Hp + heal, MaxHp);
        onHpChanged?.Invoke(this, Hp, MaxHp);
    }

    /// <summary>
    /// キャラクターにダメージを与え、HPを減らす
    /// </summary>
    /// <param name="damage">ダメージ計算済みの値</param>
    public void Damage(float damage)
    {
        var finalDamage = damage * DamageTakenCalculation.DamageRate;
        Hp = Mathf.Max(Hp - finalDamage, 0);
        onHpChanged?.Invoke(this, Hp, MaxHp);
        onHitEffect?.Invoke();
        //ダメージUIを表示
        CharacterDamageUI.Instance.DamageUIShowDisplay(charaObject.transform, finalDamage).Forget();
        DeathDetermination();
    }

    /// <summary>
    /// ダメージUIを一斉に表示する
    /// </summary>
    /// <param name="damages">キャラクターごとの攻撃力</param>
    public async UniTask DamageUIAsync(List<float> damages)
    {
        //ダメージUIを一斉に表示するため、リストを作成し追加する
        List<UniTask> tasks = new List<UniTask>();
        foreach (var damage in damages)
        {
            tasks.Add(DamageAsync(damage));
        }
        //全てのダメージUIの表示が終わるまで待機する
        await UniTask.WhenAll(tasks);
    }
    /// <summary>
    /// キャラクターにダメージを与え、HPを減らす
    /// </summary>
    /// <param name="damage">ダメージ</param>
    private async UniTask DamageAsync(float damage)
    {
        var finalDamage = damage * DamageTakenCalculation.DamageRate;
        //0を下回らないようにする
        Hp = Math.Max(Hp - finalDamage, 0);
        onHpChanged?.Invoke(this, Hp, MaxHp);
        onHitEffect?.Invoke();
        //ダメージUIを表示
        await CharacterDamageUI.Instance.DamageUIShowDisplay(charaObject.transform, finalDamage);
        DeathDetermination();
    }
    
    /// <summary>
    /// 死亡判定
    /// </summary>
    private void DeathDetermination()
    {
        if (Hp <= 0)
        {
            onDeathEffect?.Invoke();
            onDeath?.Invoke(charaObject);
        }
    }

    /// <summary>
    /// MPを増やす
    /// </summary>
    /// <param name="addMp">回復するMP</param>
    public void AddMp(int addMp)
    {
        var beforeMp = Mp;
        Mp = Math.Min(Mp + addMp, MaxMp);
        onMpAdd?.Invoke(this, Mp, beforeMp);
    }

    /// <summary>
    /// MPを減らす
    /// </summary>
    /// <param name="reduceMp">減らすMP</param>
    public void ReduceMp(int reduceMp)
    {
        var beforeMp = Mp;
        Mp = Math.Max(Mp - reduceMp, 0);
        onMpReduce?.Invoke(this, Mp, beforeMp);
    }

    /// <summary>
    /// 攻撃力を上昇させる
    /// </summary>
    public void AttackPowerUp(float powerUp)
    {
        Debug.Log("バフ前" + Attack);
        var value = Attack * powerUp;
        Attack = value;
        Debug.Log("バフ後" + Attack);
    }

    /// <summary>
    /// 防御力を上昇させる
    /// </summary>
    public void DefensePowerUp(float powerUp)
    {
        var value =  Defense * powerUp;
        Defense = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public void AttackPowerDown(float powerDown)
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void DefensePowerDown(float powerDown)
    {
        
    }
    
    /// <summary>
    /// キャラクターのバフ、デバフを解除する
    /// </summary>
    public void UndoStatus()
    {
        Attack = originalAttack;
        Defense = originalDefense;
        Speed = originalSpeed;
    }

    #region 魔法の効果による状態異常の管理

    /// <summary>
    /// 状態異常中かの判定
    /// </summary>
    /// <returns>true：状態異常中　false：状態異常ではない</returns>
    public bool IsUnderAbnormalStatus()
    {
        if (statusAilments.Count > 0) return true;
        return false;
    }
    
    /// <summary>
    /// 状態異常開始
    /// 魔法の効果時に呼ぶ
    /// </summary>
    /// <param name="status">魔法の効果</param>>
    /// <param name="type">状態異常の種類</param>>
    /// <param name="icon">状態異常のアイコン</param>>
    /// <param name="duration">状態異常の継続ターン</param>>
    public void StatusEffectInfliction(StatusAilment status, StatusAbnormalityType type, Sprite icon, int duration)
    {
        //状態異常が被っていない場合、状態異常を開始する
        if (!statusAilments.Any(sa  => sa.StatusAbnormalityType == type))
        {
            //状態異常を開始し、このキャラクターの状態異常を追加
            status.EffectGrant(this);
            statusAilments.Add(status);
        }

        //TODO：これifの中に入れていいような気がするんだ
        var info = new StatusAbnormalityInfo(this, type, icon, duration);
        onStatusAbnormalityOccurrence?.Invoke(info, this);
    }

    /// <summary>
    /// 状態異常中の効果を与える
    /// ターン開始時に呼ぶ
    /// </summary>
    public void StatusEffectStart()
    {
        //状態異常がある場合、効果を受ける
        for (int i = statusAilments.Count - 1; i >= 0; i--)
        {
            Debug.Log(statusAilments.Count + "状態異常効果を呼ぶ前");
            statusAilments[i].EffectActivation(this);
            Debug.Log(statusAilments.Count + "状態異常効果を呼んだ後");
            if(statusAilments.Count <= 0) return;
            onStatusAbnormalityProgress?.Invoke(this, statusAilments[i].StatusAbnormalityType);
            if (statusAilments[i].IsEnd)
            {
                statusAilments[i].EffectEnd(this);
                onStatusAbnormalityEnd?.Invoke(this, statusAilments[i].StatusAbnormalityType);
                statusAilments.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 痺れ状態かを判定
    /// </summary>
    /// <returns>true：痺れ状態　false：痺れ状態じゃない</returns>
    public bool IsParalysisStatus()
    {
        return statusAilments.Any(sa => sa.StatusAbnormalityType == StatusAbnormalityType.ElectricShock);
    }
    /// <summary>
    /// 吸水状態かを判定
    /// ・吸水状態だった場合、ダメージを与えてきたキャラクターのHPを回復
    /// </summary>
    /// <param name="damage">ダメージ</param>>
    /// <param name="status">ダメージを与えてきたキャラクター</param>>
    public void IsWaterAbsorption(float damage, CharacterBaseStatus status)
    {
        //水魔法の状態異常がある場合、HPを回復させる
        var water = statusAilments.FirstOrDefault(sa =>
            sa.StatusAbnormalityType == StatusAbnormalityType.Wet);
        if (water != null)
        {
            var heal = damage * water.WaterAbsorption;
            status.Heal(heal);
            Debug.Log("回復");
        }
        else
        {
            Debug.Log("水魔法の状態異常はない");
        }
    }
    
    /// <summary>
    /// 状態異常のリストをクリアする
    /// </summary>
    public void StatusAilmentsClear()
    {
        statusAilments.Clear();
    }
    
    #endregion

    #region パリィなどの防御アクション関連
    //防御アクションの成否フラグ
    public bool ParrySuccess { get;private set; }
    public bool JustGuardSuccess { get; private set; }
    public bool JumpSuccess { get; private set; }
    public bool EvasionSuccess { get; private set; }
    
    /// <summary>
    /// パリィの入力を検知する
    /// </summary>
    /// <param name="inputPeriod">入力を受け付ける時間</param>>
    public void ParryInput(float inputPeriod)
    {
        IsParry = true;
        parryTimer = inputPeriod;
    }
    /// <summary>
    /// ジャストガードの入力を検知する
    /// </summary>
    /// <param name="inputPeriod">入力を受け付ける時間</param>>
    public void JustGuardInput(float inputPeriod)
    {
        IsJustGuard = true;
        justGuardTimer = inputPeriod;
    }
    /// <summary>
    /// ジャンプの入力を検知する
    /// </summary>
    /// <param name="inputPeriod">入力を受け付ける時間</param>
    public void JumpInput(float inputPeriod)
    {
        IsJump = true;
        jumpTimer = inputPeriod;
    }
    /// <summary>
    /// 回避の入力を検知する
    /// </summary>
    /// <param name="inputPeriod">入力を受け付ける時間</param>
    public void EvasionInput(float inputPeriod)
    {
        IsEvasion = true;
        evasionTimer = inputPeriod;
    }

    /// <summary>
    /// 各防御アクションのタイマーを減らす
    /// </summary>
    /// <param name="deltaTime">Time.deltaTimeを渡す</param>>
    public void UpdateDefenseActionTimer(float deltaTime)
    {
        if (IsParry)
        {
            parryTimer -= deltaTime;
            if(parryTimer <= 0) IsParry = false;
        }

        if (IsJustGuard)
        {
            justGuardTimer -= deltaTime;
            if(justGuardTimer <= 0) IsJustGuard = false;
        }
    }

    /// <summary>
    /// なんらかの防御アクションがすでに入力されているか判定を行う
    /// </summary>
    /// <returns>true：入力済み　false：入力未済</returns>
    public bool IsInputDefenseAction()
    {
        return (IsParry || IsJustGuard || IsJump || IsEvasion);
    }
    
    /// <summary>
    /// 防御アクション入力の判定
    /// ・Enemyが攻撃をする際に呼ぶ
    /// </summary>
    public DefenseActionType DefenseActionJudgment()
    {
        if (IsParry)
        {
            Debug.Log("パリィ成功");
            ParrySuccess = true;
            return DefenseActionType.Parry;
        }

        if (IsJustGuard)
        {
            Debug.Log("ジャストガード成功");
            JustGuardSuccess = true;
            return DefenseActionType.JustGuard;
        }

        if (IsJump)
        {
            Debug.Log("ジャンプ成功");
            JumpSuccess = true;
            return DefenseActionType.Jump;
        }

        if (IsEvasion)
        {
            Debug.Log("回避成功");
            EvasionSuccess = true;
            return DefenseActionType.Evasion;
        }

        return DefenseActionType.None;
    }

    /// <summary>
    /// 防御アクションの成否判定をリセットする
    /// </summary>
    public void DefenseActionSuccessReset()
    {
        ParrySuccess = false;
        JustGuardSuccess = false;
        JumpSuccess = false;
        EvasionSuccess = false;
    }
    #endregion

    #region 防御アクション成功処理
    /// <summary>
    /// パリィ成功
    /// </summary>
    /// <param name="mp">増加するMP量</param>>
    public void ParrySuccessProcessing(int mp)
    {
        AddMp(mp);
        onParrySuccess?.Invoke(mp);
    }
    /// <summary>
    /// ジャストガード成功
    /// </summary>
    public void JustGuardProcessing()
    {
        onJustGuardSuccess?.Invoke();
    }

    #endregion
}

/// <summary>
/// 防御アクションの種類
/// </summary>
public enum DefenseActionType
{
    Parry,
    JustGuard,
    Jump,
    Evasion,
    None
}
