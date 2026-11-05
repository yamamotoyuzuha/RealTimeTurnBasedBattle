using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 編成画面の編成枠のボタン
/// </summary>
public class FormationSlotButton : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [Header("この編成枠のキャラクターデータ"), SerializeField] private CharacterTitleData _characterTitleData;
    
    /// <summary>
    /// 編成枠が選択されたときに行われる
    /// </summary>
    public event Action<int> OnSlotSelected;

    public event Action OnSlotDragging;
    public event Action<int> OnSlotSwapBegin;
    public event Action OnSlotSwapEnd;
    public event Action<int> OnSlotDrop;
    private int _slotIndex; // 枠のインデックス
    
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
        OnSlotSelected?.Invoke(_slotIndex);
    }

    public void OnDrag(PointerEventData eventData)      // ドラッグ中
    {
        OnSlotDragging?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData) // ドラッグ開始
    {
        OnSlotSwapBegin?.Invoke(_slotIndex);
    }

    public void OnEndDrag(PointerEventData eventData)   // ドラッグ終了
    {
        OnSlotSwapEnd?.Invoke();
    }

    public void OnDrop(PointerEventData eventData)      // ドロップ
    {
        OnSlotDrop?.Invoke(_slotIndex);
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
