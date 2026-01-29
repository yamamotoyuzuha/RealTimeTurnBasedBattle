using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// バトル中のUI管理を行う
/// </summary>
public class BattleOperatingInstructionsUI : MonoBehaviour
{
    public static BattleOperatingInstructionsUI Instance { get; private set; }
    
    [Header("コマンド選択UI")]
    [SerializeField] private GameObject _commandSelectionUI;
    [Header("コマンド確定UI")]
    [SerializeField] private GameObject _commandConfirmedUI;
    [Header("コマンド実行UI")]
    [SerializeField] private GameObject _commandExecuteUI;
    [Header("防御アクションUI")] 
    [SerializeField] private GameObject _defenseActionUI;

    void Awake()
    {
        if (Instance == null) Instance = this;
        UISettings();
    }

    /// <summary>
    /// UIの設定
    /// </summary>
    private void UISettings()
    {
        //イメージ
        var uiSI = _commandSelectionUI.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        var uiCI =  _commandConfirmedUI.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        var uiAI = _commandExecuteUI.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        var uiPI = _defenseActionUI.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        var uiGI = _defenseActionUI.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
        var data = OperationDataManager.Instance.OperationUIData;
        uiSI.sprite = data.BackSprite;
        uiCI.sprite = data.BackSprite;
        uiAI.sprite = data.ExecuteSprite;
        uiPI.sprite = data.ParrySprite;
        uiGI.sprite = data.JustGuardSprite;
        
        //テキスト
        var uiS = _commandSelectionUI.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var uiC = _commandConfirmedUI.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var uiA = _commandExecuteUI.transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var dfP = _defenseActionUI.transform.GetChild(1).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var dfG = _defenseActionUI.transform.GetChild(2).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        uiS.text = "Back";
        uiC.text = "Back";
        uiA.text = "Execute";
        dfP.text = "Parry";
        dfG.text = "Just Guard";
        HiddenUI();
    }

    /// <summary>
    /// コマンド選択UIの表示
    /// </summary>
    public void CommandSelectionUI()
    {
        _commandSelectionUI.SetActive(true);
        _commandConfirmedUI.SetActive(false);
        _commandExecuteUI.SetActive(false);
    }

    /// <summary>
    /// コマンド確定UIの表示
    /// コマンド実行UIの表示
    /// </summary>
    public void CommandConfirmedUI()
    {
        _commandSelectionUI.SetActive(false);
        _commandConfirmedUI.SetActive(true);
        _commandExecuteUI.SetActive(true);
    }

    public void HiddenUI()
    {
        _commandSelectionUI.SetActive(false);
        _commandConfirmedUI.SetActive(false);
        _commandExecuteUI.SetActive(false);
    }

    /// <summary>
    /// 防御アクションUIの表示、非表示
    /// Enemyのターン中に表示する
    /// true：表示　false：非表示
    /// </summary>
    public void DefenseActionUI(bool isDisplay)
    {
        _defenseActionUI.SetActive(isDisplay);
    }
}
