using UnityEngine;
using UnityEngine.UI;

public class MassStatus : MonoBehaviour
{
    [Header("マスデータ")]
    [SerializeField] private MagicMassData magicMassData;
    public MagicMassData MagicMassData => magicMassData;
    
    private Image massImage; //マスのImage
    public Image MassImage => massImage;

    private void Awake()
    {
        massImage = GetComponent<Image>();
    }

    /// <summary>
    /// マスのデータを設定する
    /// </summary>
    /// <param name="_magicMassData">設定するマスのデータ</param>
    public void SetMassData(MagicMassData _magicMassData)
    {
        magicMassData = _magicMassData;
    }

    /// <summary>
    /// マスの色を設定する
    /// </summary>
    /// <param name="_color">設定するマスの色</param>
    public void SetMassColor(Color _color)
    {
        massImage.color = _color;
    }
}
