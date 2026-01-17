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
    public abstract void EffectGrant();
    /// <summary>
    /// 効果発動
    /// </summary>
    /// <param name="status">この効果を受けるキャラクター</param>
    public abstract void EffectActivation(CharacterBaseStatus status);
    /// <summary>
    /// 効果終了
    /// </summary>
    public abstract void EffectEnd();
    
    /// <summary>
    /// 持続ターンが終了したかのフラグ
    /// true：終了した　false：終了していない
    /// </summary>
    public bool IsEnd {get; protected set;}
}

/// <summary>
/// 炎魔法の効果による状態異常
/// </summary>
public class FlameEffect : StatusAilment
{
    private int flameDamage; //持続ダメージ
    
    /// <summary>
    /// 効果の設定を行う
    /// </summary>
    /// <param name="damage">持続ダメージ</param>
    /// <param name="sustainability">持続ターン</param>
    public FlameEffect(int damage, int sustainability)
    {
        flameDamage = damage;
        Sustainability = sustainability;
    }
    
    public override void EffectGrant()
    {
        Debug.Log("火傷状態になった");
    }

    public override void EffectActivation(CharacterBaseStatus status)
    {
        Debug.Log("火傷ダメージを与える前" + status.Hp);
        //持続ターンが０になったら効果終了
        status.Damage(flameDamage);
        Sustainability--;
        Debug.Log("火傷ダメージを与える" + status.Hp);
        
        IsEnd = Sustainability <= 0;
    }

    public override void EffectEnd()
    {
        Debug.Log("火傷状態が終了");
    }
}
/// <summary>
/// 氷魔法の効果による状態異常
/// </summary>
public class IceEffect : StatusAilment
{
    public override void EffectGrant()
    {
        
    }

    public override void EffectActivation(CharacterBaseStatus status)
    {
        
    }

    public override void EffectEnd()
    {
        
    }
}


