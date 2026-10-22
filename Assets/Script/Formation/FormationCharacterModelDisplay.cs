using UnityEngine;

/// <summary>
/// 編成・選択画面でのキャラクターモデルの表示を管理
/// </summary>
public class FormationCharacterModelDisplay : MonoBehaviour
{
    [SerializeField] private CharacterModelManager _characterModelManager;
    [Header("生成場所（編成画面）"), SerializeField] private Transform[] _generateFormationPositions;
    [Header("生成場所（選択画面）"), SerializeField] private Transform _generateSelectPoint;
    
    private CharacterTitleData[] _currentDisplayFormationModels;        // 現在、編成画面で表示中のモデル
    private CharacterTitleData _currentDisplaySelectModel;              // 現在、選択画面で表示中のモデル
    
    // TODO：編成画面と選択画面で処理を分ける
    // TODO：途中の処理は同じだと思うからそこは分けて考えた方がいいかもしれない
    // TODO：いっそのことクラスごとわけてもいいかもしれない（生成と表示を分ける）


    private void Awake()
    {
        _currentDisplayFormationModels = new CharacterTitleData[_generateFormationPositions.Length];
    }

    /// <summary>
    /// 編成画面でのキャラクターモデルの表示・非表示
    /// </summary>
    /// <param name="characterTitleData"></param>
    /// <param name="slotIndex"></param>
    public void DisplayFormationCharacterModel(CharacterTitleData characterTitleData, int slotIndex)
    {
        // 現在、表示しているモデルを取得
        GameObject currentModel = null;
        var currentData = _currentDisplayFormationModels[slotIndex];
        currentModel = currentData == null ? 
            _characterModelManager.GetOrCreateCharacterModel(characterTitleData, _generateFormationPositions[slotIndex]) 
            : _characterModelManager.GetOrCreateCharacterModel(currentData, null);
        // 表示するモデルを取得
        var pos = _generateFormationPositions[slotIndex];
        var nextModel = _characterModelManager.GetOrCreateCharacterModel(characterTitleData, pos);
        
        // 現在、表示中のモデルデータを更新
        _currentDisplayFormationModels[slotIndex] = characterTitleData;
        
        currentModel.SetActive(false);
        nextModel.SetActive(true);
    }

    /// <summary>
    /// 選択画面でのキャラクターモデルの表示・非表示
    /// </summary>
    /// <param name="characterTitleData"></param>
    public void DisplaySelectCharacterModel(CharacterTitleData characterTitleData)
    {
        // モデルの取得
        var currentModel =
            _characterModelManager.GetOrCreateCharacterModel(_currentDisplaySelectModel, _generateSelectPoint);
        var nextModel = _characterModelManager.GetOrCreateCharacterModel(characterTitleData, _generateSelectPoint);
        
        _currentDisplaySelectModel = characterTitleData;
        
        currentModel.SetActive(false);
        nextModel.SetActive(true);
    }
}
