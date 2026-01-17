using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class CharacterDamageUI : MonoBehaviour
{
    public static CharacterDamageUI Instance { get; private set; }
    
    [Header("MainCamera")]
    [SerializeField] private Camera _mainCamera;
    [Header("Canvas")] 
    [SerializeField] private Canvas _canvasTransform;
    [Header("ダメージUIの生成場所")]
    [SerializeField] private Transform _damageUIParent;
    [Header("ダメージUIのPrefab")]
    [SerializeField] private GameObject _damageUIPrefab;

    /// <summary>
    /// 生成したDamageUI
    /// </summary>
    private List<DamageUI> damageUIs = new List<DamageUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        GenerateDamageUI(5);
    }

    /// <summary>
    /// UIの生成を行う
    /// </summary>
    /// <param name="count">生成する個数</param>
    private void GenerateDamageUI(int count)
    {
        //DamageUIを生成し、保持しておく
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(_damageUIPrefab, _damageUIParent);
            var text = obj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            var damageUI = new DamageUI(
                obj, text);
            
            damageUIs.Add(damageUI);
            damageUIs[i].DamageUIPrefabInstance.SetActive(false);
        }
    }

    /// <summary>
    /// 通常攻撃の場合
    /// ダメージUIの表示を更新する
    /// </summary>
    /// <param name="target">ダメージを受けたキャラクター</param>>
    /// <param name="damage">ダメージ</param>>
    public void DamageUIShowDisplay(Transform target, float damage)
    {
        //表示中ではないDamageUIを取得
        DamageUI damageUI = null;
        foreach (var ui in damageUIs)
        {
            if (!ui.DamageUIPrefabInstance.activeSelf)
            {
                damageUI = ui;
                break;
            }
        }
        
        //もし全てが表示中だった場合、UIを新しく生成する
        if (damageUI == null)
        {
            GenerateDamageUI(1);
        }
        else //表示を行う
        {
            //ランダムで表示位置を決定
            Vector2 screenPos = damageUI.GetScreenLocalPosition(target.transform.position, _mainCamera);
            //スクリーン座標をランダムでずらす
            screenPos.x += UnityEngine.Random.Range(-60f, 60f);
            screenPos.y += UnityEngine.Random.Range(-60f, 60f);
            
            Vector2 localPos = damageUI.GetCanvasLocalPosition(screenPos, _canvasTransform);
            damageUI.DamageUIPrefabInstance.transform.localPosition = localPos;
            damageUI.DamageUIPrefabInstance.SetActive(true);
            
            damageUI.SetDamageText(damage);
            damageUI.Hidden(500).Forget();
        }
    }

    //TODO：この処理は後でも構わない
    /// <summary>
    /// 属性攻撃の場合
    /// ダメージUIの表示を更新する
    /// </summary>
    /// <param name="target">ダメージを受けたキャラクター</param>
    /// <param name="damage">ダメージ</param>
    public void AttributeDamageUIShowDisplay(Transform target, float damage)
    {
        
    }
}

/// <summary>
/// ダメージUIの情報
/// ・生成したオブジェクト
/// ・ダメージを表示するテキスト
/// </summary>
public class DamageUI
{
    public GameObject DamageUIPrefabInstance{get; private set;}
    private TextMeshProUGUI damageUIText;

    public DamageUI(GameObject obj, TextMeshProUGUI text)
    {
        DamageUIPrefabInstance = obj;
        damageUIText = text;
    }
    
    public Vector2 GetCanvasLocalPosition(Vector3 screenPosition, Canvas canvas)
    {
        return canvas.transform.InverseTransformPoint(screenPosition);
    }

    /// <summary>
    /// ワールド座標をスクリーン座標に変換
    /// </summary>
    /// <param name="worldPosition">ワールド座標</param>
    /// <param name="mainCamera">メインカメラ</param>
    /// <returns></returns>
    public Vector2 GetScreenLocalPosition(Vector3 worldPosition, Camera mainCamera)
    {
        return RectTransformUtility.WorldToScreenPoint(mainCamera, worldPosition);
    }

    /// <summary>
    /// ダメージをテキストに設定
    /// </summary>
    /// <param name="damage">ダメージ</param>
    public void SetDamageText(float damage)
    {
        damageUIText.text = damage.ToString();
    }

    /// <summary>
    /// ダメージを表示したあと、指定時間後に非表示にする
    /// </summary>
    /// <param name="time">待機時間</param>>
    public async UniTask Hidden(int time)
    {
        //拡大してから、縮小して非表示にする
        await DamageUIPrefabInstance.transform.DOScale(
            new Vector3(2f, 2f, 2f), 0.5f).SetEase(Ease.InQuart).AsyncWaitForCompletion();
        await UniTask.Delay(TimeSpan.FromMilliseconds(time));
        await DamageUIPrefabInstance.transform.DOScale(
            new Vector3(0.5f, 0.5f, 0.5f), 0.5f).SetEase(Ease.InQuart).AsyncWaitForCompletion();
        DamageUIPrefabInstance.SetActive(false);
    }
}
