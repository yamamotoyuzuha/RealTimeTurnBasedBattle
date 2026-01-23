using Cinemachine;
using UnityEngine;

/// <summary>
/// プレイヤーキャラクターのカメラアングル設定
/// ・Enumによって表示するカメラの切り替え
/// ・TurnManagerから参照
/// </summary>
public class CharacterCameraSettings : MonoBehaviour
{
    [Header("デフォルトカメラ位置F")]
    [SerializeField] private Transform _defaultCamPosF;
    [Header("デフォルトカメラ位置L")]
    [SerializeField] private Transform _defaultCamPosL;
    [Header("行動確定時のカメラ位置F")]
    [SerializeField] private Transform _actionConfirmedCamPosF;
    [Header("行動確定時のカメラ位置L")]
    [SerializeField] private Transform _actionConfirmedCamPosL;
    [Header("各コマンド決定時のカメラ位置F")]
    [SerializeField] private Transform _commandConfirmedCamPosF;
    [Header("各コマンド決定時のカメラ位置L")]
    [SerializeField] private Transform _commandConfirmedCamPosL;
    [Header("魔法パネル開始時のカメラ位置F")]
    [SerializeField] private Transform _magicPanelStartCamPosF;
    [Header("魔法パネル開始時のカメラ位置L")]
    [SerializeField] private Transform _magicPanelStartCamPosL;
    [Header("行動開始時のカメラ位置F")]
    [SerializeField] private Transform _startActionCamPosF;
    [Header("行動確定時のカメラ位置L")]
    [SerializeField] private Transform _startActionCamPosL;

    #region Follow
    public Transform DefaultCamPosF => _defaultCamPosF;
    public Transform ActionConfirmedCamPosF => _actionConfirmedCamPosF;
    public Transform CommandConfirmedCamPosF => _commandConfirmedCamPosF;
    public Transform MagicPanelStartCamPosF => _magicPanelStartCamPosF;
    public Transform StartActionCamPosF => _startActionCamPosF;
    #endregion

    #region LookAt
    public Transform DefaultCamPosL => _defaultCamPosL;
    public Transform ActionConfirmedCamPosL => _actionConfirmedCamPosL;
    public Transform CommandConfirmedCamPosL => _commandConfirmedCamPosL;
    public Transform MagicPanelStartCamPosL => _magicPanelStartCamPosL;
    public Transform StartActionCamPosL => _startActionCamPosL;
    #endregion
    
    

    /*
    [Header("デフォルトカメラ")]
    [SerializeField] private CinemachineVirtualCamera _defaultCam;
    [Header("行動確定時のカメラ")] 
    [SerializeField] private CinemachineVirtualCamera _actionConfirmedCam;
    [Header("各コマンド決定時のカメラ")]
    [SerializeField] private CinemachineVirtualCamera _commandConfirmedCam;
    [Header("魔法パネル開始時のカメラ")] 
    [SerializeField] private CinemachineVirtualCamera _magicPanelStartCame;
    [Header("行動開始時のカメラ")] 
    [SerializeField] private CinemachineVirtualCamera _startActionCam;

    //シネマシーンのPriority設定
    private int priorityActive = 20;
    private int priorityNonActive = 10;
    */

    /*
    /// <summary>
    /// カメラの切り替え
    /// </summary>
    /// <param name="cameraActiveType">切り替えたいカメラアングル</param>
    public void CameraAngleChange(CameraActiveType cameraActiveType)
    {
        //ResetCameraAngles();

        switch (cameraActiveType)
        {
            case CameraActiveType.Default:
                _defaultCam.Priority = priorityActive;
                break;
            case CameraActiveType.ActionConfirmed:
                _actionConfirmedCam.Priority = priorityActive;
                break;
            case CameraActiveType.CommandConfirmed:
                _commandConfirmedCam.Priority = priorityActive;
                break;
            case CameraActiveType.MagicPanelStart:
                _magicPanelStartCame.Priority = priorityActive;
                break;
            case CameraActiveType.StartAction:
                _startActionCam.Priority = priorityActive;
                break;
        }
    }
    */

    /*
    /// <summary>
    /// シネマシーンのPriorityをリセット
    /// </summary>
    private void ResetCameraAngles()
    {
        _defaultCam.Priority = priorityNonActive;
        _actionConfirmedCam.Priority = priorityNonActive;
        _commandConfirmedCam.Priority = priorityNonActive;
        _magicPanelStartCame.Priority = priorityNonActive;
        _startActionCam.Priority = priorityNonActive;
    }
    */
}

/// <summary>
/// カメラアングルの種類
/// </summary>
public enum CameraActiveType
{
    Default,
    ActionConfirmed,
    CommandConfirmed,
    MagicPanelStart,
    StartAction,
}
