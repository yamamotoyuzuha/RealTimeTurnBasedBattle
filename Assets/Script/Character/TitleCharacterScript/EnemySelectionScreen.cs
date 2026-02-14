using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトル画面で行うEnemyの選択
/// </summary>
public class EnemySelectionScreen : MonoBehaviour
{
    [Header("TitleInputManger")]
    [SerializeField] private TitleInputManager _titleInputManager;
    [Header("EnemySelectionScreenUIController")] 
    [SerializeField] private EnemySelectionScreenUIController _selectionScreenUIController;
    [Header("TitleUIManager")]
    [SerializeField] private TitleUIManager _titleUIManager;
    [Header("Enemy一覧")] 
    [SerializeField] private List<CharacterBaseData> _allEnemy;
    [Header("選択したEnemy")] 
    [SerializeField] private CharacterBaseData _selectEnemy;
    /// <summary>
    /// 現在、選択しているEnemy
    /// </summary>
    private CharacterBaseData currentEnemy;
    /// <summary>
    /// Enemy選択画面の表示、非表示
    /// true：表示　false：非表示
    /// </summary>
    public bool IsSelectDisplay { get; private set; }
    private int currentIndex;

    void Start()
    {
        //UIなどの生成
        foreach (var enemy in _allEnemy)
        {
            _selectionScreenUIController.onEnemyUIGenerate?.Invoke(enemy);
            _selectionScreenUIController.onSelectionEnemyGenerate?.Invoke(enemy, _allEnemy.IndexOf(enemy));
        }
        //初期設定
        currentIndex = 0;
        _selectEnemy = _allEnemy[0];
        currentEnemy = _selectEnemy;
        SelectedChangeEnemy(0);
        SelectEnemy(currentEnemy);
        CombatInformationManager.Instance.AddCombatInfoEnemy(_selectEnemy);
    }

    void Update()
    {
        SelectScreenDisplayToggle();
        if(!IsSelectDisplay) return;
        if (_titleInputManager.TitleInput.Player.SelectLeft.triggered)
        {
            SelectedChangeEnemy(-1);
        }
        if (_titleInputManager.TitleInput.Player.SelectRight.triggered)
        {
            SelectedChangeEnemy(1);
        }

        if (_titleInputManager.TitleInput.Player.Add.triggered) //Enemyの選択
        {
            SelectEnemy(currentEnemy);
        }
    }

    private void SelectScreenDisplayToggle()
    {
        if (_titleInputManager.TitleInput.Player.EnemySelectDisplay.triggered)
        {
            if (!IsSelectDisplay) //表示
            {
                IsSelectDisplay = true;
                _selectionScreenUIController.onEnemySelectionShowOrHidden?.Invoke(true);
                _titleInputManager.SetGame(false);
                _titleUIManager.onEsShow?.Invoke();
            }
            else //非表示
            {
                IsSelectDisplay = false;
                _selectionScreenUIController.onEnemySelectionShowOrHidden?.Invoke(false);
                _titleInputManager.SetGame(true);
                _titleUIManager.onTitleShow?.Invoke();
                
                //非表示にしたタイミングでEnemyの確定
                CombatInformationManager.Instance.AddCombatInfoEnemy(_selectEnemy);
            }
        }
    }

    /// <summary>
    /// Enemyの選択を切り替える
    /// </summary>
    /// <param name="index"></param>
    private void SelectedChangeEnemy(int index)
    {
        currentIndex = (currentIndex + index + _allEnemy.Count) % _allEnemy.Count;
        currentEnemy = _allEnemy[currentIndex];
        _selectionScreenUIController.onChangeEnemy?.Invoke(currentEnemy);
    }

    /// <summary>
    /// 戦闘を行うEnemyの戦闘
    /// </summary>
    /// <param name="data">選択したEnemy</param>>
    private void SelectEnemy(CharacterBaseData data)
    {
        _selectEnemy = data;
        _selectionScreenUIController.onSelectEnemy?.Invoke(_selectEnemy);
    }
}
