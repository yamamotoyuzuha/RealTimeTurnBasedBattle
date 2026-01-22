using UnityEngine;

/// <summary>
/// 全てのマスデータの管理
/// </summary>
[CreateAssetMenu(fileName = "MassData", menuName = "ScriptableObject/AllMagicMassData")]
public class AllMagicMassDatas : ScriptableObject
{
    [Header("魔法パネルのスタートマス")]
    [SerializeField] private MagicMassData _startMassData;
    [Header("魔法パネルのゴールマス")]
    [SerializeField] private MagicMassData _goalMassData;
    [Header("魔法パネルの非表示のマス")]
    [SerializeField] private MagicMassData _noneMassData;
    [Header("魔法パネルの普通マス")]
    [SerializeField] private MagicMassData _normalMassData;
    [Header("魔法パネルの効果マス")]
    [SerializeField] private MagicMassData _effectMassData;
    [Header("魔法パネルの通行禁止マス")]
    [SerializeField] private MagicMassData _noEntryMassData;
    
    public MagicMassData StartMassData => _startMassData;
    public MagicMassData GoalMassData => _goalMassData;
    public MagicMassData NoneMassData => _noneMassData;
    public MagicMassData NormalMassData => _normalMassData;
    public MagicMassData EffectMassData => _effectMassData;
    public MagicMassData NoEntryMassData => _noEntryMassData;
}
