/// <summary>
/// コマンド関連の処理
/// </summary>
public interface ICommand
{
    /// <summary>
    /// キャラクターのコマンドを取得する
    /// </summary>
    /// <returns>現在のコマンドを返す</returns>
    public CommandState GetCommand();
    
    /// <summary>
    /// 現在のコマンドを変更する
    /// </summary>
    /// <param name="commandState">変更するコマンド</param>
    public void SetCommand(CommandState commandState);

    /// <summary>
    /// コマンドのUIを表示する
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>>
    public void ShowCommandUI(bool flag);
}
