using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PartyStatusUI : MonoBehaviour
{
    [Header("参照")]
    [Header("TurnManager")]
    [SerializeField] private TurnManager _turnManager;
    [Header("UI関連")]
    [Header("ステータスアイコン生成場所")] 
    [SerializeField] private Transform _statusParent;
    [Header("ステータスアイコン")]
    [SerializeField] private GameObject _statusUIPrefab;
    [Header("MP")]
    [SerializeField] private GameObject _mpPrefab;
    
    /// <summary>
    /// パーティーステータスUIの表示切り替え
    /// true：表示　false：非表示
    /// </summary>
    public Action<bool> onPartyStatusDisplay;
    
    /// <summary>
    /// 生成したステータスUIの管理
    /// </summary>
    private Dictionary<CharacterBaseStatus, PlayerStatusUI> playerStatusUIs = new Dictionary<CharacterBaseStatus, PlayerStatusUI>();
    /// <summary>
    /// プレイヤー操作キャラクター
    /// </summary>
    private List<GameObject> partyCharas = new List<GameObject>();

    void Start()
    {
        StatusUIGenerate();
        onPartyStatusDisplay += AllStatusUIDisplay;
    }

    /// <summary>
    /// ステータスアイコンを生成する
    /// </summary>
    private void StatusUIGenerate()
    {
        for (int i = 0; i < 2; i++)
        {
            var obj = Instantiate(_statusUIPrefab, _statusParent);
            //TODO：これはキャラクターのＭＰを参照したほうがよい
            //MPを生成して、仮配列に格納
            GameObject[] mps = new GameObject[7];
            for (int j = 0; j < 7; j++)
            {
                mps[j] = Instantiate(_mpPrefab, obj.transform.GetChild(4).transform);
            }
            
            //UIの情報を作成する
            var ui = new PlayerStatusUI(
                obj,
                obj.transform.GetChild(2).GetComponent<Image>(),
                obj.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>(),
                obj.transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>(),
                mps
            );
            
            var chara = partyCharas[i].GetComponent<Status>();
            playerStatusUIs[chara.GetCharacterStatus()] = ui;
            //HPなどの情報を設定
            playerStatusUIs[chara.GetCharacterStatus()].maxHpText.text = chara.GetCharacterStatus().MaxHp.ToString();
            playerStatusUIs[chara.GetCharacterStatus()].currentHpText.text = chara.GetCharacterStatus().Hp.ToString();
            //Actionの登録
            chara.GetCharacterStatus().onHpChanged += HpIncreaseOrDecrease;
            chara.GetCharacterStatus().onMpAdd += MpAddUI;
            chara.GetCharacterStatus().onMpReduce += MpReduceUI;
            chara.GetCharacterStatus().onStatusDisplay += StatusUIDisplay;
            
            ui.uiObj.SetActive(false);
        }
    }

    /// <summary>
    /// プレイヤー操作キャラクターを設定
    /// </summary>
    /// <param name="chara">プレイヤー操作キャラクター</param>
    public void SetPartyCharacter(GameObject chara)
    {
        partyCharas.Add(chara);
    }

    /// <summary>
    /// ステータスUIを一括表示切り替え
    /// </summary>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void AllStatusUIDisplay(bool isDisplay)
    {
        foreach (var ui in playerStatusUIs)
        {
            ui.Value.uiObj.SetActive(isDisplay);
        }
    }
    /// <summary>
    /// ステータスUIの表示切り替え
    /// </summary>
    /// <param name="data">切り替えをするキャラクター</param>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void StatusUIDisplay(CharacterBaseStatus data, bool isDisplay)
    { 
        var uiObj = playerStatusUIs[data].uiObj;
        uiObj.SetActive(isDisplay);
    }
    
    /// <summary>
    /// HPが増減したときに、UIに反映させる
    /// </summary>
    /// <param name="status">HPが変動したキャラ</param>>
    /// <param name="currentHp">現在のHP</param>
    /// <param name="maxHp">最大HP</param>
    private void HpIncreaseOrDecrease(CharacterBaseStatus status, float currentHp, float maxHp)
    {
        //HPが変動したキャラのUIを更新する
        var ui = playerStatusUIs[status];
        ui.hpBar.fillAmount = currentHp / maxHp;
        ui.currentHpText.text = status.Hp.ToString();
    }

    /// <summary>
    /// MPが増えた時に、UIに反映させる
    /// </summary>
    /// <param name="status">MPが変動したキャラ</param>
    /// <param name="currentMp">増加後のMP</param>
    /// <param name="beforeMp">増加前のMP</param>
    private void MpAddUI(CharacterBaseStatus status, int currentMp, int beforeMp)
    {
        //増加後と増加前の値の差を求め、増えた分のMPを表示する
        var mp = currentMp - beforeMp;
        for (int i = 0; i < mp; i++)
        {
            var mps = playerStatusUIs[status].mps;
            mps[i].SetActive(true);
        }
    }

    /// <summary>
    /// MPが減った時に、UIに反映させる
    /// </summary>
    /// <param name="status">MPが変動したキャラ</param>
    /// <param name="currentMp">増減後のMP</param>
    /// <param name="beforeMp">増減前のMP</param>
    private void MpReduceUI(CharacterBaseStatus status, int currentMp, int beforeMp)
    {
        //増減後のと増減前の値の差を求め、減った分のMPを非表示にする
        var mp = beforeMp - currentMp;
        for (int i = 0; i < mp; i++)
        {
            var mps = playerStatusUIs[status].mps;
            mps[i].SetActive(false);
        }
    }
}

/// <summary>
/// StatusUIの情報を纏めたもの
/// </summary>
public class PlayerStatusUI
{
    //生成したUI
    public GameObject uiObj;
    
    //HP関連
    public Image hpBar;
    public TextMeshProUGUI maxHpText;
    public TextMeshProUGUI currentHpText;
    
    //MP関連
    public GameObject[] mps;

    public PlayerStatusUI(GameObject uiObj, Image hpBar, TextMeshProUGUI maxHpText, TextMeshProUGUI currentHpText, GameObject[] mps)
    {
        this.uiObj = uiObj;
        this.hpBar = hpBar;
        this.maxHpText = maxHpText;
        this.currentHpText = currentHpText;
        this.mps = mps;
    }
}
