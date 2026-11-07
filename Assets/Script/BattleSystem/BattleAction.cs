using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 戦闘アクションの管理
/// </summary>
public class BattleAction
{
    private readonly Dictionary<CommandState, Func<Character, UniTask>> _commandActions = new();

    /// <summary>
    /// コマンド登録
    /// </summary>
    /// <param name="command">コマンドの種類</param>
    /// <param name="commandAction">コマンド処理</param>
    public void RegisterCommand(CommandState command, Func<Character, UniTask> commandAction)
    {
        if(commandAction == null) return;
        
        _commandActions[command] = commandAction;
    }

    /// <summary>
    /// コマンド解除
    /// </summary>
    /// <param name="command">解除するコマンド</param>
    public void RemoveCommand(CommandState command)
    {
        _commandActions.Remove(command);
    }

    /// <summary>
    /// コマンド実行
    /// </summary>
    /// <param name="character">実行を行うキャラクター</param>
    public async UniTask ExecuteCommand(Character character)
    {
        if(character == null) return;

        var command = character.CommandsSystem.CurrentCommandState;
        if (!_commandActions.TryGetValue(command, out var commandAction))
        {
            Debug.LogWarning("コマンドが登録されていない");
            return;
        }
        
        await commandAction.Invoke(character);
    }
}
