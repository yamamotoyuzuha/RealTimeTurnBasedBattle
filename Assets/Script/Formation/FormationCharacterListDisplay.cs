using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 編成可能なキャラクター一覧画面の管理
/// ・編成可能なキャラクターのアイコン表示
/// </summary>
public class FormationCharacterListDisplay : MonoBehaviour
{
    [Header("キャラクターデータ一覧"), SerializeField]
    private List<CharacterTitleData> _formationData;

    [Header("UI")] [SerializeField] private GameObject _formationCharacterListUI; // 編成可能なキャラクターアイコンの一覧UI
    [SerializeField] private GameObject _characterSelectButtonPrefab; // キャラクターアイコンのPrefab
    [SerializeField] private Transform _parentUIPosition; // キャラクターアイコンの生成場所


    // TODO：これは毎回UIを生成する必要がある
    // TODO：編成可能なキャラクターアイコンUIの生成をし、初期化を行う
    // TODO：生成したUIはスクロールバーの子オブジェクトにする
    // TODO：この編成可能とかいらないかも、全部表示でいいのではないのだろうか


    private void Awake()
    {

    }

    private void Start()
    {

    }
    
    /// <summary>
    /// キャラクターアイコンの削除
    /// </summary>
    private void ClearCharacterIconListUI()
    {
        foreach (Transform parent in _parentUIPosition.transform)
        {
            Destroy(parent.gameObject);
        }
    }

    /// <summary>
    /// UIの表示を行う
    /// ・キャラクターアイコンの生成
    /// </summary>
    public void Show(CharacterFormationDisplay characterFormationDisplay)
    {
        ClearCharacterIconListUI();

        // UIの生成
        foreach (var character in _formationData)
        {
            // 生成
            var obj = Instantiate(_characterSelectButtonPrefab, _parentUIPosition.transform);
            // UIの初期化を行う
            var info = obj.GetComponent<FormationCharacterIconInfo>();
            var selectButton = obj.GetComponent<CharacterSelectButton>();
            info.Initialize(character);
            selectButton.Initialize(character, characterFormationDisplay);
        }

        _formationCharacterListUI.SetActive(true);
    }

    /// <summary>
    /// UIの非表示を行う
    /// </summary>
    public void Hide()
    {
        _formationCharacterListUI.SetActive(false);
    }
}
