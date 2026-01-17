using System;
using System.Collections.Generic;
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
    /// <param name="heal"></param>
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
        //0を下回らないようにする
        Hp = Math.Max(Hp - damage, 0);
        onHpChanged?.Invoke(this, Hp, MaxHp);
        
        //ダメージUIを表示
        CharacterDamageUI.Instance.DamageUIShowDisplay(charaObject.transform, damage);
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
    public void StatusEffectInfliction(StatusAilment status)
    {
        //状態異常を開始し、このキャラクターの状態異常を追加
        status.EffectGrant();
        statusAilments.Add(status);
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
            statusAilments[i].EffectActivation(this);
            if (statusAilments[i].IsEnd)
            {
                statusAilments[i].EffectEnd();
                statusAilments.RemoveAt(i);
            }
        }
    }

    #endregion

    #region パリィなどの防御アクション関連
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
    /// 防御アクション入力の判定
    /// </summary>
    public DefenseActionType DefenseActionJudgment()
    {
        if (IsParry)
        {
            Debug.Log("パリィ成功");
            return DefenseActionType.Parry;
        }

        if (IsJustGuard)
        {
            Debug.Log("ジャストガード成功");
            return DefenseActionType.JustGuard;
        }

        return DefenseActionType.None;
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
