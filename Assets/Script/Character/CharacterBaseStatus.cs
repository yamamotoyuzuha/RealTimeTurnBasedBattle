using System;
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
    
    private readonly CharacterEventsSystem _eventsSystem;
        
    // バフ、デバフ前のステータスを保持しておく
    private float originalHp;
    private int originalMp;
    private float originalAttack;
    private float originalDefense;
    private int originalSpeed;
    
    //キャラクター
    private GameObject charaObject;
    public GameObject CharaObject => charaObject;
    
    /// <summary>
    /// 初期ステータスを設定する
    /// </summary>
    /// <param name="eventsSystem">CharacterEventsSystem</param>>
    /// <param name="hp">CharacterBaseData：HP</param>
    /// <param name="mp">CharacterBaseData：MP</param>
    /// <param name="attack">CharacterBaseData：Attack</param>
    /// <param name="defense">CharacterBaseData：Defense</param>
    /// <param name="speed">CharacterBaseData：Speed</param>
    /// <param name="obj">キャラクター本体</param>>
    /// <param name="special">必殺技ゲージ量</param>>
    public CharacterBaseStatus(CharacterEventsSystem eventsSystem, float hp, int mp, float attack, float defense, int speed, GameObject obj)
    {
        _eventsSystem = eventsSystem;
        
        MaxHp = hp;
        Hp = hp;
        MaxMp = mp;
        Mp = mp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
        charaObject = obj;
        OriginalStatusSet();
    }

    /// <summary>
    /// 元のステータスを設定
    /// </summary>
    private void OriginalStatusSet()
    {
        originalHp = Hp;
        originalMp = Mp;
        originalAttack = Attack;
        originalDefense = Defense;
        originalSpeed = Speed;
    }

    /// <summary>
    /// キャラクターのHPを回復する
    /// </summary>
    /// <param name="heal">回復量</param>
    public void Heal(float heal)
    {
        //最大HPを上回らないようにする
        Hp = Math.Min(Hp + heal, MaxHp);
        _eventsSystem.onHpChanged?.Invoke(this, Hp, MaxHp);
    }

    public void Damage(float value)
    {
        Hp = Mathf.Max(Hp - value, 0);
    }

    /// <summary>
    /// MPを増やす
    /// </summary>
    /// <param name="addMp">回復するMP</param>
    public void AddMp(int addMp)
    {
        var beforeMp = Mp;
        Mp = Math.Min(Mp + addMp, MaxMp);
        _eventsSystem.onMpAdd?.Invoke(this, Mp, beforeMp);
    }

    /// <summary>
    /// MPを減らす
    /// </summary>
    /// <param name="reduceMp">減らすMP</param>
    public void ReduceMp(int reduceMp)
    {
        var beforeMp = Mp;
        Mp = Math.Max(Mp - reduceMp, 0);
        _eventsSystem.onMpReduce?.Invoke(this, Mp, beforeMp);
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
    
    /// <summary>
    /// キャラクターの攻撃力を元に戻す
    /// </summary>
    public void UndoAttackStatus()
    {
        Attack = originalAttack;
    }
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
