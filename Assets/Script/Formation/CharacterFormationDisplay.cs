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
    private FormationSlotButton[] _formationSlotButtons; // 編成スロットUIの保持
    private int _currentSelectIndex = -1; // 現在、選択している編成枠
    
    // TODO：これをやる
    // TODO：編成しているキャラクターが生成されていることを確認した
    // TODO：そしたら、選択画面にいったら、編成画面のキャラクターは非表示にする
    // TODO：選択画面で入れ替えたら、キャラクターモデルが切り替わることを確認する
    

    // TODO：ドラッグ＆ドロップについて
    //       実装の感じを考えておく。編成枠でクリックの長押しが確認されたら、その長押しされたところにあったデータを保持する
    
    
    
    
    private void Awake()
    {
        
    }

    private void Start()
    {
        GenerationFormationModel();
        GenerationFormationSlotUI();
        _formationCharacterListDisplay.GenerationCharacterIconListUI(this);
    }

    private void Update()
    {
        
    }
    
    /// <summary>
    /// 編成枠が選択されたとき
    /// </summary>
    public void OnFormationSlotSelected(int selectIndex)
    {
        _formationCharacterListDisplay.DisplaySwitch(true);
        _formationButtonUI.SetActive(false);
        _currentSelectIndex = selectIndex;
        
        // TODO：ここで選択されたキャラクターを生成し、表示を行う（別のスクリプトから呼び出し）
        
    }

    /// <summary>
    /// キャラクター選択が選択されたとき
    /// </summary>
    public void OnCharacterSelectButtonSelected()
    {
        _formationCharacterListDisplay.DisplaySwitch(false);
        _formationButtonUI.SetActive(true);
        
        // TODO：ここも
    }

    /// <summary>
    /// 選択したキャラクターを現在の編成へと設定する
    /// </summary>
    public void SelectCharacterFormationSettings(CharacterTitleData selectedCharacterTitleData)
    {
        // 選択している編成枠がない場合、処理を行わない
        if(_currentSelectIndex < 0) return;
        
        // 選択している編成枠に選択したキャラクターを設定する
        _currentFormationData[_currentSelectIndex] = selectedCharacterTitleData;
        _formationSlotButtons[_currentSelectIndex].SetCharacterTitleData(selectedCharacterTitleData);
        
        // 現在の表示しているモデルを切り替える
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
        var count = _currentFormationData.Count;
        _formationSlotButtons = new FormationSlotButton[count];
        // UIを生成して、データの設定を行う
        for (int i = 0; i < _currentFormationData.Count; i++)
        {
            var data = _currentFormationData[i];
            var obj = Instantiate(_formationSlotButtonPrefab, _formationButtonUI.transform);
            // 編成枠の設定
            var slotButton = obj.GetComponent<FormationSlotButton>();
            slotButton.Initialize(data, i);
            slotButton.OnSlotSelected += OnFormationSlotSelected;
            _formationSlotButtons[i] = slotButton;
        }
    }
}
