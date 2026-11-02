using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 編成候補一覧のキャラクター選択ボタン
/// </summary>
public class CharacterSelectButton : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// このボタンが担当するキャラクター
    /// </summary>
    private CharacterTitleData _characterTitleData;
    /// <summary>
    /// キャラクターアイコンが選択されたときに行われる
    /// </summary>
    public event Action<CharacterTitleData> OnCharacterSelected;

    public void Initialize(CharacterTitleData characterTitleData)
    {
        _characterTitleData = characterTitleData;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.LogWarning("このキャラクターを編成に変更　：　" + _characterTitleData.CharacterName);
        OnCharacterSelected?.Invoke(_characterTitleData);
    }
}
