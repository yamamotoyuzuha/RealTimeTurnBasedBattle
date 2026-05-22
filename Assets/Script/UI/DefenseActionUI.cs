using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// 防御アクションUIの管理
/// </summary>
public class DefenseActionUI : MonoBehaviour
{
    [Header("UI生成場所")]
    [SerializeField] private Transform _uiParent;
    [Header("防御アクションUIのPrefab")]
    [SerializeField] private GameObject _defenseUIPrefab;
    [Header("MpUIのPrefab")]
    [SerializeField] private GameObject _successMpUIPrefab;
    //UI
    private GameObject defenseUIPrefabInstance;
    private GameObject successMpUIPrefabInstance;
    private TextMeshProUGUI defenseActionText;
    private TextMeshProUGUI successMpCountText;
    //プレイヤーキャラ
    private Character _character;

    void Start()
    {
        _character = transform.root.GetComponent<Status>().GetCharacter();
        DefenseActionUIGenerate();
        _character.EventsSystem.onParrySuccess += ParryUI;
        _character.EventsSystem.onJustGuardSuccess += JustGuardUI;
    }

    /// <summary>
    /// 防御アクションUIの生成とUIの情報を取得
    /// </summary>
    private void DefenseActionUIGenerate()
    {
        defenseUIPrefabInstance = Instantiate(_defenseUIPrefab, _uiParent);
        successMpUIPrefabInstance = Instantiate(_successMpUIPrefab, _uiParent);
        defenseActionText = defenseUIPrefabInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        successMpCountText = successMpUIPrefabInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        defenseUIPrefabInstance.SetActive(false);
        successMpUIPrefabInstance.SetActive(false);
    }

    /// <summary>
    /// パリィが成功した時にUIを表示する
    /// </summary>
    private void ParryUI(int mp)
    {
        defenseActionText.text = "Parry";
        successMpCountText.text = mp.ToString();
        defenseUIPrefabInstance.SetActive(true);
        successMpUIPrefabInstance.SetActive(true);
        HiddenUI(defenseUIPrefabInstance, 0.4f).Forget();
        HiddenUI(successMpUIPrefabInstance, 0.4f).Forget();
    }
    
    /// <summary>
    /// ジャストガードが成功した時にUIを表示する
    /// </summary>
    private void JustGuardUI()
    {
        defenseActionText.text = "Just Guard";
        defenseUIPrefabInstance.SetActive(true);
        HiddenUI(defenseUIPrefabInstance, 0.4f).Forget();
    }

    /// <summary>
    /// UIの非表示を行う
    /// </summary>
    /// <param name="ui">アニメーションを行うUI</param>>
    /// <param name="duration">アニメーション時間</param>>
    private async UniTask HiddenUI(GameObject ui, float duration)
    {
        await ui.transform.DOScale(
            new Vector3(1.5f, 1.5f, 1.5f), duration).AsyncWaitForCompletion();
        ui.SetActive(false);
    }
}
