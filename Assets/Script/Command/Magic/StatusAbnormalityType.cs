using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 状態異常の種類
/// </summary>
public enum StatusAbnormalityType
{
    Burn,
    Frozen,
    Electrification,
    Wet,
    None
}

/// <summary>
/// 状態異常の情報
/// </summary>
public class StatusAbnormalityInfo
{
    public CharacterBaseStatus charaStatus;
    public StatusAbnormalityType statusAbnormalityType;
    public Sprite saSprite;
    public int saDuration;

    public StatusAbnormalityInfo(CharacterBaseStatus status, StatusAbnormalityType type, Sprite sprite, int duration)
    {
        charaStatus = status;
        statusAbnormalityType = type;
        saSprite = sprite;
        saDuration = duration;
    }
}
/// <summary>
/// 状態異常UIの情報
/// </summary>
public class StatusAbnormalityUIInfo
{
    public GameObject ui;
    public Image image;
    public TextMeshProUGUI text;

    public StatusAbnormalityUIInfo(GameObject ui, Image image, TextMeshProUGUI text)
    {
        this.ui = ui;
        this.image = image;
        this.text = text;
    }
}
