using UnityEngine;

[CreateAssetMenu(fileName = "FlameMagicData", menuName = "ScriptableObject/Magic/FlameMagicData")]
public class FlameMagicData : MagicBaseData
{
    [Header("持続ダメージ")]
    [SerializeField] private int _damage;
    [Header("持続ダメージ継続ターン")] 
    [SerializeField] private int _damageOverTime;

    public override void MagicAction(CharacterStatusEffectSystem status)
    {
        status.IsMagicReactionCheck(this);
        if(!MagicEffectProbability.ProbabilityCalculation(MagicProbability)) return;
        //炎魔法の状態異常を生成し、ターゲットに状態異常を付与する
        var flameEffect = new FlameEffect(_damage, StatusEffect, _damageOverTime, SaType);
        status.StatusEffectInfliction(flameEffect, SaType, StatusEffect, _damageOverTime);
    }

    public override bool IsDefenceActionPossible(DefenseActionType type)
    {
        return DefenseActions.Contains(type);
    }
}
