using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 戦闘を行うキャラクターの生成などを管理
/// </summary>
public class BattleCharacterSetUp : MonoBehaviour
{
    [SerializeField] private TurnManager _turnManager;
    [SerializeField] private PartyStatusUI _partyStatusUI;
    private BattleCharacter _battleCharacter;
    private BattleUltimateManager _battleUltimateManager;

    public void GetBattleSystem(BattleCharacter battleCharacter, BattleUltimateManager battleUltimateManager)
    {
        _battleCharacter = battleCharacter;
        _battleUltimateManager = battleUltimateManager;
    }

    public void BattleInitialize()
    {
        SetCombatInformation();
    }
    
    /// <summary>
    /// Titleの戦闘情報を元に必要な処理を行う
    /// ・戦闘を行うキャラクターの生成
    /// ・速度順にソート
    /// </summary>
    private void SetCombatInformation()
    {
        // 戦闘キャラクターを生成
        _battleCharacter.ClearCharacters();
        // 戦闘を行うキャラクターを格納する
        List<GameObject> fieldCharacter = new List<GameObject>();
        fieldCharacter.AddRange(CombatInformationCharacterGenerate());

        // 速度順にソート
        fieldCharacter = fieldCharacter.OrderByDescending(i =>
            i.GetComponent<Status>().GetCharacter().BaseStatus.Speed).ToList();

        // TurnManagerにデータを渡す
        var maxIndex = fieldCharacter.Count - 1;
        var lastTurnCharacter = fieldCharacter[maxIndex];
        _turnManager.GetBattleManagerData(fieldCharacter, lastTurnCharacter);

        // 必殺技の登録
        foreach (var character in _battleCharacter.GetPlayerCharacters())
        {
            character.EventsSystem.onUltimateActivated += _battleUltimateManager.ExecuteUltimate;
        }
    }
    
    /// <summary>
    /// 戦闘情報にあるキャラクターを生成する
    /// </summary>
    /// <returns>戦闘を行うキャラクターのリストを返す</returns>
    private List<GameObject> CombatInformationCharacterGenerate()
    {
        // 戦闘を行うプレイヤー、エネミーを取得する
        var infoP = CombatInformationManager.Instance.CombatInfoPlayerCharacter;
        var infoE = CombatInformationManager.Instance.CombatInfoEnemyCharacter;

        List<GameObject> infoObj = new List<GameObject>();
        foreach (var info in infoP)
        {
            var objP = Instantiate(info.CharacterPrefab);
            infoObj.Add(objP);

            // BattleCharacterに登録
            var character = objP.GetComponent<Status>().GetCharacter();
            _battleCharacter.RegisterCharacter(character);

            // パーティーステータスUIに登録
            _partyStatusUI.SetPartyCharacter(objP);
        }

        var objE = Instantiate(infoE.CharacterPrefab);
        infoObj.Add(objE);

        // Enemyも登録
        var enemyCharacter = objE.GetComponent<Status>().GetCharacter();
        _battleCharacter.RegisterCharacter(enemyCharacter);

        return infoObj;
    }
}
