using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// キャラクターの効果を管理する
/// バフ、デバフ、状態異常の付与・継続・終了を行う
/// </summary>
public class CharacterStatusEffectSystem
{
    private Character _character;
    private CharacterBaseStatus _baseStatus;
    private CharacterCombatSystem _combatSystem;
    private CharacterEventsSystem _eventsSystem;
    
    /// <summary>
    /// 現在の状態異常を保持
    /// </summary>
    public List<StatusAilment> StatusAilments { get; private set; }
    
    public CharacterStatusEffectSystem(Character character, CharacterBaseStatus baseStatus, CharacterCombatSystem combatSystem, CharacterEventsSystem eventsSystem)
    {
        StatusAilments = new List<StatusAilment>();
        this._character = character;
        this._baseStatus = baseStatus;
        this._combatSystem = combatSystem;
        this._eventsSystem = eventsSystem;
    }
    
    /// <summary>
    /// 状態異常中かの判定
    /// </summary>
    /// <returns>true：状態異常中　false：状態異常ではない</returns>
    public bool IsUnderAbnormalStatus()
    {
        if (StatusAilments.Count > 0) return true;
        return false;
    }
    
    /// <summary>
    /// 状態異常開始
    /// 魔法の効果時に呼ぶ
    /// </summary>
    /// <param name="status">魔法の効果</param>>
    /// <param name="type">状態異常の種類</param>>
    /// <param name="icon">状態異常のアイコン</param>>
    /// <param name="duration">状態異常の継続ターン</param>>
    public void StatusEffectInfliction(StatusAilment status, StatusAbnormalityType type, Sprite icon, int duration)
    {
        // 状態異常が被っていない場合、状態異常を開始する
        if (!StatusAilments.Any(sa  => sa.StatusAbnormalityType == type))
        {
            // 状態異常を開始し、このキャラクターの状態異常を追加
            status.EffectGrant(_character);
            StatusAilments.Add(status);
        }

        //TODO：これifの中に入れていいような気がするんだ
        var info = new StatusAbnormalityInfo(_baseStatus, type, icon, duration);
        _eventsSystem.onStatusAbnormalityOccurrence?.Invoke(info, _baseStatus);
    }

    /// <summary>
    /// 状態異常中の効果を与える
    /// ターン開始時に呼ぶ
    /// </summary>
    public void StatusEffectStart()
    {
        // 状態異常がある場合、効果を受ける
        for (int i = StatusAilments.Count - 1; i >= 0; i--)
        {
            //Debug.Log(statusAilments.Count + "状態異常効果を呼ぶ前");
            StatusAilments[i].EffectActivation(_character);
            //Debug.Log(statusAilments.Count + "状態異常効果を呼んだ後");
            if(StatusAilments.Count <= 0) return;
            _eventsSystem.onStatusAbnormalityProgress?.Invoke(_baseStatus, StatusAilments[i].StatusAbnormalityType);
            if (StatusAilments[i].IsEnd)
            {
                StatusAilments[i].EffectEnd(_character);
                _eventsSystem.onStatusAbnormalityEnd?.Invoke(_baseStatus, StatusAilments[i].StatusAbnormalityType);
                StatusAilments.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 痺れ状態かを判定
    /// </summary>
    /// <returns>true：痺れ状態　false：痺れ状態じゃない</returns>
    public bool IsParalysisStatus()
    {
        return StatusAilments.Any(sa => sa.StatusAbnormalityType == StatusAbnormalityType.Electrification);
    }
    
    /// <summary>
    /// 吸水状態かを判定
    /// ・吸水状態だった場合、ダメージを与えてきたキャラクターのHPを回復
    /// </summary>
    /// <param name="damage">ダメージ</param>>
    /// <param name="status">ダメージを与えてきたキャラクター</param>>
    public void IsWaterAbsorption(float damage, CharacterBaseStatus status)
    {
        // 水魔法の状態異常がある場合、HPを回復させる
        var water = StatusAilments.FirstOrDefault(sa =>
            sa.StatusAbnormalityType == StatusAbnormalityType.Wet);
        if (water != null)
        {
            var heal = damage * water.WaterAbsorption;
            status.Heal(heal);
            //Debug.Log("回復");
        }
        else
        {
            //Debug.Log("水魔法の状態異常はない");
        }
    }
    
    /// <summary>
    /// 状態異常のリストをクリアする
    /// </summary>
    public void StatusAilmentsClear()
    {
        StatusAilments.Clear();
    }
    
    /// <summary>
    /// 魔法反応が起きるか判定を行う
    /// </summary>
    /// <param name="mData">魔法の情報</param>>
    public void IsMagicReactionCheck(MagicBaseData mData)
    {
        //魔法反応が起きる状態異常かを判定する
        var msaType = mData.MagicReactionInfo.SaType;
        if (StatusAilments.Any(sa => sa.StatusAbnormalityType == msaType))
        {
            var type = mData.MagicReactionInfo.MrType;
            MagicReactionClassification(type, mData);
        }
    }

    /// <summary>
    /// 魔法反応の種類別で処理
    /// </summary>
    /// <param name="type">魔法反応</param>
    /// <param name="mData">魔法の情報</param>
    private void MagicReactionClassification(MagicReactionType type, MagicBaseData mData)
    {
        switch (type)
        {
            case MagicReactionType.Evaporation:
                var evaporation = new Evaporation(mData.MagicReactionInfo.Damage, mData.StatusEffect);
                evaporation.MagicReactionAction(_combatSystem);
                break;
            case MagicReactionType.Dissolution:
                break;
        }
    }
}
