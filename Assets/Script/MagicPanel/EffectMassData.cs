using UnityEngine;

[CreateAssetMenu(fileName = "EffectMassData/", menuName = "ScriptableObject/EffectMassData")]
public class EffectMassData : ScriptableObject
{
    [Header("バフ倍率")] 
    [SerializeField] private float _buffPercent;
    public float BuffPercent => _buffPercent;
    [Header("デバフ倍率")]
    [SerializeField] private float _debuffPercent;
    public float DebuffPercent => _debuffPercent;
    
    [Header("効果マスの種類")]
    [SerializeField] private EffectMassType _type;
    public EffectMassType Type => _type;

    [Header("効果説明")]
    [SerializeField] private string _explanation;
    public string  Explanation => _explanation;
    
    [Header("属性")]
    [SerializeField] private CharacterAttributesType _attributes;
    public CharacterAttributesType Attributes => _attributes;
}
