using UnityEngine;

/// <summary>
/// タイトル画面のUIを管理する
/// </summary>
public class TitleSceneUIManager : MonoBehaviour
{
    /// <summary>
    /// タイトル画面の遷移状態
    /// </summary>
    enum TitleSceneState
    {
        Home,
        Formation
    }
    
    [SerializeField] private TitleInputManager _titleInputManager;
    [SerializeField] private CharacterFormationDisplay _characterFormationDisplay;
    [Header("タイトルUI")] [SerializeField] private GameObject _titleUIObj;

    /// <summary>
    /// 現在のタイトル画面の遷移状態
    /// </summary>
    private TitleSceneState _currentTitleSceneState;

    private void Awake()
    {
        _characterFormationDisplay.DisplayFormation(false);
        _currentTitleSceneState = TitleSceneState.Home;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        ToggleDisplay();
    }

    /// <summary>
    /// タイトル画面の表示切り替え
    /// </summary>
    private void ToggleDisplay()
    {
        if (_titleInputManager.TitleInput.Player.FormationDisplay.triggered)
        {
            switch (_currentTitleSceneState)
            {
                case TitleSceneState.Home:
                    _titleUIObj.SetActive(false);
                    _characterFormationDisplay.DisplayFormation(true);
                    _currentTitleSceneState = TitleSceneState.Formation;
                    break;
            
                case TitleSceneState.Formation:
                    _titleUIObj.SetActive(true);
                    _characterFormationDisplay.DisplayFormation(false);
                    _currentTitleSceneState = TitleSceneState.Home;
                    break;
            }
        }
    }
}
