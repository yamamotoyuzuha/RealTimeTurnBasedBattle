using UnityEngine;

/// <summary>
/// キャラクターの演出面
/// </summary>
public interface IAnimationCharacter
{
    /// <summary>
    /// コマンドのアニメーションを再生
    /// </summary>
    /// <param name="animationName">再生するアニメーションの名前</param>
    public void SetAnimationPlay(string animationName);

    /// <summary>
    /// エフェクトの生成場所を取得する
    /// </summary>
    /// <param name="effectPosName">アニメーションの名前</param>
    /// <returns>エフェクトの生成場所</returns>
    public Transform GetEffectTransform(string effectPosName);
}
