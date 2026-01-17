using UnityEngine;

public class ItemBaseData : ScriptableObject
{
    [Header("アイテムの名前")]
    [SerializeField] private string itemName;
    public string ItemName => itemName;
    [Header("アイテムの説明")]
    [SerializeField] private string itemDescription;
    public string ItemDescription => itemDescription;
}
