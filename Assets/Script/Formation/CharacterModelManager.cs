using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターモデルの生成・取得・保持を管理
/// </summary>
public class CharacterModelManager : MonoBehaviour
{
    private class ModelInfo
    {
        public GameObject CharacterModel { get; private set; }
        public Transform ModelTransform { get; private set; }

        public ModelInfo(GameObject obj, Transform transform)
        {
            CharacterModel = obj;
            ModelTransform = transform;
        }
    }
    
    private readonly Dictionary<CharacterTitleData, ModelInfo> _characterModel = new();  // 生成済みのモデル

    public GameObject GetOrCreateCharacterModel(CharacterTitleData characterTitleData, Transform parent)
    {
        var model = GetCharacterModel(characterTitleData);
        return model == null ? GenerateCharacterModel(characterTitleData, parent) : model;
    }
    
    private GameObject GetCharacterModel(CharacterTitleData characterTitleData)
    {
        return _characterModel.TryGetValue(characterTitleData, out var model) ? model.CharacterModel : null;
    }

    private GameObject GenerateCharacterModel(CharacterTitleData characterTitleData, Transform parent)
    {
        var prefab = characterTitleData.CharacterPrefab;
        var model = Instantiate(prefab, parent);
        _characterModel.Add(characterTitleData, new ModelInfo(model, parent));
        
        return model;
    }
}
