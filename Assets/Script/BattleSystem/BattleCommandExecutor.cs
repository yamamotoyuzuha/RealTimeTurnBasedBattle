using System;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// 選択したコマンドの実行
/// </summary>
public class BattleCommandExecutor
{
    private readonly TurnManager _turnManager;
    private readonly CommandInputManager _commandInputManager;
    private readonly BattleCameraAngleManager _battleCameraAngleManager;
    private readonly FieldSettings _fieldSettings;
    private readonly BattleCharacter _battleCharacter;
    
    public BattleCommandExecutor(TurnManager turnManager, CommandInputManager commandInputManager, 
        BattleCameraAngleManager battleCameraAngleManager, FieldSettings fieldSettings, BattleCharacter battleCharacter)
    {
        _turnManager = turnManager;
        _commandInputManager = commandInputManager;
        _battleCameraAngleManager = battleCameraAngleManager;
        _fieldSettings = fieldSettings;
        _battleCharacter = battleCharacter;
    }

    /// <summary>
    /// 魔法コマンドを実行
    /// </summary>
    /// <param name="character">コマンドを行うキャラクター</param>
    /// <returns>魔法コマンド</returns>
    public UniTask ExecuteMagic(Character character)
    {
        return MagicAction(character.BaseStatus.CharaObject);
    }

    /// <summary>
    /// 通常攻撃コマンドを実行
    /// </summary>
    /// <param name="character">コマンドを行うキャラクター</param>
    /// <returns>通常攻撃コマンド</returns>
    public UniTask ExecuteAttack(Character character)
    {
        return AttackAction(character.BaseStatus.CharaObject);
    }
    
     /// <summary>
    /// 魔法攻撃の処理
    /// </summary>
    private async UniTask MagicAction(GameObject character)
    {
        //MPを減らす
        var status = _turnManager.Characters[character].BaseStatus;
        status.ReduceMp(_commandInputManager.CurrentMagic.ConsumptionMp);
        
        //キャラクターの魔法パネルを取得し、表示する
        var magicPanel = character.GetComponent<MagicPanel>();
        magicPanel.MagicPanelToggle();
        
        //魔法パネルの表示待機時間を取得
        var magic = _commandInputManager.CurrentMagic;
        var disPlayTime = magic.MagicPanelDisplayTime;
        //魔法パネルの表示時間経過かクリアされるのどちらか早い方を待つ
        await UniTask.WhenAny(UniTask.Delay(TimeSpan.FromSeconds(disPlayTime)),
            magicPanel.MagicPanelCompleted());
        //表示時間以内に魔法パネルをクリア出来ていなかったら非表示にする
        if (!magicPanel.IsMagicPanelClear) magicPanel.MagicPanelToggle();
        
        //カメラアングルの切り替え
        var cameraSet = _turnManager.Characters[character].CameraSettings;
        _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.StartAction,
            cameraSet.StartActionCamPosF, cameraSet.StartActionCamPosL);
        
        //アニメーション関連 TODO：アニメーション周り
        //var animChara = turnManager.CharacterInfos[character].animationCharacter;
        //animChara.SetAnimationPlay(magic.CommandAnimationData._animationTriggerName);
        await UniTask.Delay(TimeSpan.FromSeconds(magic.AnimationTime));
        //エフェクトを生成
        var effect = UnityEngine.Object.Instantiate(magic.ParticleObj, _fieldSettings.EnemyCharaPos.position, Quaternion.identity);
        UnityEngine.Object.Destroy(effect, 1);
        //キャラの攻撃力を取得して、ダメージを与える
        var damage = status.Attack;
        var enemyStatus = GetEnemyCharacter();
        enemyStatus.CombatSystem.TakeDamage(damage, magic.StatusEffect);
        enemyStatus.StatusEffectSystem.IsWaterAbsorption(damage, status);
        //魔法パネルでバフがかかっていたのを元に戻す
        status.UndoAttackStatus();
        //魔法特有の効果を相手に付与する
        magic.GetMagicBaseData().MagicAction(enemyStatus.StatusEffectSystem);
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// 通常攻撃の処理
    /// </summary>
    private async UniTask AttackAction(GameObject character)
    {
        //通常攻撃のため、MPを増やす
        var playerStatus = _turnManager.Characters[character].BaseStatus;
        playerStatus.AddMp(1);
        
        //カメラアングルの切り替え
        var cameraSet = _turnManager.Characters[character].CameraSettings;
        _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.StartAction,
            cameraSet.StartActionCamPosF, cameraSet.StartActionCamPosL);
        var enemyStatus = GetEnemyCharacter();
        //アニメーション関連 TODO：ここも
        //var animChara = turnManager.CharacterInfos[character].animationCharacter;
        //animChara.SetAnimationPlay("Attack");
        await UniTask.Delay(TimeSpan.FromSeconds(2)); //アニメーション時間分待機
        enemyStatus.CombatSystem.TakeDamage(playerStatus.Attack, null);
        enemyStatus.StatusEffectSystem.IsWaterAbsorption(playerStatus.Attack, playerStatus);
        
        await UniTask.Delay(TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// EnemyのCharacterを取得する
    /// </summary>
    /// <returns>EnemyのCharacter</returns>
    private Character GetEnemyCharacter()
    {
        return _battleCharacter.GetEnemyCharacters().FirstOrDefault();
    }
}
