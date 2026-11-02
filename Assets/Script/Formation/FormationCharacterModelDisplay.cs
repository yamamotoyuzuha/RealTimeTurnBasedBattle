using UnityEngine;

/// <summary>
/// 編成画面でのキャラクターモデルの表示・非表示を管理
/// </summary>
public class FormationCharacterModelDisplay : MonoBehaviour
{
    [SerializeField] private CharacterModelManager _characterModelManager;
    [Header("生成場所のルート"), SerializeField] private GameObject _generateRoot;
    [Header("生成場所"), SerializeField] private Transform[] _generateFormationPositions;
    
    private CharacterTitleData[] _currentDisplayFormationModels;        // 編成画面で表示中のモデル
   
    private void Awake()
    {
        _currentDisplayFormationModels = new CharacterTitleData[_generateFormationPositions.Length];
    }

    /// <summary>
    /// 編成画面でのキャラクターモデルの表示・非表示
    /// </summary>
    /// <param name="characterTitleData">編成画面で選択したキャラクター</param>
    /// <param name="slotIndex">枠のインデックス</param>
    public void DisplayFormationCharacterModel(CharacterTitleData characterTitleData, int slotIndex)
    {
        var parent = _generateFormationPositions[slotIndex];
        var currentData = _currentDisplayFormationModels[slotIndex];
        // 現在表示しているモデルを非表示にする
        if (currentData != null)
        {
            var currentModel = _characterModelManager.GetCharacterModel(currentData, parent);
            if (currentModel != null) currentModel.SetActive(false);
        }

        // 新しいモデルを取得
        var nextModel = _characterModelManager.GetOrCreateCharacterModel(characterTitleData, parent);
        if (nextModel == null) return;

        nextModel.transform.localPosition = Vector3.zero;
        nextModel.transform.localRotation = Quaternion.identity;
        nextModel.SetActive(true);

        _currentDisplayFormationModels[slotIndex] = characterTitleData;
    }

    /// <summary>
    /// モデルの表示切り替え
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>
    public void DisplaySwitch(bool flag)
    {
        _generateRoot.SetActive(flag);
    }
}
