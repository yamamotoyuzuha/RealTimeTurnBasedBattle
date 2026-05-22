/// <summary>
/// キャラクターの防御アクションによる処理を管理する
/// 入力、行動などの処理
/// </summary>
public class CharacterDefenseActionSystem
{
    private CharacterBaseStatus _baseStatus;
    private CharacterEventsSystem _eventsSystem;
    
    // 防御アクションの入力フラグ
    public bool IsParry { get; private set; }
    public bool IsJustGuard { get; private set; }
    // 各防御アクションのタイマー
    private float _parryTimer;
    private float _justGuardTimer;
    
    // 防御アクションの成否フラグ
    public bool ParrySuccess { get;private set; }
    public bool JustGuardSuccess { get; private set; }
    
    public CharacterDefenseActionSystem(CharacterBaseStatus baseStatus, CharacterEventsSystem eventsSystem)
    {
        this._baseStatus = baseStatus;
        this._eventsSystem = eventsSystem;
    }
    
    /// <summary>
    /// パリィの入力を検知する
    /// </summary>
    /// <param name="inputPeriod">入力を受け付ける時間</param>>
    public void ParryInput(float inputPeriod)
    {
        IsParry = true;
        _parryTimer = inputPeriod;
    }
    
    /// <summary>
    /// ジャストガードの入力を検知する
    /// </summary>
    /// <param name="inputPeriod">入力を受け付ける時間</param>>
    public void JustGuardInput(float inputPeriod)
    {
        IsJustGuard = true;
        _justGuardTimer = inputPeriod;
    }

    /// <summary>
    /// 各防御アクションのタイマーを減らす
    /// </summary>
    /// <param name="deltaTime">Time.deltaTimeを渡す</param>>
    public void UpdateDefenseActionTimer(float deltaTime)
    {
        if (IsParry)
        {
            _parryTimer -= deltaTime;
            if(_parryTimer <= 0) IsParry = false;
        }

        if (IsJustGuard)
        {
            _justGuardTimer -= deltaTime;
            if(_justGuardTimer <= 0) IsJustGuard = false;
        }
    }

    /// <summary>
    /// なんらかの防御アクションがすでに入力されているか判定を行う
    /// </summary>
    /// <returns>true：入力済み　false：入力未済</returns>
    public bool IsInputDefenseAction()
    {
        return (IsParry || IsJustGuard);
    }
    
    /// <summary>
    /// 防御アクション入力の判定
    /// ・Enemyが攻撃をする際に呼ぶ
    /// </summary>
    public DefenseActionType DefenseActionJudgment()
    {
        if (IsParry)
        {
            ParrySuccess = true;
            return DefenseActionType.Parry;
        }

        if (IsJustGuard)
        {
            JustGuardSuccess = true;
            return DefenseActionType.JustGuard;
        }

        return DefenseActionType.None;
    }

    /// <summary>
    /// 防御アクションの成否判定をリセットする
    /// </summary>
    public void DefenseActionSuccessReset()
    {
        ParrySuccess = false;
        JustGuardSuccess = false;
    }
    
    /// <summary>
    /// パリィ成功
    /// </summary>
    /// <param name="mp">増加するMP量</param>>
    public void ParrySuccessProcessing(int mp)
    {
        //AddMp(mp); 
        _eventsSystem.onParrySuccess?.Invoke(mp);
    }
    
    /// <summary>
    /// ジャストガード成功
    /// </summary>
    public void JustGuardProcessing()
    {
        _eventsSystem.onJustGuardSuccess?.Invoke();
    }
    
    /// <summary>
    /// ジャストガード成功による体幹ゲージの減少
    /// </summary>
    /// <param name="decrease">減少量</param>>
    public void CoreGaugeDecrease(float decrease)
    {
        //_currentCoreGauge -= decrease;
        //_eventsSystem.onCoreGaugeChanged?.Invoke(_currentCoreGauge, _maxCoreGauge);
    }
}
