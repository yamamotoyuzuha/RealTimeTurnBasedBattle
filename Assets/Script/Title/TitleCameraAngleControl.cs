using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// タイトル画面のカメラアングル制御
/// </summary>
public class TitleCameraAngleControl : MonoBehaviour
{
    public static TitleCameraAngleControl Instance { get; private set; }

    [Header("カメラ")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [Header("カメラ位置")] 
    [SerializeField] private Transform _homePos;
    [SerializeField] private Transform _formationPos;
    [SerializeField] private Transform _selectionPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// カメラアングルの切り替え
    /// </summary>
    /// <param name="state">変更後のカメラアングル</param>
    public void ChangeCameraAngle(TitleSceneState state)
    {
        switch (state)
        {
            case TitleSceneState.Home:
                _virtualCamera.Follow = _homePos;
                _virtualCamera.LookAt = _homePos;
                break;
            case TitleSceneState.Formation:
                _virtualCamera.Follow = _formationPos;
                _virtualCamera.LookAt = _formationPos;
                break;
            case TitleSceneState.Selection:
                _virtualCamera.Follow = _selectionPos;
                _virtualCamera.LookAt = _selectionPos;
                break;
        }
    }
}
