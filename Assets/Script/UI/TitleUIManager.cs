using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// タイトルのUI管理
/// </summary>
public class TitleUIManager : MonoBehaviour
{
    [Header("TitleOperationUI")]
    [SerializeField] private GameObject _titleOperationUI;
    [Header("PFOperationUI")]
    [SerializeField] private GameObject _pfOperationUI;
    [Header("ESOperationUI")]
    [SerializeField] private GameObject _esOperationUI;
    [Header("TitleOperationUIのOperationUI")]
    [SerializeField] private Image _toImage0;
    [SerializeField] private Image _toImage1;
    [SerializeField] private Image _toImage2;
    [SerializeField] private TextMeshProUGUI _toText0;
    [SerializeField] private TextMeshProUGUI _toText1;
    [SerializeField] private TextMeshProUGUI _toText2;
    [Header("PFOperationUIのOperationUI")]
    [SerializeField] private Image _pfImage0;
    [SerializeField] private Image _pfImage1;
    [SerializeField] private Image _pfImage2;
    [SerializeField] private Image _pfImage3;
    [SerializeField] private Image _pfImage4;
    [SerializeField] private TextMeshProUGUI _pfText0;
    [SerializeField] private TextMeshProUGUI _pfText1;
    [SerializeField] private TextMeshProUGUI _pfText2;
    [SerializeField] private TextMeshProUGUI _pfText3;
    [SerializeField] private TextMeshProUGUI _pfText4;
    [Header("ESOperationUIのOperationUI")]
    [SerializeField] private Image _esImage0;
    [SerializeField] private Image _esImage1;
    [SerializeField] private Image _esImage2;
    [SerializeField] private Image _esImage3;
    [SerializeField] private TextMeshProUGUI _esText0;
    [SerializeField] private TextMeshProUGUI _esText1;
    [SerializeField] private TextMeshProUGUI _esText2;
    [SerializeField] private TextMeshProUGUI _esText3;

    /// <summary>
    /// タイトルの操作UIを表示
    /// </summary>
    public Action onTitleShow;
    /// <summary>
    /// パーティー編成の操作UIを表示
    /// </summary>
    public Action onPfShow;
    /// <summary>
    /// Enemy選択の操作UIを表示
    /// </summary>
    public Action onEsShow;
    /// <summary>
    /// 操作UIの非表示
    /// </summary>
    public Action onUIHidden;

    void Start()
    {
        OperationUISettings();
        onTitleShow += TitleShow;
        onPfShow += PfShow;
        onEsShow += EsShow;
        onUIHidden += UIHidden;
        onTitleShow?.Invoke();
    }

    /// <summary>
    /// 操作方法UIの設定
    /// </summary>
    private void OperationUISettings()
    {
        var data = OperationDataManager.Instance.OperationUIData;
        //タイトル操作UI
        _toImage0.sprite = data.PfSprite;
        _toImage1.sprite = data.EsSprite;
        _toImage2.sprite = data.ExecuteSprite;
        _toText0.text = "PartyFormation";
        _toText1.text = "EnemySelect";
        _toText2.text = "GameStart";

        //パーティー編成操作UI
        _pfImage0.sprite = data.SelectSprite;
        _pfImage1.sprite = data.DeSelectSprite;
        _pfImage2.sprite = data.LeftArrowSprite;
        _pfImage3.sprite = data.RightArrowSprite;
        _pfImage4.sprite = data.PfSprite;
        _pfText0.text = "Add";
        _pfText1.text = "Remove";
        _pfText2.text = "Left";
        _pfText3.text = "Right";
        _pfText4.text = "Back";

        //Enemy選択操作UI
        _esImage0.sprite = data.SelectSprite;
        _esImage1.sprite = data.LeftArrowSprite;
        _esImage2.sprite = data.RightArrowSprite;
        _esImage3.sprite = data.EsSprite;
        _esText0.text = "Add";
        _esText1.text = "Left";
        _esText2.text = "Right";
        _esText3.text = "Back";
    }

    private void TitleShow()
    {
        _titleOperationUI.SetActive(true);
        _pfOperationUI.SetActive(false);
        _esOperationUI.SetActive(false);
    }

    private void PfShow()
    {
        _titleOperationUI.SetActive(false);
        _pfOperationUI.SetActive(true);
        _esOperationUI.SetActive(false);
    }

    private void EsShow()
    {
        _titleOperationUI.SetActive(false);
        _pfOperationUI.SetActive(false);
        _esOperationUI.SetActive(true);
    }

    private void UIHidden()
    {
        _titleOperationUI.SetActive(false);
        _pfOperationUI.SetActive(false);
        _esOperationUI.SetActive(false);
    }
}
