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
    [Header("状態異常UIのPrefab")]
    [SerializeField] private GameObject _statusAbnormalityPrefab;
    
    /// <summary>
    /// パーティーステータスUIの表示切り替え
    /// true：表示　false：非表示
    /// </summary>
    public Action<bool> onPartyStatusDisplay;
    
    /// <summary>
    /// 生成したステータスUIの管理
    /// </summary>
    private Dictionary<CharacterBaseStatus, PlayerStatusUI> playerStatusUIs = 
        new Dictionary<CharacterBaseStatus, PlayerStatusUI>();
    /// <summary>
    /// 状態異常UIの生成場所を保持
    /// </summary>
    private Dictionary<CharacterBaseStatus, Transform> saGeneratePositions =
        new Dictionary<CharacterBaseStatus, Transform>();
    /// <summary>
    /// 生成した状態異常UIの情報
    /// </summary>
    private Dictionary<StatusAbnormalityInfo, StatusAbnormalityUIInfo> statusAbnormalityInfos =
        new Dictionary<StatusAbnormalityInfo, StatusAbnormalityUIInfo>();
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
        var count = partyCharas.Count;
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(_statusUIPrefab, _statusParent);
            // MPを生成して、仮配列に格納
            var mpCount = _turnManager.Characters[partyCharas[i]].BaseStatus.MaxMp;
            GameObject[] mps = new GameObject[mpCount];
            for (int j = 0; j < mpCount; j++)
            {
                mps[j] = Instantiate(_mpPrefab, obj.transform.GetChild(4).transform);
            }

            //UIの情報を作成する
            var ui = new PlayerStatusUI(
                obj,
                obj.transform.GetChild(1).GetComponent<Image>(),
                obj.transform.GetChild(2).GetComponent<Image>(),
                obj.transform.GetChild(5).GetChild(0).GetComponent<TextMeshProUGUI>(),
                obj.transform.GetChild(5).GetChild(1).GetComponent<TextMeshProUGUI>(),
                mps,
                obj.transform.GetChild(8).GetComponent<Image>(),
                obj.transform.GetChild(8).GetComponent<Button>()
            );
            
            var chara = _turnManager.Characters[partyCharas[i]];
            var baseStatus = chara.BaseStatus;
            playerStatusUIs[baseStatus] = ui;
            // HPなどの情報を設定
            playerStatusUIs[baseStatus].CharaIcon.sprite =
                _turnManager.Characters[partyCharas[i]].BaseData.CharacterStatusIconSprite;
            playerStatusUIs[baseStatus].MaxHpText.text = chara.BaseStatus.MaxMp.ToString();
            playerStatusUIs[baseStatus].CurrentHpText.text = chara.BaseStatus.Hp.ToString();
            // Actionの登録
            chara.EventsSystem.onHpChanged += HpIncreaseOrDecrease;
            chara.EventsSystem.onMpAdd += MpAddUI;
            chara.EventsSystem.onMpReduce += MpReduceUI;
            chara.EventsSystem.onStatusDisplay += StatusUIDisplay;
            chara.EventsSystem.onUltimateGaugeChanged += UltimateIncreaseOrDecrease;
            ui.UIObj.SetActive(false);

            // 必殺技ゲージの初期チャージを行う
            chara.UltimateSystem.UltimateCharge();
            // 必殺技ゲージに動的にクリックイベントを登録
            playerStatusUIs[baseStatus].UltimateButton.onClick.AddListener(chara.UltimateSystem.UltimateActivation);
        }
        
        // 状態異常UIの生成位置を取得
        foreach (var info in _turnManager.Characters.Values)
        {
            // Enemyの場合、処理を飛ばす（Enemy用の状態異常UIが別に存在するため）
            if(!info.IsPlayer) continue;
            
            var status = info.EventsSystem;
            var baseStatus = info.BaseStatus;
            var ui = playerStatusUIs[baseStatus].UIObj.transform.GetChild(6);
            saGeneratePositions.Add(baseStatus, ui);
            status.onStatusAbnormalityOccurrence += StatusAbnormalityUIGenerate;
            status.onStatusAbnormalityProgress += StatusAbnormalityUIProgress;
            status.onStatusAbnormalityEnd +=  StatusAbnormalityUIEnd;
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
            ui.Value.UIObj.SetActive(isDisplay);
        }
    }
    
    /// <summary>
    /// ステータスUIの表示切り替え
    /// </summary>
    /// <param name="data">切り替えをするキャラクター</param>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void StatusUIDisplay(CharacterBaseStatus data, bool isDisplay)
    { 
        var uiObj = playerStatusUIs[data].UIObj;
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
        ui.HpBar.fillAmount = currentHp / maxHp;
        ui.CurrentHpText.text = status.Hp.ToString();
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
            var mps = playerStatusUIs[status].Mps;
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
            var mps = playerStatusUIs[status].Mps;
            mps[i].SetActive(false);
        }
    }

    /// <summary>
    /// 状態異常UIの生成を行う
    /// </summary>
    /// <param name="info">状態異常の情報</param>
    /// <param name="status">状態異常を受けたキャラクターのCharacterBaseStatus</param>
    private void StatusAbnormalityUIGenerate(StatusAbnormalityInfo info, CharacterBaseStatus status)
    {
        //同じ状態異常だったらUIの生成は行わない
        if(statusAbnormalityInfos.Any(kv => 
               kv.Key.charaStatus == status && kv.Key.statusAbnormalityType == info.statusAbnormalityType)) return;
        
        //UIの生成
        var objPos = saGeneratePositions[status];
        var obj = Instantiate(_statusAbnormalityPrefab, objPos);
        var image = obj.transform.GetChild(0).GetComponent<Image>();
        var text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var uiInfo = new StatusAbnormalityUIInfo(obj, image, text);
        uiInfo.ui = obj;
        uiInfo.image.sprite = info.saSprite;
        uiInfo.text.text = info.saDuration.ToString();
        statusAbnormalityInfos.Add(info, uiInfo);
    }

    /// <summary>
    /// 状態異常UIの更新を行う
    /// </summary>
    /// <param name="status">状態異常を受けているキャラクターのCharacterBaseStatus</param>
    /// <param name="type">状態異常のタイプ</param>
    private void StatusAbnormalityUIProgress(CharacterBaseStatus status, StatusAbnormalityType type)
    {
        //状態異常を受けているキャラと状態異常が一致したら継続ターンの更新を行う
        var sa = statusAbnormalityInfos.Keys.FirstOrDefault(kv =>
            kv.charaStatus == status && kv.statusAbnormalityType == type);
        if (sa != null && statusAbnormalityInfos.TryGetValue(sa, out var uiInfo))
        {
            sa.saDuration--;
            uiInfo.text.text = sa.saDuration.ToString();
        }
    }

    /// <summary>
    /// 状態異常UIの破棄
    /// </summary>
    /// <param name="status">状態異常を受けているキャラクターのCharacterBaseStatus</param>
    /// <param name="type">終了する状態異常のタイプ</param>
    private void StatusAbnormalityUIEnd(CharacterBaseStatus status, StatusAbnormalityType type)
    {
        //終了した状態異常が一致したら、その状態異常を削除する
        var sa = statusAbnormalityInfos.Keys.FirstOrDefault(kv =>
            kv.charaStatus == status && kv.statusAbnormalityType == type);
        if (sa != null && statusAbnormalityInfos.Remove(sa, out var uiInfo))
        {
            Destroy(uiInfo.ui);
        }
    }

    /// <summary>
    /// 必殺技UIの更新を行う
    /// </summary>
    /// <param name="status">更新があったキャラクターのCharacterBaseStatus</param>>
    /// <param name="ultCurrent">現在の必殺技ゲージ量</param>>
    /// <param name="ultMax">最大必殺技ゲージ量</param>>
    private void UltimateIncreaseOrDecrease(CharacterBaseStatus status, float ultCurrent, float ultMax)
    {
        var ui = playerStatusUIs[status];
        ui.UltimateImage.fillAmount = ultCurrent / ultMax;
    }
}

