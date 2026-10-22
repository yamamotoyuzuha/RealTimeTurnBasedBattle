using UnityEngine;

/// <summary>
/// タイトル画面で使用するキャラクターデータ
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObject/CharacterTitleData", fileName = "CharacterTitleData")]
public class CharacterTitleData : ScriptableObject
{
    [Header("キャラクターの名前"), SerializeField] private string _characterName;
    [Header("キャラクターPrefab"), SerializeField] private GameObject _characterPrefab;
    [Header("キャラクターアイコン"), SerializeField] private Sprite _characterIconSprite;

    public string CharacterName => _characterName;
    public GameObject CharacterPrefab => _characterPrefab;
    public Sprite CharacterIconSprite => _characterIconSprite;
}
