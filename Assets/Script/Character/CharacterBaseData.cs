using UnityEngine;

public class CharacterBaseData : ScriptableObject
{
    [Header("キャラクターの名前")]
    [SerializeField] private string characterName;
    public string CharacterName => characterName;
    [Header("キャラクターのアイコン")]
    [SerializeField] private Sprite characterIconSprite;
    public Sprite CharacterIconSprite => characterIconSprite;
    
    //戦闘
    [Header("HP")]
    [SerializeField] private float hp;
    [Header("MP")]
    [SerializeField] private int mp;
    [Header("攻撃力")]
    [SerializeField] private float attack;
    [Header("防御力")]
    [SerializeField] private float defense;
    [Header("速度")]
    [SerializeField] private int speed;
    
    [Header("魔法")]
    [SerializeField] private MagicBaseData[] magicBaseData;
    public MagicBaseData[] MagicBaseData => magicBaseData;
    
    [Header("属性")]
    [SerializeField] private CharacterAttributesType _attributesType;
    public CharacterAttributesType AttributesType => _attributesType;
    
    public float Hp => hp;
    public int Mp => mp;
    public float Attack => attack;
    public float Defense => defense;
    public int Speed => speed;
    
}
