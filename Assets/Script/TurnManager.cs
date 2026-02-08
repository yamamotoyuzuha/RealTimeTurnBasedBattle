using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("BattleCameraAngleManager")]
    [SerializeField] private BattleCameraAngleManager _battleCameraAngleManager;
    [Header("キャラクターアイコン生成場所")]
    [SerializeField] private Transform iconParent;
    [Header("キャラクターアイコンPrefab")]
    [SerializeField] private GameObject characterIconObj;
    //生成したキャラクターアイコンを保持
    private Dictionary<GameObject, List<GameObject>> characterIcons = new Dictionary<GameObject, List<GameObject>>(); 
    //現在のターンのキャラクターを保持
    public GameObject CurrentTurnCharacter { get; private set; }
    //敵のキャラクターを保持　追加
    public Enemy Enemy{ get; private set; }
    //最後のターンのキャラクターを保持
    private GameObject lastTurnCharacter;
    //次のターンのキャラクターを保持
    private GameObject nextTurnCharacter;
    //キャラクターデータの保持
    private List<GameObject>  fieldCharacter = new List<GameObject>();
    private Queue<GameObject> speedCharacterTurnQueue = new Queue<GameObject>();
    /// <summary>
    /// キャラクターの情報をすべて保持
    /// </summary>
    public Dictionary<GameObject, CharacterInfo> CharacterInfos { get; private set; } = new Dictionary<GameObject, CharacterInfo>();
    
    /// <summary>
    /// 次のターンを設定するイベント
    /// </summary>
    public Action onNextTurnSetUp;
    /// <summary>
    /// Enemyのターンになった時に呼ぶ
    /// </summary>
    public Action onEnemyTurnStart;
    /// <summary>
    /// ターンアイコンの表示、非表示
    /// </summary>
    public Action<bool> onTurnIconDisplay;
    
    private void Start()
    {
        //イベントの登録
        onNextTurnSetUp = () =>
        {
            CharacterIconDestroy(CurrentTurnCharacter);
            CharacterIconSetUp();
            NextTurnCharacterSet();
        };
        onTurnIconDisplay += TurnIconDisplay;
        
        //現在のターンがEnemy場合、防御アクションUIを表示する
        if (!PlayableCharacterJudgment(CurrentTurnCharacter))
        {
            BattleOperatingInstructionsUI.Instance.DefenseActionUI(true);
        }
    }
    
    private void Update()
    {
        //デバック用　後で消す
        //ターンアイコンの生成を行う
        if (Input.GetKeyDown(KeyCode.R))
        {
            BattleStartCharacterIconGeneration();
        }

        /*
        //デバック用　後で消す
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CharacterIconDestroy(currentTurnCharacter);
            CharacterIconSetUp();
            NextTurnCharacterSet();
        }
        */
    }

    /// <summary>
    /// BattleManagerから必要な情報を取得する
    /// </summary>
    /// <param name="characters">フィールドにいるキャラクター</param>
    /// <param name="lastTurnCharacter">最後のターンのキャラクター</param>
    public void GetBattleManagerData(List<GameObject> characters, GameObject lastTurnCharacter)
    {
        fieldCharacter = characters;
        this.lastTurnCharacter = lastTurnCharacter;
        
        //速度順にソートしたキャラクターをキューに格納する
        speedCharacterTurnQueue = new Queue<GameObject>(fieldCharacter);
        //現在のターンのキャラを設定する
        CurrentTurnCharacter = speedCharacterTurnQueue.Peek();
        
        //アイコンの生成
        BattleStartCharacterIconGeneration();
        
        //各キャラクターのステートを取得する
        foreach (var character in speedCharacterTurnQueue)
        {
            //各キャラクターの情報を取得し、まとめる
            var state = character.GetComponent<CharacterState>();
            state.ChangeCharacterState(CharacterStateType.Idle);
            var status = character.GetComponent<Status>();
            var command = character.GetComponent<ICommand>(); //操作キャラ判定
            var enemyBase = character.GetComponent<EnemyBase>();
            var animChara = character.GetComponent<IAnimationCharacter>();
            var charaCamera = character.GetComponent<CharacterCameraSettings>();
            var player = command != null ? "Player" : "Enemy";
            if (player != "Player") //TODO：これはあとで外部から指定し、それをバトルマネージャー側のPlayerの攻撃に参照する
            {
                Enemy = character.GetComponent<Enemy>();
            }

            var info = new CharacterInfo
            {
                state = state,
                status = status,
                characterName = player,
                command = command,
                enemyBase = enemyBase,
                animationCharacter = animChara,
                cameraSettings = charaCamera
            };
            CharacterInfos.Add(character, info);
            info.status.GetCharacterStatus().onDeath += DeceasedCharacterSetUp;
        }
        //現在のターンのキャラのステートを選択中にしておく
        var currentChara = CharacterInfos[CurrentTurnCharacter].state;
        currentChara.ChangeCharacterState(CharacterStateType.InAction);
        var battle = _battleCameraAngleManager;
        //現在のターンがEnemyか判定を行う
        if (!PlayableCharacterJudgment(CurrentTurnCharacter))
        {
            battle.BattleCameraAngleChange(BattleCameraActiveType.EAction, battle.EActionPosF, battle.EActionPosL);
            PlayerCharacterStateChanged(CharacterStateType.BeforeAttack);
            onEnemyTurnStart?.Invoke();
        }
        else //現在のターンがプレイヤーキャラクターの場合、カメラアングルの設定を行う
        {
            battle.ResetCameraAngle();
            var cameraSet = CharacterInfos[CurrentTurnCharacter].cameraSettings;
            battle.BattleCameraAngleChange(BattleCameraActiveType.DefaultPlayer, 
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
        }
    }
    
    /// <summary>
    /// バトル開始時に呼ぶ
    /// バトル開始時に、ターン順のキャラクターアイコンを一式生成する
    /// </summary>
    private void BattleStartCharacterIconGeneration()
    {
        //生成するキャラクターアイコンの数、生成する
        int fieldCharacterCount = fieldCharacter.Count;
        int count = fieldCharacterCount * 2;
        for (int i = 0; i < count; i++)
        {
            int index = i % fieldCharacterCount; //人数を超えたら０に戻る
            GameObject character = fieldCharacter[index];
            
            //生成したアイコンをキャラクターをKeyにして保存しておく
            GameObject icon = Instantiate(characterIconObj, iconParent);
            //アイコンをキャラクターデータのアイコンに設定する
            icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(character.GetComponent<Status>().GetData());
                
            if (!characterIcons.ContainsKey(character)) //まだ、キャラクターをKeyに登録してない場合
            {
                //リストを生成する
                characterIcons[character] = new List<GameObject>();
            }
            characterIcons[character].Add(icon);
        }
    }
    
    /// <summary>
    /// ターンが終了したキャラクターアイコンを消す
    /// </summary>
    /// <param name="character">ターンを終えたキャラクター</param>
    private void CharacterIconDestroy(GameObject character)
    {
        //キャラクターが存在しない場合は、処理を行わない
        if (!characterIcons.TryGetValue(character, out List<GameObject> icons)) return;
        if (icons.Count == 0) return;
        
        //最初に追加されたアイコンを消す
        var icon = icons[0];
        icons.RemoveAt(0);
        Destroy(icon);
    }
    
    /// <summary>
    /// １ターン終了ごとにターン順のキャラクターを取得する
    /// </summary>
    private void CharacterIconSetUp()
    {
        //最後に生成したキャラクターのインデックスを取得して、次のキャラクターを取得する
        var index = fieldCharacter.IndexOf(lastTurnCharacter);
        if (index < fieldCharacter.Count - 1)
        {
            //次のキャラクターの取得
            index++;
            nextTurnCharacter = fieldCharacter[index];
        }
        else
        {
            nextTurnCharacter = fieldCharacter[0];
        }
        
        CharacterIconGenerate();
        
        //最後に生成したキャラクターの更新
        lastTurnCharacter = nextTurnCharacter;
    }
    
    /// <summary>
    /// キャラクターアイコンの生成
    /// </summary>
    private void CharacterIconGenerate()
    {
        //次のターンキャラが存在しない場合、処理を行わない
        if (!characterIcons.TryGetValue(nextTurnCharacter, out List<GameObject> icons)) return;
        
        var icon = Instantiate(characterIconObj, iconParent);
        icons.Add(icon);
        
        //キャラクターアイコンを設定する
        icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(nextTurnCharacter.GetComponent<Status>().GetData());
    }

    /// <summary>
    /// 次のターンのキャラクターを設定する
    /// </summary>
    private void NextTurnCharacterSet()
    {
        AllCharacterStateChanged(CharacterStateType.Idle);
        CtCharaNumbness();
        
        //現在のターンのキャラのステートを変更する
        var currentCharacter = CharacterInfos[CurrentTurnCharacter];
        currentCharacter.state.ChangeCharacterState(CharacterStateType.InAction);
        TurnCharacterCommandUI(CurrentTurnCharacter, true);
        var status = currentCharacter.status.GetCharacterStatus();
        var battle = _battleCameraAngleManager;
        if (!PlayableCharacterJudgment(CurrentTurnCharacter))
        {
            //パリィが出来る状態にする
            battle.BattleCameraAngleChange(BattleCameraActiveType.EAction, battle.EActionPosF, battle.EActionPosL);
            PlayerCharacterStateChanged(CharacterStateType.BeforeAttack);
            BattleOperatingInstructionsUI.Instance.DefenseActionUI(true);
            onEnemyTurnStart?.Invoke();
        }
        else
        {
            status.AddMp(1);
            battle.ResetCameraAngle();
            var cameraSet = CharacterInfos[CurrentTurnCharacter].cameraSettings;
            battle.BattleCameraAngleChange(BattleCameraActiveType.DefaultPlayer, 
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
        }
        
        if (status.IsUnderAbnormalStatus())
        {
            status.StatusEffectStart();
            Debug.Log("状態異常があるため、効果を実行");
        }
    }

    /// <summary>
    /// 次のターンキャラクターが麻痺状態だったらターンをスキップ
    /// </summary>
    private void CtCharaNumbness()
    {
        var index = speedCharacterTurnQueue.Count;
        while (index > 0)
        {
            //終了したターンのキャラを取得
            var character = speedCharacterTurnQueue.Dequeue();
            //終了したターンのキャラを末尾に追加する
            speedCharacterTurnQueue.Enqueue(character);
            //現在のターンのキャラが麻痺状態でなければ、ターンを確定する
            var cChara = speedCharacterTurnQueue.Peek();
            var status = CharacterInfos[cChara].status.GetCharacterStatus();
            if (!status.IsParalysisStatus())
            {
                CurrentTurnCharacter = cChara;
                break;
            }

            //ターンアイコンもターンスキップに連動させる
            CharacterIconDestroy(cChara);
            CharacterIconSetUp();
            //麻痺でターンがスキップされても状態異常があれば処理を行う
            if (status.IsUnderAbnormalStatus())
            {
                status.StatusEffectStart();
            }
            index--;
        }

        if (index <= 0)
        {
            Debug.LogWarning("キャラクター全員が痺れ状態");
        }
    }

    /// <summary>
    /// 死亡したキャラクターの処理
    /// ・キャラクターアイコンの削除
    /// ・ターンを管理しているキューを再構築
    /// </summary>
    /// <param name="character">死亡したキャラクター</param>>
    private void DeceasedCharacterSetUp(GameObject character)
    {
        //死亡しているキャラクターが存在している場合、処理を行う
        if (CharacterInfos.TryGetValue(character, out var info))
        {
            info.state.ChangeCharacterState(CharacterStateType.Dead);
            //死亡したキャラクターのアイコンを取得し、全て削除する
            if(!characterIcons.TryGetValue(character, out List<GameObject> icons)) return;
            foreach (var icon in icons)
            {
                Destroy(icon);
            }
            
            speedCharacterTurnQueue.Clear();
            List<GameObject> characters = new List<GameObject>();
            //死亡していないキャラクターのみ追加
            foreach (var chara in fieldCharacter)
            {
                if(!CharacterInfos.ContainsKey(chara)) continue;
                if (CharacterInfos[chara].state.characterState != CharacterStateType.Dead)
                {
                    characters.Add(chara);
                    Debug.Log(chara.name);
                }
            }
            speedCharacterTurnQueue = new Queue<GameObject>(characters);
            CurrentTurnCharacter = speedCharacterTurnQueue.Peek();
            
            //TODO：ここのところで状態異常のリストをクリアしてしまっているため、継続ダメージ処理のところでエラーが出ていると思う
            //TODO：多分
            //TODO：検証してみないとわからないかも、発動条件は状態異常の効果で倒したときにエラーが出ると思う
            
            //状態異常の解除を行ってから、死亡したキャラ情報の削除を行う
            CharacterInfos[character].status.GetCharacterStatus().StatusAilmentsClear();
            CharacterInfos.Remove(character);
        }
    }

    /// <summary>
    /// コマンド選択後と次のターンに移るときに呼ぶ
    /// キャラクターのコマンドUIの表示、非表示をする
    /// </summary>
    /// <param name="character">キャラクター</param>
    /// <param name="flag">true：表示　false：非表示</param>
    public void TurnCharacterCommandUI(GameObject character, bool flag)
    {
        if (PlayableCharacterJudgment(character))
        {
            CharacterInfos[character].command.ShowCommandUI(flag);
        }
    }

    /// <summary>
    /// キャラクターが操作キャラか判定をする
    /// </summary>
    /// <param name="character">判定を行うキャラ</param>
    /// <returns>true：操作キャラ（Player）　false：操作キャラじゃない（Enemy）</returns>
    public bool PlayableCharacterJudgment(GameObject character)
    {
        if (CharacterInfos[character].characterName == "Player") return true;
        return false;
    }

    /// <summary>
    /// 全てのキャラクターのステートを一括変更する
    /// </summary>
    /// <param name="state">変更したいステート</param>
    private void AllCharacterStateChanged(CharacterStateType state)
    {
        foreach (var chara in CharacterInfos)
        { 
            chara.Value.state.ChangeCharacterState(state);
        }
    }

    /// <summary>
    /// プレイヤーキャラクターのステートを一括変更する
    /// </summary>
    /// <param name="state">変更したいステート</param>
    private void PlayerCharacterStateChanged(CharacterStateType state)
    {
        foreach (var info in CharacterInfos)
        {
            if(info.Value.characterName == "Enemy") continue;
            
            info.Value.state.ChangeCharacterState(state);
        }
    }
    
    /// <summary>
    /// ステートに変化を加えさせたい場合に呼び出す
    /// キャラクターのステートを変更する
    /// </summary>
    /// <param name="character">現在のターンのキャラクター</param>
    /// <param name="state">変更したいステート</param>>
    private void CharacterStateChanged(GameObject character, CharacterStateType state)
    {
        /*
        //キャラクターがあったらステートに変更を加える
        if (characterStates.ContainsKey(character))
        {
            //キャラクターのステートを変更する
            characterStates[character].ChangeCharacterState(state);
        }
        */
    }

    /// <summary>
    /// キャラクターのコマンドを変更する
    /// </summary>
    /// <param name="state">入力されたコマンド</param>
    public void CharacterCommandStateChanged(CommandState state)
    {
        if(CharacterInfos[CurrentTurnCharacter].characterName != "Player") return;
        CharacterInfos[CurrentTurnCharacter].command.SetCommand(state);
    }

    /// <summary>
    /// ターンアイコンの表示、非表示
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>
    private void TurnIconDisplay(bool flag)
    {
        //全てのターンアイコンの表示切り替えを行う
        foreach (var iconsValue in characterIcons.Values)
        {
            if(iconsValue == null) continue;
            foreach (var icon in iconsValue)
            {
                if(icon != null)
                    icon.SetActive(false);
            }
        }
    }
}

/// <summary>
/// キャラクターの情報
/// </summary>
public class CharacterInfo
{
    /// <summary>
    /// キャラクターのステート
    /// </summary>
    public CharacterState state;
    /// <summary>
    /// ステータスの取得
    /// </summary>
    public Status status;
    /// <summary>
    /// Player：操作キャラ
    /// Enemy：操作キャラじゃない
    /// </summary>
    public string characterName;
    /// <summary>
    /// プレイヤーのコマンド
    /// Playerのみ
    /// </summary>
    public ICommand command;
    /// <summary>
    /// Enemyの情報
    /// Enemyのみ
    /// </summary>
    public EnemyBase enemyBase;
    /// <summary>
    /// キャラクターをアニメーション管理
    /// </summary>
    public IAnimationCharacter animationCharacter;
    /// <summary>
    /// カメラアングル設定
    /// プレイヤーキャラクターのみ
    /// </summary>
    public CharacterCameraSettings cameraSettings;
}
