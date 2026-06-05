using System;
using UnityEngine;

/// <summary>
/// キャラクターの必殺技を完了する
/// </summary>
public class CharacterUltimateSystem
{
    private CharacterBaseStatus _baseStatus;
    private CharacterEventsSystem _eventsSystem;

    /// <summary>
    /// キャラクターの必殺技データ
    /// </summary>
    private UltimateBaseData _ultimateBaseData;
    
    /// <summary>
    /// 必殺技ゲージの最大量
    /// </summary>
    private int _ultimateMaxAmount;

    /// <summary>
    /// 必殺技ゲージの現在の量
    /// </summary>
    private int _ultimateCurrentAmount;

    /// <summary>
    /// 必殺技ゲージのチャージ量
    /// （ターン開始時にチャージされる量）
    /// </summary>
    private int _ultChargeAmount;

    /// <summary>
    /// 必殺技ゲージの初期設定
    /// </summary>
    /// <param name="baseStatus">CharacterBaseStatus</param>
    /// <param name="eventsSystem">CharacterEventsSystem</param>
    /// <param name="ultimateBaseData">必殺技データ</param>>
    /// <param name="max">必殺技ゲージの最大量</param>
    /// <param name="charge">必殺技ゲージのチャージ量</param>
    public CharacterUltimateSystem(CharacterBaseStatus baseStatus, CharacterEventsSystem eventsSystem, 
        UltimateBaseData ultimateBaseData, int max, int charge)
    {
        _baseStatus = baseStatus;
        _eventsSystem = eventsSystem;
        _ultimateBaseData = ultimateBaseData;
        _ultimateMaxAmount = max;
        _ultimateCurrentAmount = 0;
        _ultChargeAmount = charge;
    }

    /// <summary>
    /// 必殺技が発動可能かを判定
    /// </summary>
    /// <returns>true：可能　false：不可能</returns>
    public bool IsCanActivation()
    {
        return _ultimateCurrentAmount >= _ultimateMaxAmount;
    }

    /// <summary>
    /// 必殺技ゲージをチャージする
    /// ・プレイヤー操作キャラのターン開始時に呼ぶ
    /// </summary>
    public void UltimateCharge()
    {
        _ultimateCurrentAmount = Math.Min(_ultimateCurrentAmount + _ultChargeAmount, _ultimateMaxAmount);
        _eventsSystem.onUltimateGaugeChanged?.Invoke(_baseStatus, _ultimateCurrentAmount, _ultimateMaxAmount);
    }

    /// <summary>
    /// 必殺技ゲージを０にする
    /// ・必殺技発動後に呼ぶ
    /// </summary>
    private void UltimateGaugeClear()
    {
        _ultimateCurrentAmount = 0;
        _eventsSystem.onUltimateGaugeChanged?.Invoke(_baseStatus, _ultimateCurrentAmount, _ultimateMaxAmount);
    }

    /// <summary>
    /// 必殺技の発動処理
    /// </summary>
    public void UltimateActivation()
    {
        if(!IsCanActivation()) return;
        
        _eventsSystem.onUltimateActivated?.Invoke(_ultimateBaseData, _baseStatus);
        UltimateGaugeClear();
    }
}
