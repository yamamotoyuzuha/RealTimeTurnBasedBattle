using System;
using UnityEngine;

/// <summary>
/// 魔法反応
/// ・例えば、水＋雷で感電とか
/// </summary>
public abstract class MagicReaction
{
    /// <summary>
    /// 魔法反応を発動
    /// </summary>
    /// <param name="status">魔法反応を受けるキャラクター</param>>
    public abstract void MagicReactionAction(CharacterBaseStatus status);
}

/// <summary>
/// 蒸発反応
/// </summary>
public class Evaporation : MagicReaction
{
    private float damage;
    private Sprite imageIcon;
    public Evaporation(float damage, Sprite image)
    {
        this.damage = damage;
        imageIcon = image;
    }
    
    public override void MagicReactionAction(CharacterBaseStatus status)
    {
        status.Damage(damage, imageIcon);
    }
}

/// <summary>
/// 溶解反応
/// </summary>
public class Dissolution : MagicReaction
{
    private float damage;
    public Dissolution(float damage, Sprite image)
    {
        this.damage = damage;
    }

    public override void MagicReactionAction(CharacterBaseStatus status)
    {
        
    }
}

/// <summary>
/// 魔法反応の種類
/// </summary>
public enum MagicReactionType
{
    [InspectorName("蒸発（炎＋水）")]
    Evaporation,
    [InspectorName("溶解（炎＋氷）")]
    Dissolution,
    [InspectorName("爆雷（炎＋雷）")]
    DepthCharge,
    [InspectorName("凍結（水＋氷）")]
    Freeze,
    [InspectorName("感電（水＋雷）")]
    ElectricShock,
    [InspectorName("破砕（雷＋氷）")]
    Crushing,
    [InspectorName("何もなし")]
    None
}

/// <summary>
/// 魔法反応の情報
/// </summary>
[Serializable]
public class MagicReactionInfo
{
    [Header("反応する状態異常")]
    [SerializeField] private StatusAbnormalityType _saType;
    [Header("反応した結果の魔法反応")]
    [SerializeField] private MagicReactionType _mrType;
    [Header("魔法反応によるダメージ")]
    [SerializeField] private float _damage;
    
    public StatusAbnormalityType SaType => _saType;
    public MagicReactionType MrType => _mrType;
    public float Damage => _damage;
}
