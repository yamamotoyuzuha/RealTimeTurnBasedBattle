using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusUI : MonoBehaviour
{
    [Header("UI関連")]
    [Header("UIの生成場所")]
    [SerializeField] private Transform _uiGenerationParent;
    [Header("ステータスUIのPrefab")]
    [SerializeField] private GameObject _statusUIPrefab;
    [Header("状態異常UIの生成場所")]
    [SerializeField] private Transform _statusAilmentParent;
    [Header("状態異常UIのPrefab")]
    [SerializeField] private GameObject _statusAilmentPrefab;
    
    //ステータスUI関連
    private GameObject statusUIPrefabInstance;
    private TextMeshProUGUI enemyNameText;
    private Image statusHpBar;

    private CharacterBaseStatus characterBaseStatus;
    
    void Start()
    {
        //ステータスを取得する
        characterBaseStatus = GetComponent<Status>().GetCharacterStatus();
        
        //生成してUI関連の情報を取得したのちに非表示にしておく
        statusUIPrefabInstance = Instantiate(_statusUIPrefab, _uiGenerationParent);
        enemyNameText = statusUIPrefabInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        statusHpBar = statusUIPrefabInstance.transform.GetChild(1).GetComponent<Image>();
        statusUIPrefabInstance.SetActive(false);
        
        //Actionの登録を行う
        characterBaseStatus.onHpChanged += HpChangeUIUpdate;
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
}
