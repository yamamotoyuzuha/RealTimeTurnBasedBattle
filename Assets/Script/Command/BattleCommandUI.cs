using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleCommandUI : MonoBehaviour
{
    [Header("確定コマンドUI")]
    [SerializeField] private GameObject _confirmedCommandUI;
    
    /// <summary>
    /// 確定コマンドUIの表示、非表示
    /// </summary>
    public Action<bool> OnConfirmedCommandUIDisplay{get; private set;}
    
    //確定コマンドのテキストを保持
    private Image  confirmedCommandUIImage;
    private TextMeshProUGUI  confirmedCommandNameUIText;
    private TextMeshProUGUI  confirmedDescriptionUIText;
    
    //TODO：これはキャンバスじゃなくて、プレイヤーの子オブジェクトのワールドキャンバスしたほうがいいかも

    private void Start()
    {
        //イメージ、テキストを取得
        confirmedCommandUIImage = _confirmedCommandUI.transform.GetChild(0).GetComponent<Image>();
        confirmedCommandNameUIText = _confirmedCommandUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        confirmedDescriptionUIText = _confirmedCommandUI.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        
        MagicUITextToggle(false);
        OnConfirmedCommandUIDisplay += MagicUITextToggle;
    }

    /// <summary>
    /// 選択した魔法のデータをテキストに反映させる
    /// </summary>
    /// <param name="magicData">選択した魔法</param>
    public void SetMagicUIText(MagicBaseData magicData)
    {
        confirmedCommandNameUIText.text = magicData.MagicName; 
        confirmedDescriptionUIText.text = magicData.MagicExplanation;
        
        MagicUITextToggle(true);
    }

    /// <summary>
    /// テキストを設定前に戻す
    /// </summary>
    public void UndoMagicUIText()
    {
        confirmedCommandNameUIText.text = ""; 
        confirmedDescriptionUIText.text = "";
        
        MagicUITextToggle(false);
    }
    
    /// <summary>
    /// 魔法UIのテキストの表示、非表示をする
    /// </summary>
    /// <param name="isDisplay">true；表示　false；非表示</param>
    private void MagicUITextToggle(bool isDisplay)
    {
        confirmedCommandUIImage.enabled = isDisplay;
        confirmedCommandNameUIText.enabled = isDisplay;
        confirmedDescriptionUIText.enabled = isDisplay;
    }
}
