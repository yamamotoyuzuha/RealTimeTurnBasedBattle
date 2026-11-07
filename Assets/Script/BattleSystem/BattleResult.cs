/// <summary>
/// 戦闘結果の管理
/// </summary>
public class BattleResult
{
    private readonly BattleCharacter _battleCharacter;
    
    public BattleResult(BattleCharacter battleCharacter)
    {
        _battleCharacter = battleCharacter;
    }
    
    /// <summary>
    /// 戦闘結果を判定
    /// </summary>
    /// <returns>勝敗結果</returns>
    public ResultType CheckBattleResult()
    {
        bool isPlayerAlive = _battleCharacter.GetPlayerCharacters().Count > 0;
        bool isEnemyAlive = _battleCharacter.GetEnemyCharacters().Count > 0;
        
        // 敵が全滅
        if (!isEnemyAlive)
        {
            return ResultType.Victory;
        }

        // プレイヤーが全滅
        if (!isPlayerAlive)
        {
            return ResultType.Defeat;
        }

        return ResultType.None;
    }
}

/// <summary>
/// リザルトの種類
/// </summary>
public enum ResultType
{
    None,
    Victory,
    Defeat,
}
