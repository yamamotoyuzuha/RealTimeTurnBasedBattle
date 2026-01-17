using UnityEngine;
using UnityEngine.UI;

public class TurnCharacterIcon : MonoBehaviour
{
    private Image icon; //アイコン

    private void Awake()
    {
        icon = GetComponent<Image>();
    }

    /// <summary>
    /// キャラクターアイコンを設定する
    /// </summary>
    /// <param name="characterBaseData">キャラクターデータ</param>
    public void SetCharacterIcon(CharacterBaseData characterBaseData)
    {
        icon.sprite = characterBaseData.CharacterIconSprite;
    }
}
