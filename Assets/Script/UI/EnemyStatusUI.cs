using System;
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
    /*
    [Header("状態異常UIの生成場所")]
    [SerializeField] private Transform _statusAilmentParent;
    [Header("状態異常UIのPrefab")]
    [SerializeField] private GameObject _statusAilmentPrefab;
    */
    /// <summary>
    /// EnemyステータスUIの表示、非表示
    /// true：表示　false：非表示
    /// </summary>
    public Action<bool> onEnemyStatusDisplay;
    
    //ステータスUI関連
    private GameObject statusUIPrefabInstance;
    private TextMeshProUGUI enemyNameText;
    private Image statusHpBar;

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
        //statusUIPrefabInstance.SetActive(false);
        
        characterBaseStatus.onHpChanged += HpChangeUIUpdate;
        onEnemyStatusDisplay += StatusUIDisplay;
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
    /// 状態異常になった時にUIに反映を行う
    /// </summary>
    private void StatusAilmentUIUpdate()
    {
        //TODO：状態異常のアイコンを生成
        
        //TODO：現状の状態異常にあう、アイコンをSpritにいれる
    }

    /// <summary>
    /// EnemyステータスUIの表示切り替え
    /// </summary>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void StatusUIDisplay(bool isDisplay)
    {
        statusUIPrefabInstance.SetActive(isDisplay);
    }
}
