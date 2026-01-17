using UnityEngine;

/// <summary>
/// バトル中のUI管理を行う
/// </summary>
public class BattleOperatingInstructionsUI : MonoBehaviour
{
    public static BattleOperatingInstructionsUI Instance { get; private set; }
    
    [Header("行動選択UI")] 
    [SerializeField] private GameObject _actionSelectionUI;
    [Header("コマンド選択UI")]
    [SerializeField] private GameObject _commandSelectionUI;
    [Header("コマンド確定UI")]
    [SerializeField] private GameObject _commandConfirmedUI;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    
    /// <summary>
    /// 行動選択UIの表示
    /// </summary>
    public void ActionSelectionUI()
    {
        
    }

    /// <summary>
    /// コマンド選択UIの表示
    /// </summary>
    public void CommandSelectionUI()
    {
        
    }

    /// <summary>
    /// コマンド確定UIの表示
    /// </summary>
    public void CommandConfirmedUI()
    {
        
    }
}
