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
        Time.timeScale = 1;
        //InputSystemを使えるようにする
        TitleInput = new TitleInput();
        TitleInput.Enable();
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void GameStart()
    {
        if(!isStartGame) return;
        
        // TODO：ここゲームシーンに名前を変更する
        SceneManager.LoadScene("SampleScene");
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
