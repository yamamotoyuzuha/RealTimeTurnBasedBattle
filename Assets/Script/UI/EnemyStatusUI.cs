using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusUI : MonoBehaviour
{
    [Header("参照")]
    [Header("TurnManager")]
    [SerializeField] private TurnManager _turnManager;
    [Header("UI関連")]
    [Header("ステータスUIの生成場所")]
    [SerializeField] private Transform _uiGenerationParent;
    [Header("ステータスUIのPrefab")]
    [SerializeField] private GameObject _statusUIPrefab;
    [Header("状態異常UIの生成場所")]
    [SerializeField] private Transform _statusAbnormalityParent;
    [Header("状態異常UIのPrefab")]
    [SerializeField] private GameObject _statusAbnormalityPrefab;
    
    /// <summary>
    /// EnemyステータスUIの表示、非表示
    /// true：表示　false：非表示
    /// </summary>
    public Action<bool> onEnemyStatusDisplay;
    /// <summary>
    /// 状態異常UIの表示、非表示
    /// </summary>
    public Action<bool> onStatusAbnormalityDisplay;
    
    //ステータスUI関連
    private GameObject statusUIPrefabInstance;
    private TextMeshProUGUI enemyNameText;
    private Image statusHpBar;
    private Image trunkBar;
    //状態異常UI
    private Dictionary<StatusAbnormalityInfo, StatusAbnormalityUIInfo> statusAbnormalityInfos =
        new Dictionary<StatusAbnormalityInfo, StatusAbnormalityUIInfo>();
    
    private CharacterBaseStatus characterBaseStatus;

    void Start()
    {
        StatusUIGenerate();
    }

    /// <summary>
    /// ステータスUIの生成
    /// ・Actionの登録
    /// </summary>
    private void StatusUIGenerate()
    {
        Status enemy = null;
        //Enemyを取得する
        foreach (var info in _turnManager.CharacterInfos)
        {
            if (info.Value.characterName == "Enemy")
            {
                enemy = info.Value.status;
                characterBaseStatus = enemy.GetCharacterStatus();
                break;
            }
        }
        
        //UIの生成
        statusUIPrefabInstance = Instantiate(_statusUIPrefab, _uiGenerationParent);
        enemyNameText = statusUIPrefabInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        enemyNameText.text = enemy != null ? enemy.GetData().CharacterName : "";
        statusHpBar = statusUIPrefabInstance.transform.GetChild(1).GetComponent<Image>();
        trunkBar = statusUIPrefabInstance.transform.GetChild(4).GetComponent<Image>();
        //statusUIPrefabInstance.SetActive(false);
        
        characterBaseStatus.onHpChanged += HpChangeUIUpdate;
        characterBaseStatus.onCoreGaugeChanged += CoreGaugeChangeUIUpdate;
        onEnemyStatusDisplay += StatusUIDisplay;

        characterBaseStatus.onStatusAbnormalityOccurrence += StatusAilmentUIGenerate;
        characterBaseStatus.onStatusAbnormalityProgress += StatusAbnormalityUIProgress;
        characterBaseStatus.onStatusAbnormalityEnd += StatusAbnormalityUIEnd;
        onEnemyStatusDisplay += StatusAbnormalityUIDisplay;
    }

    /// <summary>
    /// HPに変動があった時にUIに反映を行う
    /// </summary>
    /// <param name="status">CharacterBaseStatus</param>>
    /// <param name="currentHp">現在のHP</param>
    /// <param name="maxHp">最大HP</param>
    private void HpChangeUIUpdate(CharacterBaseStatus status, float currentHp, float maxHp)
    {
        //TODO：出来れば、DOTweenでUIのアニメーションをする
        statusHpBar.fillAmount = currentHp / maxHp;
    }

    /// <summary>
    /// EnemyステータスUIの表示切り替え
    /// </summary>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void StatusUIDisplay(bool isDisplay)
    {
        statusUIPrefabInstance.SetActive(isDisplay);
    }
    
    /// <summary>
    /// 状態異常UIの生成
    /// </summary>
    private void StatusAilmentUIGenerate(StatusAbnormalityInfo info, CharacterBaseStatus status)
    {
        //同じ状態異常だったらUIの生成は行わない
        if(statusAbnormalityInfos
           .Any(x => x.Key.statusAbnormalityType == info.statusAbnormalityType)) return;
        
        var obj = Instantiate(_statusAbnormalityPrefab, _statusAbnormalityParent);
        var image = obj.transform.GetChild(0).GetComponent<Image>();
        var text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var uiInfo = new StatusAbnormalityUIInfo(obj, image, text);
        uiInfo.ui = obj;
        uiInfo.image.sprite = info.saSprite;
        uiInfo.text.text = info.saDuration.ToString();
        statusAbnormalityInfos.Add(info, uiInfo);
    }
    /// <summary>
    /// 状態異常UIの更新
    /// </summary>
    /// <param name="status">状態異常中のキャラクター</param>
    /// <param name="type">状態異常の種類</param>
    private void StatusAbnormalityUIProgress(CharacterBaseStatus status, StatusAbnormalityType type)
    {
        //状態異常が一致したら継続ターンの更新を行う
        var sa = statusAbnormalityInfos.Keys.FirstOrDefault(kv => kv.statusAbnormalityType == type);
        if (sa != null && statusAbnormalityInfos.TryGetValue(sa, out var uiInfo))
        {
            sa.saDuration--;
            uiInfo.text.text = sa.saDuration.ToString();
        }
    }
    /// <summary>
    /// 状態異常が終了
    /// </summary>
    /// <param name="status">状態異常が終了したキャラクター</param>
    /// <param name="type">終了した状態異常</param>
    private void StatusAbnormalityUIEnd(CharacterBaseStatus status, StatusAbnormalityType type)
    {
        //終了した状態異常が一致したら、その状態異常を削除する
        var sa = statusAbnormalityInfos.Keys.FirstOrDefault(kv => kv.statusAbnormalityType == type);
        if (sa != null && statusAbnormalityInfos.Remove(sa, out var uiInfo))
        {
            Destroy(uiInfo.ui);
        }
    }

    /// <summary>
    /// 状態異常UIの表示、非表示
    /// </summary>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void StatusAbnormalityUIDisplay(bool isDisplay)
    {
        foreach (var info in statusAbnormalityInfos)
        {
            info.Value.ui.SetActive(isDisplay);
        }
    }

    /// <summary>
    /// 体幹ゲージ量の変動
    /// </summary>
    /// <param name="currentTrunk">現在の体幹量</param>>
    /// <param name="maxTrunk">最大の体幹量</param>>
    private void CoreGaugeChangeUIUpdate(float currentTrunk, float maxTrunk)
    {
        trunkBar.fillAmount = currentTrunk / maxTrunk;
    }
}
