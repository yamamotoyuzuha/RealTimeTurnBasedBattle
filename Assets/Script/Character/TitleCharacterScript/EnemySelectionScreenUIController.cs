using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Enemy選択画面のUI管理
/// </summary>
public class EnemySelectionScreenUIController : MonoBehaviour
{
    [Header("Enemy選択画面UI")]
    [SerializeField] private GameObject _selectionUI;
    [Header("EnemyUIの生成場所")]
    [SerializeField] private Transform _enemyUIParent;
    [Header("EnemyUIのPrefab")] 
    [SerializeField] private GameObject _enemyUIPrefab;
    [Header("EnemyPrefabの生成場所")] 
    [SerializeField] private Transform[] _enemyObjParents;
    [Header("Enemy選択画面の操作説明UI")] 
    [SerializeField] private GameObject _explanationUI;
    /// <summary>
    /// EnemyUIの保持
    /// </summary>
    private List<ESelectionUIInfo> eSelectionUIInfos = new List<ESelectionUIInfo>();
    /// <summary>
    /// EnemyPrefabの保持
    /// </summary>
    private List<ESelectionInfo> eSelectionInfos = new List<ESelectionInfo>();

    #region Action
    /// <summary>
    /// EnemyUIを生成
    /// </summary>
    public Action<CharacterBaseData> onEnemyUIGenerate;
    /// <summary>
    /// EnemyPrefabの生成
    /// </summary>
    public Action<CharacterBaseData, int> onSelectionEnemyGenerate;
    /// <summary>
    /// 選択しているEnemyの変更
    /// </summary>
    public Action<CharacterBaseData> onChangeEnemy;
    /// <summary>
    /// 選択しているEnemyを決定
    /// </summary>
    public Action<CharacterBaseData> onSelectEnemy;
    /// <summary>
    /// Enemy選択画面の表示、非表示
    /// true；表示　false：非表示
    /// </summary>
    public Action<bool> onEnemySelectionShowOrHidden;
    #endregion

    void Awake()
    {
        onEnemyUIGenerate += EnemyUIGenerate;
        onSelectionEnemyGenerate += EnemyPrefabGenerate;
        onChangeEnemy += ChangeEnemyUI;
        onSelectEnemy += SelectEnemy;
        onEnemySelectionShowOrHidden += SelectionScreenUIDisplay;
    }

    /// <summary>
    /// EnemyUIの生成
    /// </summary>
    /// <param name="characterBaseData">生成するEnemyデータ</param>
    private void EnemyUIGenerate(CharacterBaseData characterBaseData)
    {
        var obj = Instantiate(_enemyUIPrefab, _enemyUIParent);
        var image = obj.transform.GetChild(0).GetComponent<Image>();
        var text = obj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        var info = new ESelectionUIInfo()
        {
            enemyData = characterBaseData,
            eSelectionUI = obj,
            enemySelectBg = image,
            enemyName = text
        };
        eSelectionUIInfos.Add(info);
        info.SetUp();
        info.eSelectionUI.SetActive(false);
    }

    /// <summary>
    /// EnemyPrefabの生成
    /// </summary>
    /// <param name="characterBaseData">生成するEnemyデータ</param>
    /// <param name="index">生成する場所のインデックス</param>>
    private void EnemyPrefabGenerate(CharacterBaseData characterBaseData, int index)
    {
        var obj = Instantiate(characterBaseData.TitleCharacterPrefab, _enemyObjParents[index]);
        var info = new ESelectionInfo()
        {
            enemyData = characterBaseData,
            enemyObj = obj
        };
        eSelectionInfos.Add(info);
        info.enemyObj.SetActive(false);
    }
    
    /// <summary>
    /// Enemyの選択が移動したときにUIに変更を行う
    /// </summary>
    /// <param name="characterBaseData"></param>
    private void ChangeEnemyUI(CharacterBaseData characterBaseData)
    {
        //一致したキャラクターデータのUIに変更を加える
        foreach (var info in eSelectionUIInfos)
        {
            if (info.enemyData == characterBaseData)
            {
                info.eSelectionUI.transform.DOScale(
                    new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
            }
            else
            {
                info.eSelectionUI.transform.DOScale(
                    new Vector3(1f, 1f, 1f), 0.2f);
            }
        }
    }

    /// <summary>
    /// Enemyが選択されたときにUIに変更を行う
    /// </summary>
    /// <param name="characterBaseData">選択したEnemyデータ</param>
    private void SelectEnemy(CharacterBaseData characterBaseData)
    {
        //選択されたEnemyのみUIを変更
        foreach (var info in eSelectionUIInfos)
        {
            if (info.enemyData == characterBaseData)
            {
                info.enemySelectBg.color = Color.gray;
            }
            else
            {
                info.enemySelectBg.color = Color.black;
            }
        }
    }

    /// <summary>
    /// 選択画面の表示、非表示を行う
    /// </summary>
    /// <param name="isDisplay">true：表示　false：非表示</param>
    private void SelectionScreenUIDisplay(bool isDisplay)
    {
        foreach (var ui in eSelectionUIInfos)
        {
            ui.eSelectionUI.SetActive(isDisplay);
        }

        foreach (var obj in eSelectionInfos)
        {
            obj.enemyObj.SetActive(isDisplay);
        }
    }
}

/// <summary>
/// EnemyUIの情報
/// </summary>
public class ESelectionUIInfo
{
    public CharacterBaseData enemyData;
    public GameObject eSelectionUI;
    public Image enemySelectBg;
    public TextMeshProUGUI enemyName;

    /// <summary>
    /// UIの設定を行う
    /// </summary>
    public void SetUp()
    {
        enemyName.text = enemyData.CharacterName;
    }
    
    //TODO：これに弱点属性の画像を表示する
}

/// <summary>
/// 選択画面に表示するEnemy
/// </summary>
public class ESelectionInfo
{
    public CharacterBaseData enemyData;
    public GameObject enemyObj;
}
