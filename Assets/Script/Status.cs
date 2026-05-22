
public interface Status
{
    public Character GetCharacter();
    
    /// <summary>
    /// キャラクターの属性を取得する
    /// </summary>
    /// <returns>キャラクターの属性</returns>
    public CharacterAttributesType GetAttributes();
}
