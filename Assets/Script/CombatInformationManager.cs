using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 戦闘情報の管理
/// ・プレイヤーが操作するキャラクターの保持（パーティー）
/// ・タイトルで選択したEnemyの保持など
/// </summary>
public class CombatInformationManager : MonoBehaviour
{
    public static CombatInformationManager Instance;
    
    /// <summary>
    /// 戦闘を行うプレイヤーキャラクターの情報
    /// </summary>
    public List<CharacterBaseData> CombatInfoPlayerCharacter {get; private set;} = new List<CharacterBaseData>();
    public CharacterBaseData CombatInfoEnemyCharacter{get; private set;}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    /// <summary>
    /// パーティー編成のデータを戦闘情報に追加する
    /// </summary>
    /// <param name="combatInfoCharacters">パーティーに編成されたキャラクターのリスト</param>
    public void AddCombatInfoCharacter(List<CharacterBaseData> combatInfoCharacters)
    {
        CombatInfoPlayerCharacter.Clear();
        CombatInfoPlayerCharacter.AddRange(combatInfoCharacters);
    }

    /// <summary>
    /// 戦闘を行うEnemyのデータを戦闘を情報に追加する
    /// </summary>
    /// <param name="combatInfoEnemy"></param>
    public void AddCombatInfoEnemy(CharacterBaseData combatInfoEnemy)
    {
        CombatInfoEnemyCharacter = combatInfoEnemy;
    }
}
