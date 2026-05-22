using UnityEngine;

/// <summary>
/// キャラクターのコマンドを管理する
/// </summary>
public class CharacterCommandSystem
{
    public CommandState CurrentCommandState {get; private set;}
    
    public CommandUI CommandUI {get; private set;}

    public CharacterCommandSystem(CommandState currentCommandState, CommandUI commandUI)
    {
        CurrentCommandState = currentCommandState;
        CommandUI = commandUI;
    }
    
    public void ChangeCommandState(CommandState newCommandState)
    {
        CurrentCommandState = newCommandState;
    }
    
    public void ShowCommandUI(bool flag)
    {
        CommandUI.ToggleCommandUI(flag);
    }
}
