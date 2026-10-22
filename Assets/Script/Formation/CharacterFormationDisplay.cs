using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの編成画面の管理
/// ・編成状態の変更
/// ・編成しているキャラクターの表示（モデル、名前等）
/// </summary>
public class CharacterFormationDisplay : MonoBehaviour
{
    [SerializeField] private FormationCharacterModelDisplay _formationCharacterModelDisplay;
    [SerializeField] private FormationCharacterListDisplay _formationCharacterListDisplay;
    [Header("最大編成メンバー数"), SerializeField] private int _maximumFormation;
    [Header("現在の編成"), SerializeField] private List<CharacterTitleData> _currentFormationData;
    [Header("UI")]
    [SerializeField] private GameObject _formationButtonUI;          // 編成スロットのUI
    [SerializeField] private GameObject _formationSlotButtonPrefab;  // スロットのPrefab
    
   
    
    // TODO：これをやる
    // TODO：編成しているキャラクターが生成されていることを確認した
    // TODO：そしたら、選択画面にいったら、編成画面のキャラクターは非表示にする
    // TODO：選択画面で入れ替えたら、キャラクターモデルが切り替わることを確認する
    

    
    private void Awake()
    {
        
    }

    private void Start()
    {
        GenerationFormationModel();
        GenerationFormationSlotUI();
    }

    private void Update()
    {
        
    }
    
    /// <summary>
    /// 編成枠が選択されたとき
    /// </summary>
    public void OnFormationSlotSelected()
    {
        _formationCharacterListDisplay.Show(this);
        _formationButtonUI.SetActive(false);
        
        // TODO：ここで選択されたキャラクターを生成し、表示を行う（別のスクリプトから呼び出し）
    }

    /// <summary>
    /// キャラクター選択が選択されたとき
    /// </summary>
    public void OnCharacterSelectButtonSelected()
    {
        _formationCharacterListDisplay.Hide();
        _formationButtonUI.SetActive(true);
        
        // TODO：ここも
    }

    /// <summary>
    /// モデルの生成
    /// </summary>
    private void GenerationFormationModel()
    {
        // モデルを生成
        for (int i = 0; i < _currentFormationData.Count; i++)
        {
            var data = _currentFormationData[i];
            _formationCharacterModelDisplay.DisplayFormationCharacterModel(data, i);
        }
    }

    /// <summary>
    /// UIの生成
    /// </summary>
    private void GenerationFormationSlotUI() 
    {
        // UIを生成して、データの設定を行う
        for (int i = 0; i < _currentFormationData.Count; i++)
        {
            var data = _currentFormationData[i];
            var obj = Instantiate(_formationSlotButtonPrefab, _formationButtonUI.transform);
            obj.GetComponent<FormationSlotButton>().SetCharacterTitleData(data, this, i);
        }
    }
}
