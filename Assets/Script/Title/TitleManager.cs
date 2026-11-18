using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面の管理を行う
/// </summary>
public class TitleManager : MonoBehaviour
{
    
    // TODO：これをタイトル管理としてつかう
    
    
    private void Awake()
    {
        Time.timeScale = 1;
    }

    public void GameStart()
    {
        // TODO：ここゲームシーンに名前を変更する
        SceneManager.LoadScene("SampleScene");
    }
}
