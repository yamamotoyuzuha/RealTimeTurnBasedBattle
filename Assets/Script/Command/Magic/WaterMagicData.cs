using UnityEngine;

[CreateAssetMenu(fileName = "WaterMagicData", menuName = "ScriptableObject/Magic/WaterMagicData")]
public class WaterMagicData : MagicBaseData
{
    [Header("水被り状態の継続ターン")] 
    [SerializeField] private int splashedWaterTurn;
    public int SplashedWaterTurn => splashedWaterTurn;
}
