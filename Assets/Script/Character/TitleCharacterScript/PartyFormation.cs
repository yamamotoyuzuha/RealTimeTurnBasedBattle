using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトル画面で行うパーティー編成
/// </summary>
public class PartyFormation : MonoBehaviour
{
    [Header("TitleInputManager")]
    [SerializeField] private TitleInputManager _titleInputManager;
    [Header("PartyFormationUIController")]
    [SerializeField] private PartyFormationUIController _partyFormationUIController;
    [Header("TitleUIManager")]
    [SerializeField] private TitleUIManager _titleUIManager;
    [Header("キャラクター一覧")]
    [SerializeField] private List<CharacterBaseData> _allCharacters = new List<CharacterBaseData>();
    [Header("パーティー")]
    [SerializeField] private List<CharacterBaseData> _partys = new List<CharacterBaseData>();
    [Header("最大パーティー人数")]
    [SerializeField] private int _maxPartyCount;
    /// <summary>
    /// 現在、選択しているキャラクター
    /// </summary>
    private CharacterBaseData selectedChara;
    /// <summary>
    /// パーティー編成画面が表示、非表示
    /// true：表示　false：非表示
    /// </summary>
    public bool IsPartyDisplay { get; private set; }
    private int currentIndex;
    
    void Start()
    {
        //UIなどの生成を行う
        foreach (var chara in _allCharacters)
        {
            _partyFormationUIController.onPartyFormationGenerate?.Invoke(chara);
            _partyFormationUIController.onPartyFormationCharacterGenerate?.Invoke(chara, _allCharacters.IndexOf(chara));
        }
        //初期設定
        currentIndex = 0;
        selectedChara = _allCharacters[0];
        SelectedCharacter(0);
        CombatInformationManager.Instance.AddCombatInfoCharacter(_partys);
    }

    void Update()
    {
        PartyDisplayToggle();
        
        if(!IsPartyDisplay) return;
        if (_titleInputManager.TitleInput.Player.SelectLeft.triggered)
        {
            SelectedCharacter(-1);
        }
        if (_titleInputManager.TitleInput.Player.SelectRight.triggered)
        {
            SelectedCharacter(1);
        }

        if (_titleInputManager.TitleInput.Player.Add.triggered) //パーティーに加える
        {
            AddChara(selectedChara);
        }
        if (_titleInputManager.TitleInput.Player.Remove.triggered) //パーティーから外す
        {
            RemoveChara(selectedChara);
        }
    }

    /// <summary>
    /// パーティー編成画面の表示切り替え
    /// </summary>
    private void PartyDisplayToggle()
    {
        if (_titleInputManager.TitleInput.Player.PartyDisplay.triggered)
        {
            if (!IsPartyDisplay) //表示
            {
                IsPartyDisplay = true;
                _partyFormationUIController.onPartyShowDisplay?.Invoke(true);
                _titleInputManager.SetGame(false);
                _titleUIManager.onPfShow?.Invoke();
            }
            else //非表示
            {
                IsPartyDisplay = false;
                _partyFormationUIController.onPartyHideDisplay?.Invoke(false);
                _titleInputManager.SetGame(true);
                _titleUIManager.onTitleShow?.Invoke();
                
                //非表示にしたタイミングでパーティー編成の確定
                CombatInformationManager.Instance.AddCombatInfoCharacter(_partys);
            }
        }
    }

    /// <summary>
    /// パーティーにキャラクターを追加
    /// </summary>
    /// <param name="chara">追加するキャラクター</param>
    private void AddChara(CharacterBaseData chara)
    {
        if(_partys.Count >= _maxPartyCount) return;
        if(!_partys.Contains(chara)) _partys.Add(chara);
        SelectedCharacterInParty(chara);
    }

    /// <summary>
    /// パーティーからキャラクターを削除
    /// </summary>
    /// <param name="chara">削除するキャラクター</param>
    private void RemoveChara(CharacterBaseData chara)
    {
        if(_partys.Contains(chara)) _partys.Remove(chara);
        SelectedCharacterInParty(chara);
    }

    /// <summary>
    /// キャラクターの選択
    /// </summary>
    /// <param name="index">インデックス</param>>
    private void SelectedCharacter(int index)
    {
        //左右どちらにも循環をする
        currentIndex = (currentIndex + index + _allCharacters.Count) % _allCharacters.Count;
        selectedChara = _allCharacters[currentIndex];
        _partyFormationUIController.onSelectCharacter?.Invoke(selectedChara);
        Debug.Log(selectedChara);

        SelectedCharacterInParty(selectedChara);
    }

    /// <summary>
    /// 選択中のキャラクターがパーティーに加入しているか判定
    /// </summary>
    /// <param name="chara">選択中のキャラクター</param>
    private void SelectedCharacterInParty(CharacterBaseData chara)
    {
        var isAdd = _partys.Contains(chara);
        _partyFormationUIController.onAddOrRemoveCharacter?.Invoke(chara, isAdd);
    }
}
