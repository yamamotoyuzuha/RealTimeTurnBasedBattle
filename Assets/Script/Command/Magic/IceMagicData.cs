using UnityEngine;

[CreateAssetMenu(fileName = "IceMagicData", menuName = "ScriptableObject/Magic/IceMagicData")]
public class IceMagicData : MagicBaseData
{
    [Header("被ダメージ倍率（○○％形式）")] 
    [SerializeField] private float _damageIncrease;
    [Header("被ダメージ増加の継続ターン")] 
    [SerializeField] private int freezeTurn;

    public override void MagicAction(CharacterBaseStatus status)
    {
        //氷魔法の状態異常を付与する
        var iceEffect = new IceEffect(_damageIncrease, freezeTurn, SaType);
        status.StatusEffectInfliction(iceEffect, SaType, StatusEffect, freezeTurn);
    }
}
