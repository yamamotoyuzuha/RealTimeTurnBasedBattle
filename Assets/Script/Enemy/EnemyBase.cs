using System;

public interface EnemyBase
{
    /// <summary>
    /// Enemyの行動を開始
    /// </summary>
    Action OnEnemyTurnAction { get; }
    
    /// <summary>
    /// Enemyの行動が終了
    /// </summary>
    Action OnEnemyTurnEnd { get; set; }
    
    /// <summary>
    /// Enemyがプレイヤーに与えるダメージ
    /// </summary>
    Action OnEnemyAttackDamage { get; }

    /// <summary>
    /// Actionを登録
    /// </summary>
    /// <param name="action">登録するAction</param>
    public void RegisterActionAttackDamage(Action action);
    /// <summary>
    /// Actionを登録解除
    /// </summary>
    /// <param name="action">登録解除するAction</param>
    public void UnsubscribeActioAttackDamage(Action action);

    /// <summary>
    /// 単体攻撃か全体攻撃かの判定を行う
    /// </summary>
    /// <returns>true：単体　false：全体</returns>
    public bool IsIndividualOrWhole();
}
