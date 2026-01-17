using Cinemachine;
using UnityEngine;

public class CharacterCameraSettings : MonoBehaviour
{
    [Header("行動確定時のカメラ")] 
    [SerializeField] private CinemachineVirtualCameraBase _actionConfirmedCam;
    [Header("各コマンド決定時のカメラ")]
    [SerializeField] private CinemachineVirtualCamera _commandConfirmedCam;

    /// <summary>
    /// 行動確定時のカメラに切り替える
    /// </summary>
    public void ActionConfirmedCamChange()
    {
        _actionConfirmedCam.gameObject.SetActive(true);
        _commandConfirmedCam.gameObject.SetActive(false);
    }

    /// <summary>
    /// 各コマンド決定時のカメラに切り替える
    /// </summary>
    public void CommandConfirmedCamChange()
    {
        _actionConfirmedCam.gameObject.SetActive(false);
        _commandConfirmedCam.gameObject.SetActive(true);
    }
}
