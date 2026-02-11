using UnityEngine;

public class MagicBaseData : CharacterCommandActionData
{
    [Header("魔法の名前")] 
    [SerializeField] private string magicName;
    public string MagicName => magicName;
    
    [Header("魔法の説明")] 
    [SerializeField] private string magicExplanation;
    public string MagicExplanation => magicExplanation;

    [Header("消費MP")]
    [SerializeField] private int consumptionMp;
    /// <summary>
    /// 消費MP
    /// </summary>
    public int ConsumptionMp => consumptionMp;
    
    [Header("魔法の属性")]
    [SerializeField] private MagicType magicType;
    public MagicType MagicType => magicType;
    
    [Header("魔法の攻撃範囲")]
    [SerializeField] private MagicRangeType magicRangeType;
    public MagicRangeType MagicRangeType => magicRangeType;

    [Header("魔法パネルの表示時間")] 
    [SerializeField] private float magicPanelDisplayTime;
    public float MagicPanelDisplayTime => magicPanelDisplayTime;

    [Header("アニメーション時間")] 
    [SerializeField] private float _animationTime;
    public float AnimationTime => _animationTime;

    [Header("状態異常アイコン")] 
    [SerializeField] private Sprite _statusEffect;
    public Sprite StatusEffect => _statusEffect;
    [Header("状態異常の種類")]
    [SerializeField] private StatusAbnormalityType _saType;
    public StatusAbnormalityType SaType => _saType;
    [Header("魔法の効果を受ける確率○○％")]
    [SerializeField] private float magicProbability;
    public float MagicProbability => magicProbability;
    
    [Header("可能な防御アクション")]
    [SerializeField] private DefenseActionType[] _defenseActions;
    public DefenseActionType[] DefenseActions => _defenseActions;

    [Header("アニメーション情報")] 
    [SerializeField] private CommandAnimationData _commandAnimationData;
    public CommandAnimationData CommandAnimationData => _commandAnimationData;

    /// <summary>
    /// 継承先の各魔法でオーバーライドして個別処理
    /// </summary>
    /// <param name="status">効果を受けるキャラクター</param>>
    public virtual void MagicAction(CharacterBaseStatus status) { }

    public override CharacterCommandActionType GetCommandType()
    {
        return CharacterCommandActionType.Magic;
    }
    public override MagicBaseData GetMagicBaseData()
    {
        return this;
    }
}
