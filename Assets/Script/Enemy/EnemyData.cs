using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/EnemyData")]
public class EnemyData : CharacterBaseData
{
    [Header("攻撃タイプ")]
    [SerializeField] private EnemyType _enemyType;
    public EnemyType EnemyAttackType => _enemyType;
    [Header("攻撃のパターン（通常）")] 
    [SerializeField] private CharacterCommandActionData[] _enemyAttackBaseDataN;
    public CharacterCommandActionData[] EnemyAttackNormal => _enemyAttackBaseDataN;
    [Header("攻撃のパターン（怒り）")]
    [SerializeField] private CharacterCommandActionData[] _enemyAttackBaseDataA;
    public CharacterCommandActionData[] EnemyAttackAnger => _enemyAttackBaseDataA;
    
    [Header("種族")]
    [SerializeField] private EnemyRace _enemyRace;
    public EnemyRace EnemyRaceType => _enemyRace;
    
    [Header("弱点属性")]
    [SerializeField] private CharacterAttributesType[] _weaknessType;
    public CharacterAttributesType[] WeaknessTypes => _weaknessType;

    [FormerlySerializedAs("_trunk")]
    [Header("体幹（ゲージの量）")]
    [SerializeField] private float _core;
    /// <summary>
    /// 体幹
    /// </summary>
    public float Core => _core;
}

/// <summary>
/// 攻撃のタイプ
/// </summary>
public enum EnemyType
{
    
}

/// <summary>
/// 種族
/// </summary>
public enum EnemyRace
{
    
}


