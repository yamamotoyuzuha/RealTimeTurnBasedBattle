using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// パーティー編成画面の管理
/// ・UI生成
/// ・UIの表示切替等
/// ・キャラクター生成
/// </summary>
public class PartyFormationUIController : MonoBehaviour
{
    [Header("パーティーメンバーUIの生成場所")] 
    [SerializeField] private Transform _partyFormationParent;
    [Header("パーティーメンバーUIのPrefab")]
    [SerializeField] private GameObject _partyCharacterUIPrefab;
    [Header("パーティーメンバーの生成場所")]
    [SerializeField] private Transform[] _partyCharacterParent;
    
    /// <summary>
    /// パーティーメンバーUIの保持
    /// </summary>
    private List<PartyCharacterUIInfo> partyCharacterUIInfos = new List<PartyCharacterUIInfo>();
    /// <summary>
    /// 生成したパーティーキャラクターの保持
    /// </summary>
    private List<PartyCharacterInfo> partyCharacterInfos = new List<PartyCharacterInfo>();

    /// <summary>
    /// パーティーメンバーのUIを生成するときに呼ぶ
    /// </summary>
    public Action<CharacterBaseData> onPartyFormationGenerate;
    /// <summary>
    /// パーティーメンバーを生成するときに呼ぶ
    /// </summary>
    public Action<CharacterBaseData, int> onPartyFormationCharacterGenerate;
    /// <summary>
    /// 選択しているキャラクターを変更するときに呼ぶ
    /// </summary>
    public Action<CharacterBaseData> onSelectCharacter;
    /// <summary>
    /// 選択しているキャラクターのパーティー加入、外す
    /// </summary>
    public Action<CharacterBaseData, bool> onAddOrRemoveCharacter;
    /// <summary>
    /// パーティー編成画面を表示
    /// </summary>
    public Action<bool> onPartyShowDisplay;
    /// <summary>
    /// パーティー編成画面を非表示
    /// </summary>
    public Action<bool> onPartyHideDisplay;
    
    void Awake()
    {
        onPartyFormationGenerate += PartyFormationUIGenerate;
        onPartyFormationCharacterGenerate += PartyFormationCharacterGenerate;
        onSelectCharacter += SelectCharacterUI;
        onAddOrRemoveCharacter += AddOrRemoveCharacter;
        onPartyShowDisplay += (flag) =>
        {
            PartySwitchDisplay(flag);
            PartyCharacterSwitchDisplay(flag);
        };
        onPartyHideDisplay += (flag) =>
        {
            PartySwitchDisplay(flag);
            PartyCharacterSwitchDisplay(flag);
        };
    }

    /// <summary>
    /// パーティーメンバーUIの生成を行う
    /// </summary>
    /// <param name="characterBaseData">生成するキャラクターの情報</param>
    private void PartyFormationUIGenerate(CharacterBaseData characterBaseData)
    {
        var obj = Instantiate(_partyCharacterUIPrefab, _partyFormationParent);
        var image = obj.transform.GetChild(0).GetComponent<Image>();
        var nameText = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var hpCText = obj.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        var info = new PartyCharacterUIInfo()
        {
            charaData = characterBaseData,
            partyUIObj = obj,
            partyCharacterBg = image,
            partyCharacterName = nameText,
            partyCharacterCurrentHp = hpCText
        };
        partyCharacterUIInfos.Add(info);
        info.Setup(characterBaseData);
        info.partyUIObj.SetActive(false);
    }
    /// <summary>
    /// パーティーメンバーの生成を行う
    /// </summary>
    /// <param name="characterBaseData">生成するキャラクターのデータ</param>
    /// <param name="index">インデックス</param>>
    private void PartyFormationCharacterGenerate(CharacterBaseData characterBaseData, int index)
    {
        var obj = Instantiate(characterBaseData.TitleCharacterPrefab, _partyCharacterParent[index]);
        var info = new PartyCharacterInfo()
        {
            charaData = characterBaseData,
            charaObj = obj
        };
        partyCharacterInfos.Add(info);
        info.charaObj.SetActive(false);
    }

    /// <summary>
    /// キャラクターの選択が移動した時に呼ばれる
    /// </summary>
    /// <param name="characterBaseData">移動後のキャラクターデータ</param>>
    private void SelectCharacterUI(CharacterBaseData characterBaseData)
    {
        //一致したキャラクターデータのUIに変更を加える
        foreach (var info in partyCharacterUIInfos)
        {
            if (info.charaData == characterBaseData)
            {
                info.partyUIObj.transform.DOScale(
                    new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
            }
            else
            {
                info.partyUIObj.transform.DOScale(
                    new Vector3(1f, 1f, 1f), 0.2f);
            }
        }
    }

    /// <summary>
    /// 選択中のキャラクターが加入もしくは、外されたときに呼ぶ
    /// </summary>
    /// <param name="characterBaseData">選択中のキャラクター</param>
    /// <param name="isAdd">true：加入　false：外された</param>
    private void AddOrRemoveCharacter(CharacterBaseData characterBaseData, bool isAdd)
    {
        if (isAdd)
        {
            foreach (var info in partyCharacterUIInfos)
            {
                if (info.charaData == characterBaseData)
                {
                    info.partyCharacterBg.color = Color.gray;
                    return;
                }
            }
        }
        else
        {
            foreach (var info in partyCharacterUIInfos)
            {
                if (info.charaData == characterBaseData)
                {
                    info.partyCharacterBg.color = Color.black;
                    return;
                }
            }
        }
    }

    /// <summary>
    /// パーティー編成画面を表示、非表示
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>>
    private void PartySwitchDisplay(bool flag)
    {
        foreach (var info in partyCharacterUIInfos)
        {
            info.partyUIObj.SetActive(flag);
        }
    }

    /// <summary>
    /// パーティーキャラクターを表示、非表示
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>>
    private void PartyCharacterSwitchDisplay(bool flag)
    {
        foreach (var info in partyCharacterInfos)
        {
            info.charaObj.SetActive(flag);
        }
    }
}

/// <summary>
/// パーティーメンバーUIの情報
/// </summary>
public class PartyCharacterUIInfo
{
    public CharacterBaseData charaData;
    public GameObject partyUIObj;
    public Image partyCharacterBg;
    public TextMeshProUGUI partyCharacterName;
    public TextMeshProUGUI partyCharacterCurrentHp;

    /// <summary>
    /// UIの設定を行う
    /// </summary>
    /// <param name="data">設定するキャラクターのデータ</param>
    public void Setup(CharacterBaseData data)
    {
        partyCharacterName.text = data.CharacterName;
        var hp = data.Hp.ToString();
        partyCharacterCurrentHp.text = hp + "/" + hp;
    }
}
/// <summary>
/// パーティーキャラクターの情報
/// </summary>
public class PartyCharacterInfo
{
    public CharacterBaseData charaData;
    public GameObject charaObj;
}
