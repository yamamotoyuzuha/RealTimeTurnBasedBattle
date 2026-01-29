using UnityEngine;

/// <summary>
/// 操作UIのデータを管理
/// </summary>
public class OperationDataManager : MonoBehaviour
{
    public static OperationDataManager Instance {get; private set;}
    
    [Header("操作UIのデータ")]
    [SerializeField] private OperationUIData _operationUIData;
    public OperationUIData OperationUIData => _operationUIData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
