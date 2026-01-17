using UnityEngine;

[CreateAssetMenu(fileName = "IceMagicData", menuName = "ScriptableObject/Magic/IceMagicData")]
public class IceMagicData : MagicBaseData
{
    [Header("凍結の継続ターン")] 
    [SerializeField] private int freezeTurn;
    public int FreezeTurn => freezeTurn;
}
