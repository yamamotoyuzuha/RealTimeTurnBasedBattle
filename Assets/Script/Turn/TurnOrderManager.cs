using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターのターン順を管理
/// </summary>
public class TurnOrderManager
{
    private readonly List<GameObject> _turnOrder = new();
    
    private int _currentTurnIndex = -1;
    
    /// <summary>
    /// ターン順
    /// </summary>
    public List<GameObject> TurnOrder => _turnOrder;

    /// <summary>
    /// 現在のターンキャラクター
    /// </summary>s
    public GameObject CurrentTurnCharacter
    {
        get
        {
            if (_currentTurnIndex < 0 || _currentTurnIndex >= _turnOrder.Count) return null;
            
            return _turnOrder[_currentTurnIndex];
        }
    }

    public void Initialize(List<GameObject> characters)
    {
        if(characters == null) return;
        
        // 速度順にソートされたキャラクターを追加
        _turnOrder.AddRange(characters);
        // 現在のターンを設定
        _currentTurnIndex = _turnOrder.Count > 0 ? 0 : -1;
    }

    /// <summary>
    /// 次のターンキャラクターを取得
    /// ・インデックスの変更はなし
    /// </summary>
    /// <returns>次のターンキャラクター</returns>
    public GameObject GetNextCharacter()
    {
        if (_turnOrder.Count == 0 || _currentTurnIndex < 0) return null;

        var nextIndex = (_currentTurnIndex + 1) % _turnOrder.Count;
        return _turnOrder[nextIndex];
    }

    /// <summary>
    /// ターンを進める
    /// </summary>
    /// <returns>進めた後のキャラクター</returns>
    public GameObject AdvanceTurn()
    {
        if (_turnOrder.Count == 0)
        {
            _currentTurnIndex = -1;
            return null;
        }

        if (_currentTurnIndex < 0) _currentTurnIndex = 0;
        else
        {
            _currentTurnIndex = (_currentTurnIndex + 1) % _turnOrder.Count;
        }
        
        return CurrentTurnCharacter;
    }
    
    public bool SetCurrentCharacter(GameObject character)
    {
        var index = _turnOrder.IndexOf(character);
        if (index < 0) return false;

        _currentTurnIndex = index;
        return true;
    }
    
    public GameObject GetNextCharacterAfter(GameObject character)
    { 
        var index = _turnOrder.IndexOf(character);
        if (index < 0 || _turnOrder.Count == 0) return null;

        var nextIndex = (index + 1) % _turnOrder.Count;

        return _turnOrder[nextIndex];
    }
    
    public bool RemoveCharacter(GameObject character)
    {
        int removeIndex = _turnOrder.IndexOf(character);
        if (removeIndex < 0) return false;

        bool isCurrentCharacter = removeIndex == _currentTurnIndex;

        _turnOrder.RemoveAt(removeIndex);

        if (_turnOrder.Count == 0)
        {
            _currentTurnIndex = -1;
            return true;
        }

        if (isCurrentCharacter)
        {
            // 削除した位置に次のキャラクターが入る
            // 末尾を削除した場合は先頭へ戻る
            _currentTurnIndex = removeIndex >= _turnOrder.Count ? 0 : removeIndex;
        }
        else if (removeIndex < _currentTurnIndex)
        {
            // 現在のキャラクターより前を削除した場合
            _currentTurnIndex--;
        }

        return true;
    }
    
}
