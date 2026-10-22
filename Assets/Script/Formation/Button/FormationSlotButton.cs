using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 編成画面の編成枠のボタン
/// </summary>
public class FormationSlotButton : MonoBehaviour, IPointerClickHandler
{
    [Header("この編成枠のキャラクターデータ"), SerializeField] private CharacterTitleData _characterTitleData;
    
    private CharacterFormationDisplay _characterFormationDisplay;
    private int _slotIndex;
    
    /// <summary>
    /// キャラクターデータを設定
    /// </summary>
    /// <param name="characterTitleData">変更後のキャラクターデータ</param>
    /// <param name="characterFormationDisplay">キャラクターの編成画面の管理</param>
    /// <param name="index">枠の番号</param>
    public void SetCharacterTitleData(CharacterTitleData characterTitleData, CharacterFormationDisplay characterFormationDisplay, int index)
    {
        _characterTitleData = characterTitleData;
        _characterFormationDisplay = characterFormationDisplay;
        _slotIndex = index;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO：クリックした編成枠にいるキャラクターの詳細を表示する
        // TODO：ここで編成可能なキャラクターを一覧で表示する
        
        Debug.LogWarning("FormationButton::OnPointerClick()");
        Debug.LogWarning("この編成枠のキャラクターの名前：" + _characterTitleData.CharacterName);
        
        // UIの設定と生成を行う
        //_formationCharacterListDisplay.SetUpUI();
        //_formationCharacterListDisplay.GenerationFormationCharacterListUIIcon();
        
        _characterFormationDisplay.OnFormationSlotSelected();
    }
}
