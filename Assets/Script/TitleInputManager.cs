using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleInputManager : MonoBehaviour
{
    public TitleInput TitleInput{get; private set;}

    /// <summary>
    /// ゲーム開始
    /// true：可能　false：不可能
    /// </summary>
    private bool isStartGame = true;

    void Awake()
    {
        //InputSystemを使えるようにする
        TitleInput = new TitleInput();
        TitleInput.Enable();
    }

    void Update()
    {
        if(!isStartGame) return;
        //これはデバッグ用　いずれはゲーム進行用のステートマシンを作成する
        if (TitleInput.Player.Decision.triggered)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    /// <summary>
    /// ゲーム開始のフラグ
    /// </summary>
    /// <param name="isFlag">true：可能　false：不可能</param>
    public void SetGame(bool isFlag)
    {
        isStartGame = isFlag;
    }
}
