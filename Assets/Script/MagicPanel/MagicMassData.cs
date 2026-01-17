using UnityEngine;

[CreateAssetMenu(fileName = "MassData", menuName = "ScriptableObject/MassData")]
public class MagicMassData : ScriptableObject
{
    [Header("マスのタイプ")]
    [SerializeField] private MassType massType;
    [Header("マスの色")]
    [SerializeField] private Color color;

    public MassType MassType => massType;
    public Color Color => color;
    
    [Space(30)]
    [Header("効果マスの場合、効果マスの種類を設定する")]
    [SerializeField] private EffectMassData _effectMassData;
    public EffectMassData EffectMassData => _effectMassData;
}

/// <summary>
/// マスの種類
/// </summary>
public enum MassType
{
    Start,
    Goal,
    None,
    Normal,
    Effect,

    //通れないマス
    NoEntry,
}
