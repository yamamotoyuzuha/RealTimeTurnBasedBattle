using System.Collections.Generic;
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
    [Header("魔法反応")]
    [SerializeField] private MagicReactionInfo _magicReactionInfo;
    public MagicReactionInfo MagicReactionInfo => _magicReactionInfo;
    [Header("魔法の効果を受ける確率○○％")]
    [SerializeField] private float _magicProbability;
    public float MagicProbability => _magicProbability;
    
    [Header("可能な防御アクション")]
    [SerializeField] private List<DefenseActionType> _defenseActions;
    public List<DefenseActionType> DefenseActions => _defenseActions;

    [Header("アニメーション情報")] 
    [SerializeField] private CommandAnimationData _commandAnimationData;
    public CommandAnimationData CommandAnimationData => _commandAnimationData;
    
    [Header("魔法のエフェクト")]
    [SerializeField] private GameObject _particleObj;
    public GameObject ParticleObj => _particleObj;

    /// <summary>
    /// 継承先の各魔法でオーバーライドして個別処理
    /// </summary>
    /// <param name="status">効果を受けるキャラクター</param>>
    public virtual void MagicAction(CharacterBaseStatus status){}
    /// <summary>
    /// 継承先で可能な防御アクションかの判定を行う
    /// 一致した場合は、可能の判定となる
    /// </summary>
    /// <param name="type">防御アクションの種類</param>
    /// <returns>true：防御アクション可能　false：防御アクション不可能</returns>
    public virtual bool IsDefenceActionPossible(DefenseActionType type){return false;}

    public override CharacterCommandActionType GetCommandType()
    {
        return CharacterCommandActionType.Magic;
    }
    public override MagicBaseData GetMagicBaseData()
    {
        return this;
    }
}
