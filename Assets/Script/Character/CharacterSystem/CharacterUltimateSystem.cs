/// <summary>
/// キャラクターの必殺技を完了する
/// </summary>
public class CharacterUltimateSystem
{
    /// <summary>
    /// 必殺技ゲージの最大量
    /// </summary>
    public int UltimateMaxAmount { get; private set; }
    /// <summary>
    /// 必殺技ゲージの現在の量
    /// </summary>
    public int UltimateCurrentAmount { get; private set; }

    public CharacterUltimateSystem(int max)
    {
        UltimateMaxAmount = max;
        UltimateCurrentAmount = 0;
    }

    /// <summary>
    /// 必殺技が発動可能かを判定
    /// </summary>
    /// <returns>true：可能　false：不可能</returns>
    public bool IsCanActivation()
    {
        return UltimateCurrentAmount >= UltimateMaxAmount;
    }
}
