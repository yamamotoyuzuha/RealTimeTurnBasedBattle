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
    public CharacterTitleData CharacterTitleData { get; private set; }

    private CharacterFormationDisplay _characterFormationDisplay;

    public void Initialize(CharacterTitleData characterTitleData, CharacterFormationDisplay characterFormationDisplay)
    {
        CharacterTitleData = characterTitleData;
        _characterFormationDisplay = characterFormationDisplay;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO：これがクリックされたタイミングで編成管理をしているスクリプトへと情報を渡す
        
        Debug.LogWarning("CharacterSelectButton::OnPointerClick()");
        Debug.LogWarning("このキャラクターを編成に変更　：　" + CharacterTitleData.CharacterName);
        
        // TODO：ここでクリックされたら、表示しているキャラクターを変更し表示する
        // TODO：その変更された通知を CharacterFormationDisplay に送る
        // TODO：ここで選択されたら、すぐに非表示を行うわけでなく選択したキャラクターの3Dモデルを表示する
        // キャラクターアイコンの非表示を行う
        _characterFormationDisplay.OnCharacterSelectButtonSelected();
    }
}
