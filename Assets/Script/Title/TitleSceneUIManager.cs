using UnityEngine;

/// <summary>
/// タイトル画面のUIを管理する
/// </summary>
public class TitleSceneUIManager : MonoBehaviour
{
    [SerializeField] private CharacterFormationDisplay _characterFormationDisplay;
    [Header("タイトルのUI")] [SerializeField] private GameObject _titleUIObj;
    [Header("編成画面のUI")] [SerializeField] private GameObject _formationUIObj;

    /// <summary>
    /// 現在のタイトル画面の遷移状態
    /// </summary>
    private TitleSceneState _currentTitleSceneState;

    private void Awake()
    {
        _characterFormationDisplay.DisplayFormation(false);
        _currentTitleSceneState = TitleSceneState.Home;
    }

    /// <summary>
    /// タイトル画面の表示切り替え
    /// </summary>
    public void ToggleDisplay()
    {
        switch (_currentTitleSceneState)
        {
            case TitleSceneState.Home:
                _titleUIObj.SetActive(false);
                _formationUIObj.SetActive(true);
                _characterFormationDisplay.DisplayFormation(true);
                _currentTitleSceneState = TitleSceneState.Formation;
                break;
            
            case TitleSceneState.Formation:
                _titleUIObj.SetActive(true);
                _formationUIObj.SetActive(false);
                _characterFormationDisplay.DisplayFormation(false);
                _currentTitleSceneState = TitleSceneState.Home;
                break;
        }
    }
}
