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
    public int SpecialMove { get; private set; }
        
    // バフ、デバフ前のステータスを保持しておく
    private float originalHp;
    private int originalMp;
    private float originalAttack;
    private float originalDefense;
    private int originalSpeed;
    
    //キャラクター
    private GameObject charaObject;
    public GameObject CharaObject => charaObject;

    #region Enemyのみが使用するもの
    

    /// <summary>
    /// 最大の体幹ゲージ量
    /// </summary>
    private float _maxCoreGauge;
    /// <summary>
    /// 現在の体幹ゲージ量
    /// </summary>
    private float _currentCoreGauge;
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
    /// <param name="core">体幹ゲージ量</param>>
    /// <param name="special">必殺技ゲージ量</param>>
    public CharacterBaseStatus(float hp, int mp, float attack, float defense, int speed, GameObject obj, float core, int special)
    {
        MaxHp = hp;
        Hp = hp;
        MaxMp = mp;
        Mp = mp;
        Attack = attack;
        Defense = defense;
        Speed = speed;
        charaObject = obj;
        _maxCoreGauge = core;
        _currentCoreGauge = core;
        SpecialMove = special;
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
        //onHpChanged?.Invoke(this, Hp, MaxHp);
    }

    public void Dama(float value)
    {
        Hp = Mathf.Max(Hp - value, 0);
    }

    /*
    /// <summary>
    /// キャラクターにダメージを与え、HPを減らす
    /// </summary>
    /// <param name="damage">ダメージ計算済みの値</param>
    /// <param name="image">ダメージアイコン</param>>
    public void Damage(float damage, Sprite image)
    {
        var finalDamage = damage * DamageTakenCalculation.DamageRate;
        var rollUpDamage = Mathf.Round(finalDamage);
        Hp = Mathf.Max(Hp - rollUpDamage, 0);
        onHpChanged?.Invoke(this, Hp, MaxHp);
        onHitEffect?.Invoke();
        //ダメージUIを表示
        CharacterDamageUI.Instance.DamageUIShowDisplay(charaObject.transform, rollUpDamage, image).Forget();
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
        var rollUpDamage = Mathf.Round(finalDamage);
        //0を下回らないようにする
        Hp = Math.Max(Hp - rollUpDamage, 0);
        onHpChanged?.Invoke(this, Hp, MaxHp);
        onHitEffect?.Invoke();
        //ダメージUIを表示
        await CharacterDamageUI.Instance.DamageUIShowDisplay(charaObject.transform, rollUpDamage, null);
        DeathDetermination();
    }
    */

    /// <summary>
    /// MPを増やす
    /// </summary>
    /// <param name="addMp">回復するMP</param>
    public void AddMp(int addMp)
    {
        var beforeMp = Mp;
        Mp = Math.Min(Mp + addMp, MaxMp);
        //onMpAdd?.Invoke(this, Mp, beforeMp);
    }

    /// <summary>
    /// MPを減らす
    /// </summary>
    /// <param name="reduceMp">減らすMP</param>
    public void ReduceMp(int reduceMp)
    {
        var beforeMp = Mp;
        Mp = Math.Max(Mp - reduceMp, 0);
        //onMpReduce?.Invoke(this, Mp, beforeMp);
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

    #region 必殺技

    /// <summary>
    /// 必殺技の発動
    /// </summary>
    public void SpecialMoveActivated()
    {
        SpecialMove = 0;
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
