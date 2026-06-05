using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 必殺技のベースクラス
/// </summary>
public abstract class UltimateBaseData : ScriptableObject
{
    [Header("必殺技の名前")] 
    [SerializeField] private string _ultName;

    /// <summary>
    /// 必殺技発動時の演出
    /// </summary>
    public abstract void PlayCutIn();
    
    /// <summary>
    /// 必殺技の実行
    /// </summary>
    /// <param name="combatSystem">必殺技を受けるキャラクターの<c>CharacterCombatSystem</c>></param>
    /// <param name="baseStatus">必殺技を与えるキャラクターの<c>CharacterBaseStatus</c>></param>
    public abstract UniTask Execute(CharacterCombatSystem combatSystem, CharacterBaseStatus baseStatus);

    /// <summary>
    /// 必殺技の終了
    /// </summary>
    public abstract void End();
}
