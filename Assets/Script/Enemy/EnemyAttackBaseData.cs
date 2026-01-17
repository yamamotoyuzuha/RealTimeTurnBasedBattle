using UnityEngine;

public class EnemyAttackBaseData : CharacterCommandActionData
{
    [Header("攻撃の名前")]
    [SerializeField] private string attackName;
    public string AttackName => attackName;
}
