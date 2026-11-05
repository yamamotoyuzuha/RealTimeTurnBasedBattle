using System;
using UnityEngine;

/// <summary>
/// 編成画面でのドラッグとドロップの管理
/// </summary>
public class FormationSlotDragController : MonoBehaviour
{
    /// <summary>
    /// ドロップした際に行う
    /// </summary>
    public event Action<int, int> OnFormationSlotSwapRequested;
    
    private int _slotDragIndex; // ドラッグした編成スロットのインデックス

    /// <summary>
    /// ドラッグ中
    /// </summary>
    public void Dragging()
    {
        // TODO：ここにカーソルの移動時に分かりやすいUIを作成しておく
        // TODO：優先度は低め
        //Debug.LogWarning("ドラッグ中");
    }

    /// <summary>
    /// ドラッグ開始
    /// </summary>
    /// <param name="dragIndex">ドラッグした編成枠のインデックス</param>
    public void BeginDrag(int dragIndex)
    {
        _slotDragIndex = dragIndex;
    }

    /// <summary>
    /// ドラッグ終了
    /// </summary>
    public void EndDrag()
    {
        _slotDragIndex = -1;
        
        // TODO：終了したらUIを非表示とかにする
    }

    /// <summary>
    /// ドロップ
    /// </summary>
    /// <param name="dropIndex">ドロップした編成枠のインデックス</param>
    public void Drop(int dropIndex)
    {
        if(_slotDragIndex < 0) return;
        if(_slotDragIndex == dropIndex) return;
        
        OnFormationSlotSwapRequested?.Invoke(_slotDragIndex, dropIndex);
    }
}
