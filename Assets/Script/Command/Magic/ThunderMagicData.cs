using UnityEngine;

[CreateAssetMenu(fileName = "ThunderMagicData", menuName = "ScriptableObject/Magic/ThunderMagicData")]
public class ThunderMagicData : MagicBaseData
{
    [Header("感電の継続ターン")] 
    [SerializeField] private int _electricShockTurn;

    public override void MagicAction(CharacterBaseStatus status)
    {
        if(!MagicEffectProbability.ProbabilityCalculation(MagicProbability)) return;
        var thunderEffect = new ThunderEffect(_electricShockTurn, SaType);
        status.StatusEffectInfliction(thunderEffect, SaType, StatusEffect, _electricShockTurn);
    }
}
