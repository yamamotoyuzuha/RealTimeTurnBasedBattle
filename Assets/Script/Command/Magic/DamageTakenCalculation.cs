/// <summary>
/// 魔法の効果などによる被ダメージ増減の計算
/// ・CharacterBaseStatusで保持しておく
/// </summary>
public class DamageTakenCalculation
{
    /// <summary>
    /// 被ダメージの倍率
    /// </summary>
    public float DamageRate { get; private set; } = 1f;

    /// <summary>
    /// 被ダメージ倍率を増加
    /// </summary>
    /// <param name="rate">倍率　○○％</param>>
    public void AddRate(float rate)
    {
        DamageRate *= 1f + rate / 100f;
    }

    /// <summary>
    /// 被ダメージの倍率を元に戻す
    /// </summary>
    public void ResetRate()
    {
        DamageRate = 1f;
    }
}
