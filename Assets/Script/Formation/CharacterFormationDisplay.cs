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
    [SerializeField] private SelectCharacterModelDisplay _selectedCharacterModelDisplay;
    [SerializeField] private FormationCharacterListDisplay _formationCharacterListDisplay;
    [SerializeField] private FormationSlotDragController _formationSlotDragController;
    [Header("最大編成メンバー数"), SerializeField] private int _maximumFormation;
    [Header("現在の編成"), SerializeField] private List<CharacterTitleData> _currentFormationData;
    [Header("UI")]
    [SerializeField] private GameObject _formationButtonUI;          // 編成スロットのUI
    [SerializeField] private GameObject _formationSlotButtonPrefab;  // スロットのPrefab
    private FormationSlotButton[] _formationSlotButtons; // 編成スロットUIの保持
    private int _currentSelectIndex = -1; // 現在、選択している編成枠
    
    
    
    // TODO：ドラッグ＆ドロップについて
    //       実装の感じを考えておく。編成枠でクリックの長押しが確認されたら、その長押しされたところにあったデータを保持する
    // TODO：ドラッグ＆ドロップの実装を行う
    
    
    
    private void Awake()
    {
        
    }

    private void Start()
    {
        Initialize();
        GenerationFormationModel();
        GenerationFormationSlotUI();
        _formationCharacterListDisplay.GenerationCharacterIconListUI(this);
    }

    private void Update()
    {
        // TODO：これは仮のデバッグです
        // TODO：おわったら消す
        if (Input.GetKeyDown(KeyCode.A))
        {
            ConfirmCharacterSelection();
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        _formationSlotDragController.OnFormationSlotSwapRequested +=
            SwapFormation;
    }
    
    /// <summary>
    /// 編成枠が選択されたとき
    /// </summary>
    public void OnFormationSlotSelected(int selectIndex)
    {
        _formationCharacterListDisplay.DisplaySwitch(true);
        _formationButtonUI.SetActive(false);
        _currentSelectIndex = selectIndex;
        
        // 選択画面に現在の編成キャラクターを表示
        _selectedCharacterModelDisplay.DisplaySelectCharacterModel(_currentFormationData[selectIndex]);
        _formationCharacterModelDisplay.DisplaySwitch(false);
        _selectedCharacterModelDisplay.DisplaySwitch(true);
    }
    
    /// <summary>
    /// 選択画面のモデルを変更する
    /// </summary>
    /// <param name="selectedCharacterTitleData">選択したキャラクター</param>
    public void OnCharacterSelected(CharacterTitleData selectedCharacterTitleData)
    {
        // 選択画面のプレビューモデルだけを変更
        _selectedCharacterModelDisplay.DisplaySelectCharacterModel(selectedCharacterTitleData);
    }

    /// <summary>
    /// キャラクター選択を終了する
    /// </summary>
    public void ConfirmCharacterSelection()
    {
        if (_currentSelectIndex < 0) return;
        
        UpdateFormationCharacterData();
        ReturnFormationDisplay();
        _currentSelectIndex = -1;
    }

    /// <summary>
    /// 編成キャラクターのデータを更新する
    /// </summary>
    private void UpdateFormationCharacterData()
    {
        var selectedData = _selectedCharacterModelDisplay.CurrentSelectModel;
        if(selectedData == null) return;
        
        _currentFormationData[_currentSelectIndex] = selectedData;
        _formationSlotButtons[_currentSelectIndex].SetCharacterTitleData(selectedData);
        _formationCharacterModelDisplay.DisplayFormationCharacterModel(selectedData, _currentSelectIndex);
    }
    
    /// <summary>
    /// 編成画面に戻る際に行う処理
    /// </summary>
    private void ReturnFormationDisplay()
    {
        // 選択画面を非表示にし、編成画面に戻す
        _formationCharacterListDisplay.DisplaySwitch(false);
        _formationButtonUI.SetActive(true);
        _formationCharacterModelDisplay.DisplaySwitch(true);
        _selectedCharacterModelDisplay.DisplaySwitch(false);
    }

    /// <summary>
    /// 編成枠のキャラクターを入れ替える
    /// ・入れ替えが行われたことによる変更を行う
    /// </summary>
    /// <param name="dragIndex">ドラッグした編成枠のインデックス</param>
    /// <param name="dropIndex">ドロップした編成枠のインデックス</param>
    private void SwapFormation(int dragIndex, int dropIndex)
    {
        // 編成データの入れ替え
        (_currentFormationData[dragIndex], _currentFormationData[dropIndex]) 
            = (_currentFormationData[dropIndex], _currentFormationData[dragIndex]);
        
        // 編成枠のデータの入れ替え
        _formationSlotButtons[dragIndex].SetCharacterTitleData(_currentFormationData[dragIndex]);
        _formationSlotButtons[dropIndex].SetCharacterTitleData(_currentFormationData[dropIndex]);
        
        // キャラクターモデルの更新
        _formationCharacterModelDisplay.DisplayFormationCharacterModel(_currentFormationData[dragIndex], dragIndex);
        _formationCharacterModelDisplay.DisplayFormationCharacterModel(_currentFormationData[dropIndex], dropIndex);
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
            _formationSlotButtons[i] = slotButton;
            
            ActionRegister(slotButton);
        }
    }

    private void ActionRegister(FormationSlotButton slotButton)
    {
        // 編成枠の選択
        slotButton.OnSlotSelected += OnFormationSlotSelected;
        // 編成枠のドラッグ&ドロップ
        var dc = _formationSlotDragController;
        slotButton.OnSlotDragging += dc.Dragging;
        slotButton.OnSlotSwapBegin += dc.BeginDrag;
        slotButton.OnSlotSwapEnd += dc.EndDrag;
        slotButton.OnSlotDrop += dc.Drop;
    }
}
