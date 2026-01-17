
using System;

public interface Status
{
    /// <summary>
    /// キャラのデータを返す
    /// </summary>
    /// <returns>キャラデータ</returns>
    public CharacterBaseData GetData();
    
    /// <summary>
    /// キャラクターのステータスを返す
    /// </summary>
    /// <returns>ステータス(HPなど)</returns>
    public CharacterBaseStatus GetCharacterStatus();

    /// <summary>
    /// キャラクターのMPを返す
    /// </summary>
    /// <returns>現在のMP</returns>
    public int GetMp();
    
    /// <summary>
    /// 各キャラの速度を返す
    /// </summary>
    /// <returns>速度</returns>
    public int GetSpeed();
    
    /// <summary>
    /// キャラクターの属性を取得する
    /// </summary>
    /// <returns>キャラクターの属性</returns>
    public CharacterAttributesType GetAttributes();
}
