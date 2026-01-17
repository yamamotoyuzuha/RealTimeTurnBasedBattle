using UnityEngine;

public class CharacterCommandActionData : ScriptableObject
{
    /// <summary>
    /// 継承先のコマンドの種類を返す
    /// </summary>
    /// <returns>コマンドの種類</returns>
    public virtual CharacterCommandActionType GetCommandType()
    {
        return CharacterCommandActionType.None;
    }
    
    /// <summary>
    /// 魔法の場合、継承先でオーバーライドする
    /// </summary>
    /// <returns>魔法のデータ</returns>
    public virtual MagicBaseData GetMagicBaseData()
    {
        return null;
    }
    
    //行動の種類に応じて増やす
}

/// <summary>
/// 行動の種類
/// </summary>
public enum CharacterCommandActionType
{
    None,
    Magic,
    Attack,
    Item
}
