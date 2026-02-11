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
}
