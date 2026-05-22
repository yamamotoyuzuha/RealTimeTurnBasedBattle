
/// <summary>
/// Enemyのみしか行われない処理を管理する
/// 体幹ゲージなど
/// </summary>
public class EnemyOnlySystem
{
    /// <summary>
    /// Enemyの行動データ
    /// </summary>
    public CharacterCommandActionData CharacterCommandActionData {get; private set;}
    /// <summary>
    /// Enemyが受けた防御アクション
    /// </summary>
    public DefenseActionType ResultDefenseActionType {get; private set;}
    
    /// <summary>
    /// 最大の体幹ゲージ量
    /// </summary>
    private float _maxCoreGauge;
    /// <summary>
    /// 現在の体幹ゲージ量
    /// </summary>
    private float _currentCoreGauge;
    
    public EnemyOnlySystem()
    {
        
    }
    
    /// <summary>
    /// Enemyの行動データを設定する
    /// </summary>
    /// <param name="data">行動するデータ</param>
    public void SetActionData(CharacterCommandActionData data)
    {
        CharacterCommandActionData = data;
    }
    
    /// <summary>
    /// Enemyが受けた防御アクションの設定
    /// </summary>
    /// <param name="defenseActionType">防御アクションの種類</param>
    public void SetResultDefenseActionType(DefenseActionType defenseActionType)
    {
        ResultDefenseActionType = defenseActionType;
    }
    /// <summary>
    /// 受けた防御アクションのリセット
    /// </summary>
    public void ResetResultDefenseActionType()
    {
        ResultDefenseActionType = DefenseActionType.None;
    }
    /// <summary>
    /// 体幹ゲージのリセット
    /// </summary>
    public void CoreGaugeReSet()
    {
        _currentCoreGauge = _maxCoreGauge;
    }
}
