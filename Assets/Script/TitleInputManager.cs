using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleInputManager : MonoBehaviour
{
    public TitleInput TitleInput{get; private set;}

    void Awake()
    {
        //InputSystemを使えるようにする
        TitleInput = new TitleInput();
        TitleInput.Enable();
    }

    void Update()
    {
        //これはデバッグ用　いずれはゲーム進行用のステートマシンを作成する
        if (TitleInput.Player.Decision.triggered)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
