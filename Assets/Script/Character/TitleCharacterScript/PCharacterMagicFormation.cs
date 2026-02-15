using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクターの魔法編成管理
/// </summary>
public class PCharacterMagicFormation : MonoBehaviour
{
    //TODO：これは後回しでいい
    //TODO：キャラクターごとに魔法を変更出来るようにしておく
    
    [Header("PartyFormation")]
    [SerializeField] private PartyFormation _partyFormation;
    [Header("魔法編成画面UI")] 
    [SerializeField] private GameObject _mfUI;
    [Header("魔法UIの生成場所")]
    [SerializeField] private Transform _mfUIParent;
    [Header("魔法UIのPrefab")]
    [SerializeField] private GameObject _mfUIPrefab;
    /// <summary>
    /// 生成した魔法UIの保持
    /// </summary>
    private List<MagicFormationInfo> magicFormationInfos = new List<MagicFormationInfo>();
    private bool isShow;

    void Awake()
    {
        MagicUIGeneration();
        _partyFormation.onMagicFormationUI += ShowAndHide;
        _partyFormation.onMagicUpdate += MagicFormationUIShow;
    }

    /// <summary>
    /// 魔法UIの生成
    /// </summary>
    private void MagicUIGeneration()
    {
        for (int i = 0; i < 6; i++)
        {
            var obj = Instantiate(_mfUIPrefab, _mfUIParent);
            var text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            var image = obj.transform.GetChild(2).GetComponent<Image>();
            var info = new MagicFormationInfo(text, image);
            magicFormationInfos.Add(info);
        }
        _mfUI.SetActive(false);
    }
    
    /// <summary>
    /// 魔法選択UIの表示切り替え
    /// </summary>
    private void ShowAndHide()
    {
        if (!isShow) //表示
        {
            isShow = true;
            _mfUI.SetActive(true);
            MagicFormationUIShow();
        }
        else //非表示
        {
            isShow = false;
            _mfUI.SetActive(false);
        }
    }

    /// <summary>
    /// 魔法編成画面を表示する
    /// ・UIの表示やカメラアングルなど
    /// </summary>
    private void MagicFormationUIShow()
    {
        //既に選択済みの魔法の情報を設定する
        var magicData = _partyFormation.SelectedChara.MagicBaseData;
        for (int i = 0; i < magicFormationInfos.Count; i++)
        {
            var ui = magicFormationInfos[i];
            ui.MagicName.text = magicData[i].MagicName;
            ui.MagicIcon.sprite = magicData[i].StatusEffect;
        }
    }
}

/// <summary>
/// 魔法編成画面に表示する魔法UI
/// </summary>
public class MagicFormationInfo
{
    public TextMeshProUGUI MagicName { get; private set; }
    public Image MagicIcon { get; private set; }

    public MagicFormationInfo(TextMeshProUGUI text, Image icon)
    {
        MagicName = text;
        MagicIcon = icon;
    }
}
