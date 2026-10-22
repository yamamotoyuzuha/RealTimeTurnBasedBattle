using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 編成可能なキャラクターアイコンUIの情報
/// </summary>
public class FormationCharacterIconInfo : MonoBehaviour
{
    /// <summary>
    /// キャラクターデータ
    /// </summary>
    public CharacterTitleData Data { get; private set; }

    [SerializeField] private Image _characterIconImage;
    
    /// <summary>
    /// 初期化
    /// ・キャラクターデータ、アイコンの設定を行う
    /// </summary>
    /// <param name="data">キャラクターデータ</param>
    public void Initialize(CharacterTitleData data)
    {
        Data = data;
        _characterIconImage.sprite = Data.CharacterIconSprite;
    }
}
