using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 勝敗UIの管理
/// ・勝敗UIと敗北UI
/// </summary>
public class VictoryOrDefeatUI : MonoBehaviour
{
    [Header("CommandInputManager")]
    [SerializeField] private CommandInputManager _commandInputManager;
    [Header("勝利UI")]
    [SerializeField] private GameObject _victoryUI;
    [Header("敗北UI")]
    [SerializeField] private GameObject _defeatUI;
    [Header("DOTweenの設定")]
    [Header("勝敗UIの表示位置")]
    [SerializeField] private RectTransform _uiTransform;
    [Header("勝敗UIの移動時間")] 
    [SerializeField] private float _moveTime;
    [Header("フェード用画像")]
    [SerializeField] private Image _fadeImage;
    [Header("フェードの時間")]
    [SerializeField] private float _fadeTime;
    //勝敗UIのRectTransform
    private RectTransform victoryUIRect;
    private RectTransform defeatUIRect;
    
    /// <summary>
    /// 勝利UIの表示
    /// </summary>
    public Func<UniTask> OnVictoryUIDisplay{get;private set;}
    /// <summary>
    /// 敗北UIの表示
    /// </summary>
    public Func<UniTask> OnDefeatUIDisplay{get;private set;}

    void Awake()
    {
        victoryUIRect = _victoryUI.GetComponent<RectTransform>();
        defeatUIRect = _defeatUI.GetComponent<RectTransform>();
        OnVictoryUIDisplay += VictoryUI;
        OnDefeatUIDisplay += DefeatUI;
    }

    /// <summary>
    /// 勝利UIの表示
    /// </summary>
    private async UniTask VictoryUI()
    {
        //フェードと移動の処理（Time.Scaleを無視する）
        var fade = _fadeImage.DOFade(1f, _fadeTime);
        var move = victoryUIRect.DOAnchorPos(_uiTransform.anchoredPosition, _moveTime)
            .SetEase(Ease.OutQuad);
        
        //フェードと移動を同時に行う
        Sequence seq = DOTween.Sequence();
        seq.Append(fade);
        seq.Join(move);
        seq.SetUpdate(true);
        await seq.AsyncWaitForCompletion();
        
        //入力されるまで待機する
        await UniTask.WaitUntil(() => _commandInputManager.CommandInput.Player.Decision.triggered);
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// 敗北UIの表示
    /// </summary>
    private async UniTask DefeatUI()
    {
        var fade = _fadeImage.DOFade(1f, _fadeTime);
        var move = defeatUIRect.DOAnchorPos(_uiTransform.anchoredPosition, _moveTime)
            .SetEase(Ease.OutQuad);
        
        Sequence seq = DOTween.Sequence();
        seq.Append(fade);
        seq.Join(move);
        seq.SetUpdate(true);
        await seq.AsyncWaitForCompletion();
        
        await UniTask.WaitUntil(() => _commandInputManager.CommandInput.Player.Decision.triggered);
        SceneManager.LoadScene("Title");
    }
}
