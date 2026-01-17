using UnityEngine;

[CreateAssetMenu(fileName = "ThunderMagicData", menuName = "ScriptableObject/Magic/ThunderMagicData")]
public class ThunderMagicData : MagicBaseData
{
    [Header("感電の継続ターン")] 
    [SerializeField] private int electricShockTurn;
    public int ElectricShockTurn => electricShockTurn;
}
