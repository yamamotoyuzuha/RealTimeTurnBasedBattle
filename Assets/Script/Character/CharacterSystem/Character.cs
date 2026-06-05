using UnityEngine;

/// <summary>
/// キャラクターの各システムを管理するクラス
/// ここからシステムを参照する形にする
/// </summary>
public class Character
{
    public CharacterBaseData BaseData {get; private set;}
    public CharacterBaseStatus BaseStatus { get; private set; }
    public CharacterCombatSystem CombatSystem { get; private set; }
    public CharacterDefenseActionSystem DefenseActionSystem { get; private set; }
    public CharacterStatusEffectSystem StatusEffectSystem { get; private set; }
    public CharacterEventsSystem EventsSystem { get; private set; }
    public CharacterStateMachine StateMachine { get; private set; }
    public EnemyOnlySystem EnemyOnlySystem {get; private set;}
    public CharacterCommandSystem CommandsSystem { get; private set; }
    public CharacterUltimateSystem UltimateSystem { get; private set; }
    public CharacterCameraSettings CameraSettings { get; private set; }
    
    /// <summary>
    /// キャラクター
    /// </summary>
    public GameObject CharacterObject { get; private set; }
    
    /// <summary>
    /// プレイヤーか敵かの判定
    /// true：プレイヤー　false：敵
    /// </summary>
    public bool IsPlayer { get; private set; }

    public Character(CharacterBaseData data, GameObject obj, bool isPlayer, CommandUI commandUI = null,
        CharacterCameraSettings cameraSettings = null, float core = 0)
    {
        BaseData = data;
        CharacterObject = obj;
        IsPlayer = isPlayer;
        
        EventsSystem = new CharacterEventsSystem();
        BaseStatus = new CharacterBaseStatus(EventsSystem, data.Hp, data.Mp, data.Attack, data.Defense, data.Speed, CharacterObject);
        CombatSystem = new CharacterCombatSystem(BaseStatus, EventsSystem);
        DefenseActionSystem = new CharacterDefenseActionSystem(BaseStatus, EventsSystem);
        StatusEffectSystem = new CharacterStatusEffectSystem(this, BaseStatus, CombatSystem, EventsSystem);
        StateMachine = new CharacterStateMachine();
        EnemyOnlySystem = new EnemyOnlySystem(EventsSystem, core);
        CommandsSystem = new CharacterCommandSystem(CommandState.None, commandUI);
        UltimateSystem = new CharacterUltimateSystem(BaseStatus, EventsSystem, data.UltimateBaseData, data.UltimateMaxGauge, data.UltimateCharge);
        CameraSettings = cameraSettings;
    }
}