/// <summary>
/// StatusUIの情報を纏めたもの
/// </summary>
public class PlayerStatusUI
{
    /// <summary>
    /// 生成したUI
    /// </summary>
    public GameObject UIObj { get; private set; }
    
    /// <summary>
    /// キャラクターアイコン
    /// </summary>
    public Image CharaIcon {get; private set;}
    
    /// <summary>
    /// HPバー
    /// </summary>
    public Image HpBar  { get; private set; }
    /// <summary>
    /// 最大HP量のテキスト
    /// </summary>
    public TextMeshProUGUI MaxHpText {get; private set;}
    /// <summary>
    /// 現在のHP量のテキスト
    /// </summary>
    public TextMeshProUGUI CurrentHpText  { get; private set; }
    
    /// <summary>
    /// MP
    /// </summary>
    public GameObject[] Mps {get; private set;}
    
    /// <summary>
    /// 必殺技のイメージ
    /// </summary>
    public Image UltimateImage {get; private set;}
    /// <summary>
    /// 必殺技のボタン
    /// </summary>
    public Button UltimateButton {get; private set;}
    

    public PlayerStatusUI(GameObject uiObj, Image icon, Image hpBar, 
        TextMeshProUGUI maxHpText, TextMeshProUGUI currentHpText, GameObject[] mps, Image ultimateImage, Button ultimateButton)
    {
        UIObj = uiObj;
        CharaIcon = icon;
        HpBar = hpBar;
        MaxHpText = maxHpText;
        CurrentHpText = currentHpText;
        Mps = mps;
        UltimateImage = ultimateImage;
        UltimateButton = ultimateButton;
    }
}
