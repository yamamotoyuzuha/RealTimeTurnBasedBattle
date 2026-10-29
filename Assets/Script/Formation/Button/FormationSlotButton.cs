using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 編成画面の編成枠のボタン
/// </summary>
public class FormationSlotButton : MonoBehaviour, IPointerClickHandler
{
    [Header("この編成枠のキャラクターデータ"), SerializeField] private CharacterTitleData _characterTitleData;
    
    /// <summary>
    /// 編成枠が選択されたときに行われる
    /// </summary>
    public event Action<int> OnSlotSelected;
    private int _slotIndex;
    
    /// <summary>
    /// 初期化
    /// ・キャラクターのデータ設定等
    /// </summary>
    /// <param name="characterTitleData">キャラクターデータ</param>
    /// <param name="index">枠の番号</param>
    public void Initialize(CharacterTitleData characterTitleData, int index)
    {
        _characterTitleData = characterTitleData;
        _slotIndex = index;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // TODO：クリックした編成枠にいるキャラクターの詳細を表示する
        // TODO：ここで編成可能なキャラクターを一覧で表示する
        
        Debug.LogWarning("FormationButton::OnPointerClick()");
        Debug.LogWarning("この編成枠のキャラクターの名前：" + _characterTitleData.CharacterName);
        
        OnSlotSelected?.Invoke(_slotIndex);
    }

    /// <summary>
    /// この編成枠のキャラクターデータの設定
    /// </summary>
    /// <param name="characterTitleData">編成枠に設定するキャラクターデータ</param>
    public void SetCharacterTitleData(CharacterTitleData characterTitleData)
    {
        _characterTitleData = characterTitleData;
    }
}
