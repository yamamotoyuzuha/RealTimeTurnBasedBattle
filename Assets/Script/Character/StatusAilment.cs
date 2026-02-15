using UnityEngine;

/// <summary>
/// 魔法による状態異常の効果
/// ・効果付与
/// ・魔法効果
/// ・効果終了
/// </summary>
public abstract class StatusAilment
{
    /// <summary>
    /// 効果持続ターン
    /// </summary>
    public int Sustainability {get; protected set;}

    /// <summary>
    /// 効果付与
    /// </summary>
    /// <param name="status">効果を受けるキャラクター</param>>
    public abstract void EffectGrant(CharacterBaseStatus status);
    /// <summary>
    /// 効果発動
    /// </summary>
    /// <param name="status">この効果を受けるキャラクター</param>
    public abstract void EffectActivation(CharacterBaseStatus status);
    /// <summary>
    /// 効果終了
    /// </summary>
    public abstract void EffectEnd(CharacterBaseStatus status);
    
    /// <summary>
    /// 持続ターンが終了したかのフラグ
    /// true：終了した　false：終了していない
    /// </summary>
    public bool IsEnd {get; protected set;}

    /// <summary>
    /// 状態異常の種類
    /// </summary>
    public StatusAbnormalityType StatusAbnormalityType {get; protected set;}
    
    public float WaterAbsorption {get; protected set;}
}

/// <summary>
/// 炎魔法の効果による状態異常
///・火傷による延焼ダメージを与える
/// </summary>
public class FlameEffect : StatusAilment
{
    private int flameDamage; //持続ダメージ
    private Sprite imageIcon;
    
    /// <summary>
    /// 効果の設定を行う
    /// </summary>
    /// <param name="damage">持続ダメージ</param>
    /// <param name="image">持続ダメージのアイコン</param>>
    /// <param name="sustainability">持続ターン</param>
    /// <param name="type">状態異常の種類</param>>
    public FlameEffect(int damage, Sprite image, int sustainability, StatusAbnormalityType type)
    {
        flameDamage = damage;
        imageIcon = image;
        Sustainability = sustainability;
        StatusAbnormalityType = type;
    }
    
    public override void EffectGrant(CharacterBaseStatus status)
    {
        Debug.Log("火傷状態になった");
    }

    public override void EffectActivation(CharacterBaseStatus status)
    {
        Debug.Log("火傷ダメージを与える前" + status.Hp);
        //持続ターンが０になったら効果終了
        status.Damage(flameDamage, imageIcon);
        Sustainability--;
        Debug.Log("火傷ダメージを与える" + status.Hp);
        
        IsEnd = Sustainability <= 0;
    }

    public override void EffectEnd(CharacterBaseStatus status)
    {
        Debug.Log("火傷状態が終了");
    }
}
/// <summary>
/// 氷魔法の効果による状態異常
/// ・被ダメージが○○%アップする
/// </summary>
public class IceEffect : StatusAilment
{
    private float damageIncrease; //倍率
    
    /// <summary>
    /// 効果の設定
    /// </summary>
    /// <param name="increase">ダメージ増加％</param>
    /// <param name="sustainability">継続ターン</param>
    /// <param name="type">状態異常の種類</param>
    public IceEffect(float increase, int sustainability, StatusAbnormalityType type)
    {
        damageIncrease = increase;
        Sustainability = sustainability;
        StatusAbnormalityType = type;
    }
    
    public override void EffectGrant(CharacterBaseStatus status)
    {
        Debug.Log("氷結状態によって受けるダメージが上昇");
        status.DamageTakenCalculation.AddRate(damageIncrease);
    }

    public override void EffectActivation(CharacterBaseStatus status)
    {
        Sustainability--;
        IsEnd = Sustainability <= 0;
    }

    public override void EffectEnd(CharacterBaseStatus status)
    {
        //効果が切れるとともに被ダメージの倍率をリセット
        status.DamageTakenCalculation.ResetRate();
    }
}

/// <summary>
/// 雷魔法の効果による状態異常
///・痺れ状態によるターンのスキップ（行動不能）
/// </summary>
public class ThunderEffect : StatusAilment
{
    public ThunderEffect(int sustainability, StatusAbnormalityType type)
    {
        Sustainability = sustainability;
        StatusAbnormalityType = type;
    }
    
    public override void EffectGrant(CharacterBaseStatus status)
    {
        Debug.Log("痺れ状態によって次のターンがスキップされる");
    }

    public override void EffectActivation(CharacterBaseStatus status)
    {
        Sustainability--;
        IsEnd = Sustainability <= 0;
    }

    public override void EffectEnd(CharacterBaseStatus status)
    {
        Debug.Log("痺れ状態終了");
    }
}

/// <summary>
/// 水魔法の効果による状態異常
/// ・吸水（与えたダメージの一部が与えたプレイヤーのHP回復量になる）
/// </summary>
public class WaterEffect : StatusAilment
{
    public WaterEffect(float rate, int sustainability, StatusAbnormalityType type)
    {
        WaterAbsorption = rate / 100f; //0.?の形にするため
        Sustainability = sustainability;
        StatusAbnormalityType = type;
    }
    
    public override void EffectGrant(CharacterBaseStatus status)
    {
        Debug.Log("吸水状態によって、ダメージの一部でHPが回復");
    }

    public override void EffectActivation(CharacterBaseStatus status)
    {
        Sustainability--;
        IsEnd = Sustainability <= 0;
    }

    public override void EffectEnd(CharacterBaseStatus status)
    {
        Debug.Log("吸水状態終了");
    }
}


