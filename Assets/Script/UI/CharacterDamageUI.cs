using System;
using System.Collections.Generic;
using System.Numerics;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

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
    [Header("ダメージUIを表示するターゲットの位置のOffset")]
    [SerializeField] private Vector2 _damageUITargetOffset;

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

    private void Update()
    {
        DamageUIUpdate();
    }

    /// <summary>
    /// ダメージUIを表示する位置の更新を行う
    /// </summary>
    private void DamageUIUpdate()
    {
        // ダメージUIを表示後、表示するUIがあるのであればターゲットに追従するようにする
        foreach (var damageUI in damageUIs)
        {
            // ダメージが非表示、UIを表示するターゲットがいない場合は処理を飛ばす
            if(!damageUI.DamageUIPrefabInstance.activeSelf) continue;
            if(damageUI.UITarget == null) continue;
            
            Vector3 offset = new Vector3(_damageUITargetOffset.x, _damageUITargetOffset.y, 0);
            Vector2 screenPos = damageUI.GetScreenLocalPosition(damageUI.UITarget.position + offset, _mainCamera);
            screenPos += damageUI.UIOffset;
            
            Vector2 localPos = damageUI.GetCanvasLocalPosition(screenPos, _canvasTransform);
            damageUI.DamageUIPrefabInstance.transform.localPosition = localPos;
        }
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
            var image = obj.transform.GetChild(1).GetComponent<Image>();
            var damageUI = new DamageUI(
                obj, text, image);
            
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
    /// <param name="icon">ダメージアイコン</param>>
    public async UniTask DamageUIShowDisplay(Transform target, float damage, Sprite icon)
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
            // ランダムで表示位置を決定
            Vector3 offset = new Vector3(_damageUITargetOffset.x, _damageUITargetOffset.y, 0);
            Vector2 screenPos = damageUI.GetScreenLocalPosition(target.position + offset, _mainCamera);
            // スクリーン座標が被らないようにランダムで値を作成
            Vector2 randomOffset = new Vector2(
                UnityEngine.Random.Range(-60f, 60f), UnityEngine.Random.Range(-60f, 60f)
            );
            screenPos += randomOffset;
            damageUI.SetTarget(target, randomOffset);
            
            Vector2 localPos = damageUI.GetCanvasLocalPosition(screenPos, _canvasTransform);
            damageUI.DamageUIPrefabInstance.transform.localPosition = localPos;
            damageUI.DamageUIPrefabInstance.SetActive(true);
            
            damageUI.SetDamageText(damage);
            damageUI.SetIcon(icon);
            await damageUI.Hidden(500);
        }
    }

    //TODO：この処理は後でも構わない
    // TODO：これなにをやろうとしていたのかわからない
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
    /// <summary>
    /// ダメージUI
    /// </summary>
    public GameObject DamageUIPrefabInstance{get; private set;}
    /// <summary>
    /// ダメージUIを表示するターゲット
    /// </summary>
    public Transform UITarget {get; private set;}
    /// <summary>
    /// ダメージUIのオフセット
    /// </summary>
    public Vector2 UIOffset {get; private set;}
    private TextMeshProUGUI damageUIText;
    private Image imageIcon;

    public DamageUI(GameObject obj, TextMeshProUGUI text, Image icon)
    {
        DamageUIPrefabInstance = obj;
        damageUIText = text;
        imageIcon = icon;
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
    /// ダメージアイコンを設定
    /// </summary>
    /// <param name="icon">アイコンの画像</param>
    public void SetIcon(Sprite icon)
    {
        if (icon != null)
        {
            imageIcon.enabled = true;
        }
        else
        {
            imageIcon.enabled = false;
        }
        imageIcon.sprite = icon;
    }

    /// <summary>
    /// ダメージUIを表示するターゲットの設定を行う
    /// </summary>
    /// <param name="target">ダメージUIを表示するターゲットの<c>Transform</c></param>
    /// <param name="offset">ダメージUIを表示する位置の<c>Vector2</c></param>
    public void SetTarget(Transform target, Vector2 offset)
    {
        UITarget = target;
        UIOffset = offset;
    }

    /// <summary>
    /// ダメージを表示したあと、指定時間後に非表示にする
    /// </summary>
    /// <param name="time">待機時間</param>>
    public async UniTask Hidden(int time)
    {
        //拡大してから、縮小して非表示にする
        var enlargementT = DamageUIPrefabInstance.transform.DOScale(
            new Vector3(2f, 2f, 2f), 0.5f).SetEase(Ease.InQuart);
        //シーンを遷移することを考慮
        enlargementT.OnKill(() =>
        {
            if (DamageUIPrefabInstance != null) DamageUIPrefabInstance.transform.DOKill();
        });
        await enlargementT.AsyncWaitForCompletion();
        await UniTask.Delay(TimeSpan.FromMilliseconds(time));
        var reductionT = DamageUIPrefabInstance.transform.DOScale(
            new Vector3(0.5f, 0.5f, 0.5f), 0.5f).SetEase(Ease.InQuart);
        reductionT.OnKill(() =>
        {
            if (DamageUIPrefabInstance != null) DamageUIPrefabInstance.transform.DOKill();
        });
        await reductionT.AsyncWaitForCompletion();
        if (DamageUIPrefabInstance != null)
        {
            DamageUIPrefabInstance.SetActive(false);
            UITarget = null;
        }
    }
}
