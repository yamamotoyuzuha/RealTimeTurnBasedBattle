using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// キャラクターの戦闘による処理を管理する
/// キャラクターのステータスやUIなどの更新を行う
/// </summary>
public class CharacterCombatSystem
{
    private CharacterBaseStatus _baseStatus;
    private CharacterEventsSystem _eventsSystem;
    
    /// <summary>
    /// 被ダメージの倍率計算
    /// </summary>
    public DamageTakenCalculation DamageTakenCalculation { get; } = new DamageTakenCalculation();
    
    public CharacterCombatSystem(CharacterBaseStatus baseStatus, CharacterEventsSystem eventsSystem)
    {
        this._baseStatus = baseStatus;
        this._eventsSystem = eventsSystem;
    }

    /// <summary>
    /// 回復処理
    /// </summary>
    /// <param name="heal"></param>
    public void TakeHeal(float heal)
    {
        
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="image"></param>>
    public void TakeDamage(float damage, Sprite image)
    {
        var finalDamage = damage * DamageTakenCalculation.DamageRate;
        var rollUpDamage = Mathf.Round(finalDamage);
        _baseStatus.Damage(rollUpDamage);
        _eventsSystem.onHpChanged?.Invoke(_baseStatus, _baseStatus.Hp, _baseStatus.MaxHp);
        _eventsSystem.onHitEffect?.Invoke();
        // ダメージUIを表示
        CharacterDamageUI.Instance.DamageUIShowDisplay(_baseStatus.CharaObject.transform, rollUpDamage, image).Forget();
        DeathDetermination();
    }
    
    /// <summary>
    /// ダメージUIを一斉に表示する
    /// </summary>
    /// <param name="damages">キャラクターごとの攻撃力</param>
    public async UniTask DamageUIAsync(List<float> damages)
    {
        // ダメージUIを一斉に表示するため、リストを作成し追加する
        List<UniTask> tasks = new List<UniTask>();
        foreach (var damage in damages)
        {
            tasks.Add(TakeDamageAsync(damage));
        }
        // 全てのダメージUIの表示が終わるまで待機する
        await UniTask.WhenAll(tasks);
    }
    
    /// <summary>
    /// キャラクターにダメージを与え、HPを減らす
    /// </summary>
    /// <param name="damage">ダメージ</param>
    public async UniTask TakeDamageAsync(float damage)
    {
        var finalDamage = damage * DamageTakenCalculation.DamageRate;
        var rollUpDamage = Mathf.Round(finalDamage);
        _baseStatus.Damage(rollUpDamage);
        _eventsSystem.onHpChanged?.Invoke(_baseStatus, _baseStatus.Hp, _baseStatus.MaxHp);
        _eventsSystem.onHitEffect?.Invoke();
        // ダメージUIを表示
        await CharacterDamageUI.Instance.DamageUIShowDisplay(_baseStatus.CharaObject.transform, rollUpDamage, null);
        DeathDetermination();
    }

    /// <summary>
    /// 死亡判定
    /// </summary>
    private void DeathDetermination()
    {
        if (_baseStatus.Hp <= 0)
        {
            _eventsSystem.onDeathEffect?.Invoke();
            _eventsSystem.onDeath?.Invoke(_baseStatus.CharaObject);
            Debug.LogWarning("死亡した");
        }
    }
}
