using UnityEngine;

[CreateAssetMenu(fileName = "WaterMagicData", menuName = "ScriptableObject/Magic/WaterMagicData")]
public class WaterMagicData : MagicBaseData
{
    [Header("吸水率（○○％）")]
    [SerializeField] private float _waRate;
    [Header("吸水状態の継続ターン")] 
    [SerializeField] private int _waterAbsorptionTurn;

    public override void MagicAction(CharacterStatusEffectSystem status)
    {
        status.IsMagicReactionCheck(this);
        if(!MagicEffectProbability.ProbabilityCalculation(MagicProbability)) return;
        var waterEffect = new WaterEffect(_waRate, _waterAbsorptionTurn, SaType);
        status.StatusEffectInfliction(waterEffect, SaType, StatusEffect, _waterAbsorptionTurn);
    }
    
    public override bool IsDefenceActionPossible(DefenseActionType type)
    {
        return DefenseActions.Contains(type);
    }
}
