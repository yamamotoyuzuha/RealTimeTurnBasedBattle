using UnityEngine;

/// <summary>
/// 選択画面でのキャラクターモデルの表示・非表示を管理
/// </summary>
public class SelectCharacterModelDisplay : MonoBehaviour
{
    [SerializeField] private CharacterModelManager _characterModelManager;
    [Header("生成場所"), SerializeField] private Transform _generateSelectPoint;

    public CharacterTitleData CurrentSelectModel { get; private set; }
    
    /// <summary>
    /// 選択画面でのキャラクターモデルの表示・非表示
    /// </summary>
    /// <param name="characterTitleData">キャラクターデータ</param>
    public void DisplaySelectCharacterModel(CharacterTitleData characterTitleData)
    {
        // 現在表示しているモデルを非表示にする
        if (CurrentSelectModel != null)
        {
            var currentModel = _characterModelManager.GetCharacterModel(CurrentSelectModel, _generateSelectPoint);
            if (currentModel != null) currentModel.SetActive(false);
        }

        // 新しいモデルを取得
        var nextModel = _characterModelManager.GetOrCreateCharacterModel(characterTitleData, _generateSelectPoint);
        if (nextModel == null) return;

        nextModel.transform.localPosition = Vector3.zero;
        nextModel.transform.localRotation = Quaternion.identity;
        nextModel.SetActive(true);

        CurrentSelectModel = characterTitleData;
    }
    
    /// <summary>
    /// モデルの表示切り替え
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>
    public void DisplaySwitch(bool flag)
    {
        _generateSelectPoint.gameObject.SetActive(flag);
    }
}
