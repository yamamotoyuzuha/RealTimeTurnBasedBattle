using UnityEngine;
using Cinemachine;

/// <summary>
/// 戦闘全体カメラ
/// ・キャラクター個別のではなく、全体のカメラを管理する
/// </summary>
public class BattleCameraAngleManager : MonoBehaviour
{
    [Header("MainCamera")]
    [SerializeField] private CinemachineBrain _mainCamera;
    [Header("通常カメラ（フィールド）")]
    [SerializeField] private CinemachineVirtualCamera _defaultFieldCam;
    [Header("通常カメラ（プレイヤー）")]
    [SerializeField] private CinemachineVirtualCamera _defaultPlayerCam;
    [Header("行動確定時のカメラ")] 
    [SerializeField] private CinemachineVirtualCamera _actionConfirmedCam;
    [Header("各コマンド決定時のカメラ")]
    [SerializeField] private CinemachineVirtualCamera _commandConfirmedCam;
    [Header("魔法パネル開始時のカメラ")] 
    [SerializeField] private CinemachineVirtualCamera _magicPanelStartCame;
    [Header("行動開始時のカメラ")] 
    [SerializeField] private CinemachineVirtualCamera _startActionCam;
    [Header("Enemyの行動カメラ")] 
    [SerializeField] private CinemachineVirtualCamera _enemyActionCam;
    [Header("防御アクションのカメラ")]
    [SerializeField] private CinemachineVirtualCamera _defenseActionCam;

    [Header("カメラ位置")]
    [Header("フィールドのカメラ位置F）")]
    [SerializeField] private Transform _fieldPosF;
    [Header("フィールドのカメラ位置L")] 
    [SerializeField] private Transform _fieldPosL;
    [Header("Enemy行動のカメラ位置F")]
    [SerializeField] private Transform _eActionPosF;
    [Header("Enemy行動のカメラ位置L")]
    [SerializeField] private Transform _eActionPosL;
    [Header("防御アクションのカメラ位置F")]
    [SerializeField] private Transform _dActionPosF;
    [Header("防御アクションのカメラ位置L")]
    [SerializeField] private Transform _dActionPosL;

    #region Follow
    public Transform FieldPosF => _fieldPosF;
    public Transform EActionPosF => _eActionPosF;
    public Transform DActionPosF => _dActionPosF;
    #endregion
    #region LookAt
    public Transform FieldPosL => _fieldPosL;
    public Transform EActionPosL => _eActionPosL;
    public Transform DActionPosL => _dActionPosL;
    #endregion
    
    //シネマシーンのPriority設定
    private int priorityActive = 20;
    private int priorityNonActive = 10;
    
    //TODO：カスタムブレンドを使用しない場合
    //TODO：InGameSceneにVirtualCameraを必要な分だけ配置
    //TODO：その役割に応じたVirtualCameraのLookAtとFollowに各キャラのPrefabについてるスクリプトから代入の形式
    //TODO:各キャラのPrefabには、カメラがくるであろう位置に空のゲームオブジェクトを配置する
    
    void Awake()
    {
        BattleCameraAngleChange(BattleCameraActiveType.DefaultField, FieldPosF, FieldPosL);
    }

    /// <summary>
    /// カメラの切り替え
    /// </summary>
    /// <param name="cameraActiveType">切り替えたいアングルの種類</param>
    /// <param name="posF">カメラの位置</param>>
    /// <param name="posL">カメラが見る方向</param>>
    public void BattleCameraAngleChange(BattleCameraActiveType cameraActiveType, Transform posF, Transform posL)
    {
        ResetCameraAngle();

        CinemachineVirtualCamera cam = null;
        switch (cameraActiveType)
        {
            case BattleCameraActiveType.DefaultField:
                cam = _defaultFieldCam;
                break;
            case BattleCameraActiveType.DefaultPlayer:
                cam  = _defaultPlayerCam;
                break;
            case BattleCameraActiveType.ActionConfirmed:
                cam = _actionConfirmedCam;
                break;
            case BattleCameraActiveType.CommandConfirmed:
                cam = _commandConfirmedCam;
                break;
            case BattleCameraActiveType.MagicPanelStart:
                cam = _magicPanelStartCame;
                break;
            case BattleCameraActiveType.StartAction:
                cam =  _startActionCam;
                break;
            case BattleCameraActiveType.EAction:
                cam = _enemyActionCam;
                break;
            case BattleCameraActiveType.DAction:
                cam = _defenseActionCam;
                break;
        }
        
        //シネマシーンの設定を行う
        if(cam != null)
        {
            cam.Priority = priorityActive;
            cam.Follow = posF;
            cam.LookAt = posL;
        }
    }

    /// <summary>
    /// シネマシーンのPriorityをリセット
    /// </summary>
    public void ResetCameraAngle()
    {
        _defaultFieldCam.Priority = priorityNonActive;
        _defaultPlayerCam.Priority = priorityNonActive;
        _actionConfirmedCam.Priority = priorityNonActive;
        _commandConfirmedCam.Priority = priorityNonActive;
        _magicPanelStartCame.Priority = priorityNonActive;
        _startActionCam.Priority = priorityNonActive;
        _enemyActionCam.Priority = priorityNonActive;
        _defenseActionCam.Priority = priorityNonActive;
    }
}

/// <summary>
/// 戦闘全体カメラの種類
/// </summary>
public enum BattleCameraActiveType
{
    DefaultField,
    DefaultPlayer,
    ActionConfirmed,
    CommandConfirmed,
    MagicPanelStart,
    StartAction,
    EAction,
    DAction
}
