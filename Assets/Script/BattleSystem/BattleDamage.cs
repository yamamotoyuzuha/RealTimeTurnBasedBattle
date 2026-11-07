using UnityEngine;

/// <summary>
/// 戦闘によるダメージ管理
/// </summary>
public class BattleDamage : MonoBehaviour
{
    [SerializeField] private FieldSettings _fieldSettings;
    private BattleCharacter _battleCharacter;

    public void GetBattleCharacter(BattleCharacter battleCharacter)
    {
        _battleCharacter = battleCharacter;
    }
    
    /// <summary>
    /// Enemyの攻撃ダメージ処理
    /// </summary>
    public void EnemyAttackDamageCalculation(EnemyBase enemyBase, Character enemyStatus)
    {
        var players = _battleCharacter.GetPlayerCharacters();
        if (players.Count == 0) return;

        if (enemyBase.IsIndividualOrWhole())
        {
            // 単体攻撃
            int index = Random.Range(0, players.Count);

            AttackDamageTargetCheck(players[index], enemyStatus, index);
        }
        else
        {
            // 全体攻撃
            for (int i = 0; i < players.Count; i++)
            {
                AttackDamageTargetCheck(players[i], enemyStatus, i);
            }
        }
    }

    /// <summary>
    /// 攻撃対象にダメージを与えるか判定
    /// </summary>
    /// <param name="targetStatus">ダメージを与えられる攻撃対象のステータス</param>
    /// <param name="enemyStatus">ダメージを与えるEnemyのステータス</param>
    /// <param name="index">攻撃対象のインデックス</param>
    private void AttackDamageTargetCheck(Character targetStatus, Character enemyStatus, int index)
    {
        var pDefense = targetStatus.DefenseActionSystem;
        var eOnly = enemyStatus.EnemyOnlySystem;

        // 防御アクションの判定
        var dAction = pDefense.DefenseActionJudgment();
        var aData = eOnly.CharacterCommandActionData;

        switch (dAction)
        {
            case DefenseActionType.Parry:
                if (!DefenseActionCheck(dAction, aData))
                {
                    TargetDamage(targetStatus, enemyStatus, aData, index);
                    return;
                }

                // パリィ成功
                pDefense.ParrySuccessProcessing(1);
                eOnly.SetResultDefenseActionType(DefenseActionType.Parry);
                break;

            case DefenseActionType.JustGuard:
                if (!DefenseActionCheck(dAction, aData))
                {
                    TargetDamage(targetStatus, enemyStatus, aData, index);
                    return;
                }

                // ジャストガード成功
                pDefense.JustGuardProcessing();
                eOnly.SetResultDefenseActionType(DefenseActionType.JustGuard);
                // Enemyの体幹ゲージを減らす
                eOnly.CoreGaugeDecrease(targetStatus.BaseStatus.Attack);
                break;

            default:
                TargetDamage(targetStatus, enemyStatus, aData, index);
                break;
        }
    }

    /// <summary>
    /// 防御アクションが可能かどうかの判定
    /// </summary>
    /// <param name="type">防御アクションの種類</param>
    /// <param name="data">発動する魔法</param>
    /// <returns>true：可能　false：不可能</returns>
    private bool DefenseActionCheck(DefenseActionType type, CharacterCommandActionData data)
    {
        var magic = data.GetMagicBaseData();
        return magic.IsDefenceActionPossible(type);
    }

    /// <summary>
    /// ターゲットにダメージを与える
    /// </summary>
    /// <param name="targetStatus">ターゲットのステータス</param>
    /// <param name="enemyStatus">Enemyのステータス</param>
    /// <param name="data">コマンドの内容</param>
    /// <param name="index">ターゲットの位置</param>
    private void TargetDamage(Character targetStatus, Character enemyStatus, CharacterCommandActionData data, int index)
    {
        if (targetStatus == null || enemyStatus == null || data == null) return;
        
        var enemyAttack = enemyStatus.BaseStatus.Attack;
        switch (data.GetCommandType())
        {
            case CharacterCommandActionType.Magic:
                var magic = data.GetMagicBaseData();
                targetStatus.CombatSystem.TakeDamage(enemyAttack, magic.StatusEffect);
                magic.MagicAction(targetStatus.StatusEffectSystem);

                // エフェクトの生成
                if (index < 0 || index >= _fieldSettings.PlayerCharaPos.Length)
                {
                    Debug.LogWarning("プレイヤーのエフェクト位置が見つからない");
                    return;
                }
                var effect = Instantiate(magic.ParticleObj, _fieldSettings.PlayerCharaPos[index].position, Quaternion.identity);
                Destroy(effect, 1);
                break;
        }
    }
}
