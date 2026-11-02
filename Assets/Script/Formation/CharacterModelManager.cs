using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターモデルの生成・取得・保持を管理
/// </summary>
public class CharacterModelManager : MonoBehaviour
{
    /// <summary>
    /// 生成済みのモデルを管理
    /// </summary>
    private readonly Dictionary<Transform, Dictionary<CharacterTitleData, GameObject>> _models = new();

    public GameObject GetOrCreateCharacterModel(CharacterTitleData characterTitleData, Transform parent)
    {
        // 表示場所の辞書を取得
        if (!_models.TryGetValue(parent, out var models))
        {
            models = new Dictionary<CharacterTitleData, GameObject>();
            _models.Add(parent, models);
        }

        // すでに生成されている場合は再利用
        if (models.TryGetValue(characterTitleData, out var model))
        {
            if (model != null) return model;

            models.Remove(characterTitleData);
        }

        // まだ生成されていない場合は生成
        model = Instantiate(characterTitleData.CharacterPrefab, parent);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.SetActive(false);

        models.Add(characterTitleData, model);

        return model;
    } 
    
    public GameObject GetCharacterModel(CharacterTitleData characterTitleData, Transform parent)
    {
        if (_models.TryGetValue(parent, out var models) 
            && models.TryGetValue(characterTitleData, out var model)) return model;

        return null;
    }
}
