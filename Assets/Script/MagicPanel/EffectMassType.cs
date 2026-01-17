using UnityEngine;

/// <summary>
/// 効果マスの種類
/// </summary>
public enum EffectMassType
{
    [InspectorName("属性攻撃アップ")]
    Attribute,
    [InspectorName("攻撃力ダウン")]
    AttackPowerDown,
    [InspectorName("防御力ダウン")]
    DefensePowerDown,
    [InspectorName("攻撃力アップ")]
    AttackPowerUp,
    [InspectorName("防御力アップ")]
    DefensePowerUp,
    [InspectorName("会心率")]
    CriticalRate,
    [InspectorName("会心ダメージ")]
    CriticalDamage
}
