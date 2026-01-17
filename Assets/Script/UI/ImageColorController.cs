using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Imageに付けるスクリプト
/// Imageの色を変更する
/// </summary>
public class ImageColorController : MonoBehaviour
{
    [Header("使用可能")]
    [SerializeField] private Color _selectedColor;
    [Header("使用不可")]
    [SerializeField] private Color _notSelectedColor;
    
    [Header("イメージ")]
    [SerializeField] private Image _image;

    /// <summary>
    /// 使用可能
    /// </summary>
    public void AvailableColor()
    {
        _image.color = _selectedColor;
    }

    /// <summary>
    /// 使用不可
    /// </summary>
    public void UnavailableColor()
    {
        _image.color = _notSelectedColor;
    }
}
