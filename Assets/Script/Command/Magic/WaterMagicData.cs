using UnityEngine;

[CreateAssetMenu(fileName = "WaterMagicData", menuName = "ScriptableObject/Magic/WaterMagicData")]
public class WaterMagicData : MagicBaseData
{
    [Header("吸水率（○○％）")]
    [SerializeField] private float _waRate;
    [Header("吸水状態の継続ターン")] 
    [SerializeField] private int _waterAbsorptionTurn;

    public override void MagicAction(CharacterBaseStatus status)
    {
        var waterEffect = new WaterEffect(_waRate, _waterAbsorptionTurn, SaType);
        status.StatusEffectInfliction(waterEffect, SaType, StatusEffect, _waterAbsorptionTurn);
    }
}
