using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 編成可能なキャラクター一覧画面の管理
/// ・編成可能なキャラクターのアイコン表示
/// </summary>
public class FormationCharacterListDisplay : MonoBehaviour
{
    [Header("キャラクターデータ一覧"), SerializeField] private List<CharacterTitleData> _formationData;
    [Header("UI")]
    [SerializeField] private GameObject _formationCharacterListUI; // 編成可能なキャラクターアイコンの一覧UI
    [SerializeField] private GameObject _characterSelectButtonPrefab; // キャラクターアイコンのPrefab
    [SerializeField] private Transform _parentUIPosition; // キャラクターアイコンの生成場所

    private void Awake()
    {

    }

    private void Start()
    {

    }

    /// <summary>
    /// キャラクターアイコンの生成
    /// </summary>
    /// <param name="characterFormationDisplay"></param>
    public void GenerationCharacterIconListUI(CharacterFormationDisplay characterFormationDisplay)
    {
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
    }

    /// <summary>
    /// UIの表示切替
    /// </summary>
    /// <param name="isShow">true：表示　false：非表示</param>
    public void DisplaySwitch(bool isShow)
    {
        _formationCharacterListUI.SetActive(isShow);
    }
}
