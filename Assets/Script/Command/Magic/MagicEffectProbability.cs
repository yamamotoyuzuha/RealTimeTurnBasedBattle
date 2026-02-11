using UnityEngine;

/// <summary>
/// 魔法の効果を受ける確率
/// </summary>
public static class MagicEffectProbability
{
    /// <summary>
    /// 魔法効果を受けるかの確率を計算
    /// </summary>
    /// <param name="probability">魔法の確率○○％</param>
    /// <returns>true：受ける　false：受けない</returns>
    public static bool ProbabilityCalculation(float probability)
    {
        var mPro = probability / 100f; //0.○○の形に変換
        var pro = Random.value; //0~1のランダム
        
        return pro < mPro;
    }
}
