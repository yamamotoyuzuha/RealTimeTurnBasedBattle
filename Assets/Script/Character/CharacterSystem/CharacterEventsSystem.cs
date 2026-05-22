using System;
using UnityEngine;

/// <summary>
/// キャラクターのステータスに関するイベントを管理する
/// HPなどのUI更新など
/// </summary>
public class CharacterEventsSystem
{
    #region キャラクターステータス（HP等）

    /// <summary>
    /// HPの変動
    /// （現在のHP、最大HP）
    /// </summary>
    public Action<CharacterBaseStatus, float, float> onHpChanged;
    /// <summary>
    /// MPの増加
    /// （増加後のMP、増減後のMP）
    /// </summary>
    public Action<CharacterBaseStatus, int, int> onMpAdd;
    /// <summary>
    /// MPの増減
    /// （増減後のMP、増減前の）
    /// </summary>
    public Action<CharacterBaseStatus, int, int> onMpReduce;

    #endregion

    #region 特殊ステータス

    /// <summary>
    /// 体幹ゲージの変動
    /// （現在の体幹ゲージ量、最大体幹ゲージ量）
    /// </summary>
    public Action<float, float> onCoreGaugeChanged;
    /// <summary>
    /// 必殺技ゲージ量の変動
    /// （現在の必殺技ゲージ量、最大必殺技ゲージ量）
    /// </summary>
    public Action<int, int> onSpecialMoveChanged;

    #endregion

    #region 防御アクション

    /// <summary>
    /// パリィ成功
    /// （増えるMP量）
    /// </summary>
    public Action<int> onParrySuccess;
    /// <summary>
    /// ジャストガード成功
    /// </summary>
    public Action onJustGuardSuccess;

    #endregion

    #region 状態異常

    /// <summary>
    /// 状態異常が発生
    /// </summary>
    public Action<StatusAbnormalityInfo, CharacterBaseStatus> onStatusAbnormalityOccurrence;
    /// <summary>
    /// 状態異常が経過
    /// </summary>
    public Action<CharacterBaseStatus, StatusAbnormalityType> onStatusAbnormalityProgress;
    /// <summary>
    /// 状態異常が終了
    /// </summary>
    public Action<CharacterBaseStatus, StatusAbnormalityType> onStatusAbnormalityEnd;

    #endregion

    #region 演出

    /// <summary>
    /// 攻撃を受けた時の演出
    /// </summary>
    public Action onHitEffect;
    /// <summary>
    /// 死亡した時の演出
    /// </summary>
    public Action onDeathEffect;

    #endregion
    
    /// <summary>
    /// プレイヤー操作キャラクターのみ
    /// ステータスUIの表示、非表示を行う
    /// </summary>
    public Action<CharacterBaseStatus, bool> onStatusDisplay;
   
    /// <summary>
    /// 死亡
    /// </summary>
    public Action<GameObject> onDeath;
    
    public CharacterEventsSystem()
    {
        
    }
}
