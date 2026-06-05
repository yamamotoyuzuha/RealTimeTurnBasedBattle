using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 必殺技：攻撃系
/// </summary>
[CreateAssetMenu(fileName = "UltimateDamage", menuName = "ScriptableObject/ULT/UltimateDamage")]
public class UltimateDamage : UltimateBaseData
{
    [Header("与えるダメージ量")]
    [SerializeField] private float _takeDamage;
    
    public override void PlayCutIn()
    {
        
    }

    public override async UniTask Execute(CharacterCombatSystem combatSystem, CharacterBaseStatus baseStatus)
    {
        var damage = _takeDamage * baseStatus.Attack;
        await combatSystem.TakeDamageAsync(damage);
    }

    public override void End()
    {
        
    }
}
