using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 戦闘を行うキャラクターの管理
/// </summary>
public class BattleCharacter
{
    public List<Character> Characters { get; } = new();

    /// <summary>
    /// キャラクターの登録
    /// </summary>
    /// <param name="character">登録するキャラクター</param>
    public void RegisterCharacter(Character character)
    {
        if(character == null || Characters.Contains(character)) return;
        Characters.Add(character);
    }

    /// <summary>
    /// キャラクターの削除
    /// </summary>
    /// <param name="character">削除するキャラクター</param>
    public void RemoveCharacter(Character character)
    {
        if(character == null || Characters.Contains(character)) return;
        Characters.Remove(character);
    }
    
    /// <summary>
    /// 生存しているプレイヤーキャラクターの取得
    /// </summary>
    /// <returns>生存しているプレイヤーキャラクター</returns>
    public List<Character> GetPlayerCharacters()
    {
        return Characters.Where(character => character.IsPlayer && character.BaseStatus.IsAlive).ToList();
    }

    /// <summary>
    /// 生存しているエネミーキャラクターの取得
    /// </summary>
    /// <returns>生存しているエネミーキャラクター</returns>
    public List<Character> GetEnemyCharacters()
    {
        return Characters.Where(character => !character.IsPlayer && character.BaseStatus.IsAlive).ToList();
    }

    /// <summary>
    /// Characterを取得する
    /// </summary>
    /// <param name="character">Characterを取得したいキャラクターオブジェクト</param>
    /// <returns>指定キャラクターオブジェクトのCharacter</returns>
    public Character GetCharacter(GameObject character)
    {
        return Characters.Find(c => c.CharacterObject == character);
    }

    /// <summary>
    /// キャラクターをクリア
    /// </summary>
    public void ClearCharacters()
    {
        Characters.Clear();
    }
}
