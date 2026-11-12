using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターターンアイコンを管理する
/// ・生成と破棄
/// </summary>
public class TurnIconManager : MonoBehaviour
{
    [Header("キャラクターアイコン生成場所"), SerializeField] private Transform _iconRoot;
    [Header("キャラクターアイコンPrefab"), SerializeField] private GameObject _iconPrefab;

    private readonly Dictionary<GameObject, List<GameObject>> _characterIcons = new();
    private const int DisplayCount = 2; // 表示するアイコン数

    /// <summary>
    /// 戦闘開始時のキャラクターアイコンを生成
    /// </summary>
    /// <param name="characters">ターン順</param>
    public void GenerateIcons(List<GameObject> characters)
    {
        var num = characters.Count;
        var count = num * DisplayCount; // 表示するアイコン数

        for (int i = 0; i < count; i++)
        {
            var index = i % num; // 循環させる
            var character = characters[index];
            
            // アイコン生成
            var icon = Instantiate(_iconPrefab, _iconRoot);
            // アイコンをキャラクターデータのアイコンに設定する
            icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(character.GetComponent<Status>().GetCharacter().BaseData);
            if (!_characterIcons.ContainsKey(character)) // まだ、キャラクターをKeyに登録してない場合
            {
                _characterIcons[character] = new List<GameObject>();
            }
            _characterIcons[character].Add(icon);
        }
    }

    /// <summary>
    /// キャラクターアイコンを生成
    /// </summary>
    /// <param name="character">アイコンを生成するキャラクター</param>
    public void GenerateCharacterIcon(GameObject character)
    {
        // キャラクターが存在しない場合、処理を行わない
        if(!_characterIcons.TryGetValue(character, out var icons)) return;
        
        GenerateProcessing(character, ref icons);
    }

    /// <summary>
    /// 割り込みをしたキャラクターのアイコンを生成
    /// </summary>
    /// <param name="character">割り込みをしたキャラクター</param>
    public void GenerateSpecifiedCharacterIcon(GameObject character)
    {
        // 指定したキャラクターが存在しない場合、処理を行わない
        if(!_characterIcons.TryGetValue(character, out var icons)) return;
        
        var icon =  GenerateProcessing(character, ref icons);
        icon.transform.SetSiblingIndex(0);
    }

    /// <summary>
    /// 割り込みで生成したアイコンを削除する
    /// </summary>
    /// <param name="character">割り込みをしたキャラクター</param>
    public void RemoveSpecifiedCharacterIcon(GameObject character)
    {
        // キャラクターが存在しない場合は、処理を行わない
        if (!_characterIcons.TryGetValue(character, out var icons) || icons.Count == 0) return;
        // UI上の一番上にあるものを削除する
        var icon = _iconRoot.GetChild(0).gameObject;
        var index = 0;
        for (int i = 0; i < icons.Count; i++)
        {
            if (icons[i] == icon) index = i;
        }
        icons.RemoveAt(index);
        Destroy(icon);
    }

    /// <summary>
    /// アイコンを削除する
    /// </summary>
    /// <param name="character">削除するキャラクター</param>
    public void DestroyCharacterIcon(GameObject character)
    {
        // キャラクターが存在しない、キャラクターアイコンがない場所、処理を行わない
        if (!_characterIcons.TryGetValue(character, out var icons) || icons.Count == 0) return;
        
        // 先に追加されたアイコンを削除する
        var icon = icons[0];
        icons.RemoveAt(0);
        Destroy(icon);
    }

    /// <summary>
    /// 死亡したキャラクターのアイコンを削除する
    /// </summary>
    /// <param name="character">死亡したキャラクター</param>
    public void DeathCharacterIconDestroy(GameObject character)
    {
        if(!_characterIcons.TryGetValue(character, out var icons) || icons.Count == 0) return;

        foreach (var icon in icons)
        {
            Destroy(icon);
        }
        
        _characterIcons.Remove(character);
    }

    /// <summary>
    /// キャラクターアイコンの生成と設定を行う
    /// </summary>
    /// <param name="character">アイコン生成が必要なキャラクター</param>
    /// <param name="icons">生成済みのアイコン</param>
    /// <returns>生成したアイコン</returns>
    private GameObject GenerateProcessing(GameObject character, ref List<GameObject> icons)
    {
        // キャラクターアイコンを生成、設定
        var icon = Instantiate(_iconPrefab, _iconRoot);
        icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(character.GetComponent<Status>().GetCharacter().BaseData);
        icons.Add(icon);

        return icon;
    }

    /// <summary>
    /// キャラクターアイコンの表示・非表示
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>
    public void DisplayCharacterIcon(bool flag)
    {
        foreach (var iconsValue in _characterIcons.Values)
        {
            foreach (var icon in iconsValue)
            {
                icon.SetActive(flag);
            }
        }
    }
    
    public List<GameObject> GetCharacterIcons(GameObject character)
    {
        return _characterIcons[character];
    }
}
