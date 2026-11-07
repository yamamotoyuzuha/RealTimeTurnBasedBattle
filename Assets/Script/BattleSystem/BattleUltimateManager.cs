using System;
using System.Linq;
using Cysharp.Threading.Tasks;

/// <summary>
/// 戦闘中の必殺技を管理
/// </summary>
public class BattleUltimateManager
{
    private readonly TurnManager _turnManager;
    private readonly BattleCharacter _battleCharacter;

    public BattleUltimateManager(TurnManager turnManager, BattleCharacter battleCharacter)
    {
        _turnManager = turnManager;
        _battleCharacter = battleCharacter;
    }
    
    /// <summary>
    /// 必殺技を実行する
    /// </summary>
    /// <param name="ult">実行する必殺技</param>
    /// <param name="baseStatus">必殺技を実行するキャラクターの<c>CharacterBaseStatus</c>></param>
    public async UniTask ExecuteUltimate(UltimateBaseData ult, CharacterBaseStatus baseStatus)
    {
        // 現在のターンキャラクターが行動を開始している場合は、必殺技を発動せず終了後に発動
        if (_turnManager.IsTurnStarting)
        {
            _turnManager.SetInterruptCharacterUlt(async () =>
            {
                _turnManager.SetTimingCalled(true);
                _turnManager.TurnCharacterCommandUI(baseStatus.CharaObject, false);
                _turnManager.onChangeTurnCharacter?.Invoke(baseStatus.CharaObject);
                await UniTask.Delay(TimeSpan.FromSeconds(2)); // TODO：仮演出であり、2秒待つ
                await ult.Execute(_battleCharacter.GetEnemyCharacters().FirstOrDefault()?.CombatSystem, baseStatus);
                if (_turnManager.onNextTurnSetUp != null)
                {
                    await _turnManager.onNextTurnSetUp.Invoke();
                }
                _turnManager.SetTimingCalled(false);
            });
        }
        else // 即発動
        {
            _turnManager.SetTimingCalled(false);
            _turnManager.TurnCharacterCommandUI(baseStatus.CharaObject, false);
            _turnManager.onChangeTurnCharacter?.Invoke(baseStatus.CharaObject);
            await UniTask.Delay(TimeSpan.FromSeconds(2)); // TODO：仮演出であり、2秒待つ
            await ult.Execute(_battleCharacter.GetEnemyCharacters().FirstOrDefault()?.CombatSystem, baseStatus);
            if (_turnManager.onNextTurnSetUp != null)
            {
                await _turnManager.onNextTurnSetUp.Invoke();
            }
        }
    }
}
