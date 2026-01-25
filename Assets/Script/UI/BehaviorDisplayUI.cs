using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 行動内容を表示するUI
/// ・Enemyの行動
/// ・防御アクションの内容など
/// </summary>
public class BehaviorDisplayUI : MonoBehaviour
{
    [Header("行動内容UI")]
    [SerializeField] private GameObject _actionUI;
    private TextMeshProUGUI actionText;
    
    /// <summary>
    /// 行動内容UIの内容設定と表示、非表示
    /// string：表示する内容
    /// bool：表示、非表示
    /// </summary>
    public Action<string, bool> OnActionUIDisplay { get; private set; }

    void Awake()
    {
        ActionUISettings();
    }

    /// <summary>
    /// ActionUIの設定
    /// </summary>
    private void ActionUISettings()
    {
        actionText = _actionUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _actionUI.SetActive(false);
        OnActionUIDisplay += SetActionUI;
    }

    /// <summary>
    /// 行動内容UIの内容設定と表示、非表示
    /// </summary>
    /// <param name="text">表示する内容</param>
    /// <param name="flag">true：表示　false：非表示</param>>
    public void SetActionUI(string text, bool flag)
    {
        actionText.text = text;
        _actionUI.SetActive(flag);
    }
}
