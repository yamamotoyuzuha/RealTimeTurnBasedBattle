using UnityEngine;

/// <summary>
/// キャラクターの各システムを管理するクラス
/// ここからシステムを参照する形にする
/// </summary>
public class Character
{
    //TODO:CharacterBaseStatusではなく、Characterから参照する形に変更を行う
    
    public CharacterBaseData BaseData {get; private set;}
    public CharacterBaseStatus BaseStatus { get; private set; }
    public CharacterCombatSystem CombatSystem { get; private set; }
    public CharacterDefenseActionSystem DefenseActionSystem { get; private set; }
    public CharacterStatusEffectSystem StatusEffectSystem { get; private set; }
    public CharacterEventsSystem EventsSystem { get; private set; }
    public CharacterStateMachine StateMachine { get; private set; }
    public EnemyOnlySystem EnemyOnlySystem {get; private set;}
    public CharacterCommandSystem CommandsSystem { get; private set; }
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

    public Character(CharacterBaseData data, GameObject obj, bool isPlayer, CommandUI commandUI = null, CharacterCameraSettings cameraSettings = null)
    {
        BaseData = data;
        CharacterObject = obj;
        IsPlayer = isPlayer;
        
        BaseStatus = new CharacterBaseStatus(data.Hp, data.Mp, data.Attack, data.Defense, data.Speed, CharacterObject, 0, 0);
        EventsSystem = new CharacterEventsSystem();
        CombatSystem = new CharacterCombatSystem(BaseStatus, EventsSystem);
        DefenseActionSystem = new CharacterDefenseActionSystem(BaseStatus, EventsSystem);
        StatusEffectSystem = new CharacterStatusEffectSystem(this, BaseStatus, CombatSystem, EventsSystem);
        StateMachine = new CharacterStateMachine();
        EnemyOnlySystem = new EnemyOnlySystem();
        CommandsSystem = new CharacterCommandSystem(CommandState.None, commandUI);
        CameraSettings = cameraSettings;
    }
}
