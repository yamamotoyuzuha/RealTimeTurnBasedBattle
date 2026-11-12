using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("BattleCameraAngleManager")]
    [SerializeField] private BattleCameraAngleManager _battleCameraAngleManager;
    [Header("FieldSettings")]
    [SerializeField] private FieldSettings _fieldSettings;
    //現在のターンのキャラクターを保持
    public GameObject CurrentTurnCharacter { get; private set; }
    //敵のキャラクターを保持　追加
    public Enemy Enemy{ get; private set; }
    //最後のターンのキャラクターを保持
    private GameObject _lastTurnCharacter;
    //次のターンのキャラクターを保持
    private GameObject _nextTurnCharacter;
    //キャラクターの保持
    private List<GameObject> _fieldCharacter = new List<GameObject>(); //最新のターン順
    private List<GameObject> _oldFieldCharacter = new List<GameObject>(); //古いターン順
    private Queue<GameObject> _speedCharacterTurnQueue = new Queue<GameObject>();
    /// <summary>
    /// 割り込みがあったキャラクターの必殺技を保持
    /// </summary>
    private Queue<Func<UniTask>> _interruptUltQueue = new Queue<Func<UniTask>>();
    /// <summary>
    /// 割り込みなどあったか
    /// true：あった　false：なかった
    /// </summary>
    private bool _isInterruptionsEtc;
    /// <summary>
    /// 割り込んだタイミング
    /// true：ターン中　ターン開始前
    /// </summary>
    private bool _isTimingCalledInterruptions;
    /// <summary>
    /// 割り込みなどを行ったキャラクター
    /// </summary>
    private GameObject _interruptionsEtcChara;
    /// <summary>
    /// 割り込みなどが行われる前に現在のターンだったキャラクター
    /// </summary>
    private GameObject _beforeInterruptionChara;
    
    /// <summary>
    /// 割り込みの判定
    /// true：開始中　false：終了
    /// </summary>
    public bool IsTurnStarting {get; private set;}
    
    /// <summary>
    /// キャラクターの情報を保持
    /// </summary>
    public Dictionary<GameObject, Character> Characters { get; private set; } = new Dictionary<GameObject, Character>();

    /// <summary>
    /// 操作キャラクターのコマンド入力を可否
    /// true：可能　false：不可能
    /// </summary>
    public Action<bool> onCommandInputPossible;
    /// <summary>
    /// 次のターンを設定するイベント
    /// </summary>
    public Func<UniTask> onNextTurnSetUp;
    /// <summary>
    /// Enemyのターンになった時に呼ぶ
    /// </summary>
    public Action onEnemyTurnStart;
    /// <summary>
    /// ターンアイコンの表示、非表示
    /// </summary>
    public Action<bool> onTurnIconDisplay;
    /// <summary>
    /// ターン順の更新を行う
    /// </summary>
    public Action onUpdateTurnOrder;
    /// <summary>
    /// ターンキャラクターの変更を行う
    /// ・GameObjectには変更後のキャラクターを設定
    /// ・必殺技や割り込みなど
    /// </summary>
    public Action<GameObject> onChangeTurnCharacter;
    

    // Turn関連クラス
    [SerializeField] private TurnIconManager _turnIconManager;
    private void Start()
    {
        //イベントの登録
        onNextTurnSetUp = async () =>
        {
            if (_isInterruptionsEtc)
            {
                _turnIconManager.RemoveSpecifiedCharacterIcon(_interruptionsEtcChara);
                await RestoreBeforeInterruptSet();
                _isInterruptionsEtc = false;
            }
            else
            {
                _turnIconManager.DestroyCharacterIcon(CurrentTurnCharacter);
                CharacterIconSetUp();
                await NextTurnCharacterSet();
            }
        };
        onTurnIconDisplay += _turnIconManager.DisplayCharacterIcon;
        onUpdateTurnOrder = () =>
        {
            RecalculatingTurnOrder();
            ResetTurnIcon();
        };
        onChangeTurnCharacter = (chara) =>
        {
            ChangeCurrentTurnCharacter(chara);
            _turnIconManager.GenerateSpecifiedCharacterIcon(chara);
        };
        
        //現在のターンがEnemy場合、防御アクションUIを表示する
        if (!PlayableCharacterJudgment(CurrentTurnCharacter))
        {
            BattleOperatingInstructionsUI.Instance.DefenseActionUI(true);
        }
        else
        {
            BattleOperatingInstructionsUI.Instance.DefenseActionUI(false);
        }
    }
    
    private void Update()
    {
        //TODO：デバッグ用
        if (Input.GetKeyDown(KeyCode.T))
        {
            //TODO：デバッグのため、適当に指定しているが本番ではちゃんと合ってるキャラクターを指定する
            ChangeCurrentTurnCharacter(_fieldCharacter[0]);
            _turnIconManager.GenerateSpecifiedCharacterIcon(_fieldCharacter[0]);
        }
    }

    /// <summary>
    /// BattleManagerから必要な情報を取得する
    /// </summary>
    /// <param name="characters">フィールドにいるキャラクター</param>
    /// <param name="lastTurnCharacter">最後のターンのキャラクター</param>
    public void GetBattleManagerData(List<GameObject> characters, GameObject lastTurnCharacter)
    {
        _fieldCharacter = characters;
        this._lastTurnCharacter = lastTurnCharacter;
        _lastTurnCharacter = lastTurnCharacter;
        
        //速度順にソートしたキャラクターをキューに格納する
        _speedCharacterTurnQueue = new Queue<GameObject>(_fieldCharacter);
        //現在のターンのキャラを設定する
        CurrentTurnCharacter = _speedCharacterTurnQueue.Peek();
        _turnIconManager.GenerateIcons(characters);
        
        //アイコンの生成
        BattleStartCharacterIconGeneration();
        var index = 0; //キャラクター位置のインデックスを保持
        //各キャラクターのステートを取得する
        foreach (var character in _speedCharacterTurnQueue)
        {
            var chara = character.GetComponent<Status>().GetCharacter();
            Characters.Add(character, chara);
            Characters[character].EventsSystem.onDeath += DeceasedCharacterSetUp;
            if (!Characters[character].IsPlayer) // Enemyの場合
            {
                character.transform.position = _fieldSettings.EnemyCharaPos.position;
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else // Playerの場合、必殺技ゲージのチャージとキャラの立ち位置を設定する
            {
                character.transform.position = _fieldSettings.PlayerCharaPos[index].position;
                index++;
            }

            // TODO：ここの下らへん上手くCharacterから参照出来ているかわからない
            // TODO：多分、StateとCharacterCameraSettings、ICommandあたりは取得できているはず
            /* 
            //各キャラクターの情報を取得し、まとめる
            var state = character.GetComponent<CharacterState>();
            state.ChangeCharacterState(CharacterStateType.Idle);
            var status = character.GetComponent<Status>();
            var command = character.GetComponent<ICommand>(); //操作キャラ判定
            var enemyBase = character.GetComponent<EnemyBase>();
            var animChara = character.GetComponent<IAnimationCharacter>();
            var charaCamera = character.GetComponent<CharacterCameraSettings>();
            var player = command != null ? "Player" : "Enemy";
            if (player != "Player") //TODO：これはあとで外部から指定し、それをバトルマネージャー側のPlayerの攻撃に参照する　これなにをしたいんだ
            {
                Enemy = character.GetComponent<Enemy>();
                character.transform.position = _fieldSettings.EnemyCharaPos.position;
                character.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                character.transform.position = _fieldSettings.PlayerCharaPos[index].position;
                index++;
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
            */
        }
        
        // 現在のターンのキャラのステートを選択中にしておく
        var currentCharaState = Characters[CurrentTurnCharacter].StateMachine;
        currentCharaState.ChangeCharacterState(CharacterStateType.InAction);
        var battle = _battleCameraAngleManager;
        // 現在のターンがEnemyか判定を行う
        if (!PlayableCharacterJudgment(CurrentTurnCharacter))
        {
            battle.BattleCameraAngleChange(BattleCameraActiveType.EAction, battle.EActionPosF, battle.EActionPosL);
            PlayerCharacterStateChanged(CharacterStateType.BeforeAttack);
            onEnemyTurnStart?.Invoke();
        }
        else // 現在のターンがプレイヤーキャラクターの場合、操作が出来る状態にする
        {
            battle.ResetCameraAngle();
            var cameraSet = Characters[CurrentTurnCharacter].CameraSettings;
            battle.BattleCameraAngleChange(BattleCameraActiveType.DefaultPlayer,
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
            TurnCharacterCommandUI(CurrentTurnCharacter, true);
            onCommandInputPossible?.Invoke(true);
        }
    }

    /// <summary>
    /// ターン開始フラグを設定
    /// </summary>
    /// <param name="flag">true：開始　false：終了</param>
    public void SetTurnStartingFlag(bool flag)
    {
        IsTurnStarting = flag;
    }

    /// <summary>
    /// 割り込みがどのタイミングで呼ばれたか設定する
    /// </summary>
    /// <param name="flag">true：ターン中　false：ターン開始前</param>
    public void SetTimingCalled(bool flag)
    {
        _isTimingCalledInterruptions = flag;
    }
    
    /// <summary>
    /// １ターン終了ごとにターン順のキャラクターを取得する
    /// </summary>
    private void CharacterIconSetUp()
    {
        //最後に生成したキャラクターのインデックスを取得して、次のキャラクターを取得する
        var index = _fieldCharacter.IndexOf(_lastTurnCharacter);
        if (index < _fieldCharacter.Count - 1)
        {
            //次のキャラクターの取得
            index++;
            _nextTurnCharacter = _fieldCharacter[index];
        }
        else
        {
            _nextTurnCharacter = _fieldCharacter[0];
        }
        _turnIconManager.GenerateCharacterIcon(_nextTurnCharacter);
        
        //最後に生成したキャラクターの更新
        _lastTurnCharacter = _nextTurnCharacter;
    }
    
    /// <summary>
    /// 割り込みがあったキャラクターの必殺技を追加
    /// </summary>
    /// <param name="action"></param>
    public void SetInterruptCharacterUlt(Func<UniTask> action)
    {
        // 必殺技を追加する
        _interruptUltQueue.Enqueue(action);
    }

    /// <summary>
    /// 割り込みがあったキャラクターの必殺技を発動
    /// </summary>
    private async UniTask InterruptCharacterUltAction()
    {
        // 割り込みしたキャラクターの必殺技を発動
        while (_interruptUltQueue.Count > 0)
        {
            // 必殺技を取得し、発動
            var interruptUlt =  _interruptUltQueue.Dequeue();
            await interruptUlt.Invoke();
        }
    }

    /// <summary>
    /// 次のターンのキャラクターを設定する
    /// </summary>
    private async UniTask NextTurnCharacterSet()
    {
        await InterruptCharacterUltAction();
        
        AllCharacterStateChanged(CharacterStateType.Idle);
        CtCharaNumbness();
        
        // 現在のターンのキャラのステートを変更する
        var currentCharacter = Characters[CurrentTurnCharacter];
        currentCharacter.StateMachine.ChangeCharacterState(CharacterStateType.InAction);
        TurnCharacterCommandUI(CurrentTurnCharacter, true);
        var status = currentCharacter.BaseStatus;
        var effect = currentCharacter.StatusEffectSystem;
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
            currentCharacter.UltimateSystem.UltimateCharge();
            battle.ResetCameraAngle();
            var cameraSet = Characters[CurrentTurnCharacter].CameraSettings;
            battle.BattleCameraAngleChange(BattleCameraActiveType.DefaultPlayer, 
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
        }
        
        if (effect.IsUnderAbnormalStatus())
        {
            effect.StatusEffectStart();
            Debug.Log("状態異常があるため、効果を実行");
        }
    }

    /// <summary>
    /// 割り込みが行われる前のターン状態に戻す
    /// </summary>
    private async UniTask RestoreBeforeInterruptSet()
    {
        await InterruptCharacterUltAction();
        
        AllCharacterStateChanged(CharacterStateType.Idle);
        CtCharaNumbness();
        
        // 現在のターンのキャラのステートを変更する
        var currentCharacter = Characters[CurrentTurnCharacter];
        currentCharacter.StateMachine.ChangeCharacterState(CharacterStateType.InAction);
        TurnCharacterCommandUI(CurrentTurnCharacter, true);
        var status = currentCharacter.BaseStatus;
        var effect = currentCharacter.StatusEffectSystem;
        var battle = _battleCameraAngleManager;
        if (!PlayableCharacterJudgment(CurrentTurnCharacter))
        {
            // パリィが出来る状態にする
            battle.BattleCameraAngleChange(BattleCameraActiveType.EAction, battle.EActionPosF, battle.EActionPosL);
            PlayerCharacterStateChanged(CharacterStateType.BeforeAttack);
            BattleOperatingInstructionsUI.Instance.DefenseActionUI(true);
            onEnemyTurnStart?.Invoke();
        }
        else
        {
            status.AddMp(1);
            battle.ResetCameraAngle();
            var cameraSet = Characters[CurrentTurnCharacter].CameraSettings;
            battle.BattleCameraAngleChange(BattleCameraActiveType.DefaultPlayer, 
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
        }
        
        if (effect.IsUnderAbnormalStatus())
        {
            effect.StatusEffectStart();
            Debug.Log("状態異常があるため、効果を実行");
        }
    }

    //TODO：この関数を使って、もし変更がある場合、ターン順に変更があったときのActionを呼ぶ
    /// <summary>
    /// キャラクターの速度が変更されたかを判定する
    /// </summary>
    /// <returns>true：変更なし　false：変更あり</returns>
    private bool CheckSpeedCharacters()
    {
        // 現在のターン順リストとターン順を計算したリストの中身と順番が一致しているか判定する
        List<GameObject> characters = new List<GameObject>(_fieldCharacter);
        characters = characters.OrderByDescending(i =>
            Characters[i].BaseData.Speed).ToList();
        return _fieldCharacter.SequenceEqual(characters);
    }

    /// <summary>
    /// ターン順の再計算
    /// </summary>
    private void RecalculatingTurnOrder()
    {
        _oldFieldCharacter = _fieldCharacter; //最新のターン順になる前に元状態のものを保持
        _fieldCharacter = _fieldCharacter.OrderByDescending(i => 
            i.GetComponent<Status>().GetCharacter().BaseData.Speed).ToList();
        _speedCharacterTurnQueue = new Queue<GameObject>(_fieldCharacter);
        //最後のターンキャラの設定
        var index = _fieldCharacter.Count - 1;
        _lastTurnCharacter = _fieldCharacter[index];
    }

    /// <summary>
    /// ターンアイコンの再設定
    /// </summary>
    private void ResetTurnIcon()
    {
        Dictionary<GameObject, Queue<GameObject>> iconQueues = new Dictionary<GameObject, Queue<GameObject>>();
        foreach (var chara in _fieldCharacter) //新しいターン順
        {
            iconQueues[chara] = new Queue<GameObject>(_turnIconManager.GetCharacterIcons(chara));
        }

        int siblingIndex = 0; //UIの順番を保持
        bool iconsRemaining = true; //処理していないアイコンが残っているか判定

        while (iconsRemaining)
        {
            iconsRemaining = false;
            foreach (var chara in _fieldCharacter) //新しいターン順
            {
                if (iconQueues[chara].Count > 0) //まだ、未配置のアイコンがある場合
                {
                    //アイコンが残っていることにし、キャラのアイコンを取り出す
                    iconsRemaining = true;
                    var icon = iconQueues[chara].Dequeue();
                    var charaData = Characters[chara].BaseData;
                    //アイコン更新
                    icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(charaData);
                    //UI上の順番を更新
                    icon.transform.SetSiblingIndex(siblingIndex);
                    siblingIndex++;
                }
            }
        }
    }

    /// <summary>
    /// 現在のターンキャラクターを変更する
    /// ・必殺技による割り込みなど
    /// </summary>
    /// <param name="character">変更するキャラクター</param>>
    private void ChangeCurrentTurnCharacter(GameObject character)
    {
        //割り込みなどがあったことを保持しておく
        _isInterruptionsEtc = true;
        _interruptionsEtcChara = character;
        _beforeInterruptionChara = CurrentTurnCharacter;
        //現在のターンのキャラクターを設定する
        AllCharacterStateChanged(CharacterStateType.Idle);
        CurrentTurnCharacter = character;
        //現在のターンのキャラのステートを変更する
        var currentCharacter = Characters[CurrentTurnCharacter];
        currentCharacter.StateMachine.ChangeCharacterState(CharacterStateType.InAction);
        
        //カメラを設定する
        var cameraSet = Characters[CurrentTurnCharacter].CameraSettings;
        if (!PlayableCharacterJudgment(CurrentTurnCharacter)) //プレイヤーかEnemyかの判定を行う
        {
            _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.EAction, 
                _battleCameraAngleManager.EActionPosF, _battleCameraAngleManager.EActionPosL);
            PlayerCharacterStateChanged(CharacterStateType.BeforeAttack);
            BattleOperatingInstructionsUI.Instance.DefenseActionUI(true);
            onEnemyTurnStart?.Invoke();
        }
        else // プレイヤーキャラクター
        {
            _battleCameraAngleManager.BattleCameraAngleChange(BattleCameraActiveType.DefaultPlayer,
                cameraSet.DefaultCamPosF, cameraSet.DefaultCamPosL);
        }
    }

    /// <summary>
    /// 次のターンキャラクターが麻痺状態だったらターンをスキップ
    /// </summary>
    private void CtCharaNumbness()
    {
        var index = _speedCharacterTurnQueue.Count;
        var flag = false; //割り込み前のキャラクターの判定を行ったか
        while (index > 0)
        {
            if (_isInterruptionsEtc)
            {
                if (!flag) //割り込み前のキャラクターの判定を行う
                {
                    flag = true;

                    // 割り込み前のキャラクターを取得
                    var character = _beforeInterruptionChara;
                    var status = Characters[character].StatusEffectSystem;
                    // 必殺技が発動されるのは割り込み前のキャラクターがターン終了時のため、割り込み前のキャラクターをずらす
                    if (_isTimingCalledInterruptions)
                    {
                        // 割り込み前のキャラクターを取得
                        var charaIndex = _fieldCharacter.IndexOf(_beforeInterruptionChara);
                        if (charaIndex == -1)
                        {
                            Debug.LogError("キャラが見つからない");
                            return;
                        }

                        // 割り込み前のキャラクターの次のキャラクターを取得
                        var nextIndex = (charaIndex + 1) % _fieldCharacter.Count;
                        var nextCharacter = _fieldCharacter[nextIndex];
                        // 次のキャラクターが麻痺の可能性もあるため、character等の更新を行う
                        character = nextCharacter;
                        status = Characters[nextCharacter].StatusEffectSystem;
                        if (!status.IsParalysisStatus())
                        {
                            CurrentTurnCharacter = nextCharacter;
                            Debug.LogWarning(CurrentTurnCharacter + "麻痺判定の処理");
                            break;
                        }
                    }
                    else // 必殺技の発動がターン開始前の場合
                    {
                        // 割り込み前のキャラが麻痺状態でない場合、そのまま割り込み前のキャラが現在ターンキャラになる
                        if (!status.IsParalysisStatus())
                        {
                            CurrentTurnCharacter = character;
                            Debug.LogWarning(CurrentTurnCharacter + "麻痺判定の処理");
                            break;
                        }
                    }
                    
                    //ターンアイコンもターンスキップに連動させる
                    _turnIconManager.DestroyCharacterIcon(character);
                    CharacterIconSetUp();
                    
                    //麻痺でターンがスキップされても状態異常があれば処理を行う
                    if (status.IsUnderAbnormalStatus())
                    {
                        status.StatusEffectStart();
                    }
                }
                else
                {
                    //終了したターンのキャラを取得
                    var character = _speedCharacterTurnQueue.Dequeue();
                    //終了したターンのキャラを末尾に追加する
                    _speedCharacterTurnQueue.Enqueue(character);
                    //現在のターンのキャラが麻痺状態でなければ、ターンを確定する
                    var cChara = _speedCharacterTurnQueue.Peek();
                    var status = Characters[cChara].StatusEffectSystem;
                    if (!status.IsParalysisStatus())
                    {
                        CurrentTurnCharacter = cChara;
                        break;
                    }

                    //ターンアイコンもターンスキップに連動させる
                    _turnIconManager.DestroyCharacterIcon(cChara);
                    CharacterIconSetUp();
                    //麻痺でターンがスキップされても状態異常があれば処理を行う
                    if (status.IsUnderAbnormalStatus())
                    {
                        status.StatusEffectStart();
                    }
                    index--;
                }
            }
            else
            {
                //終了したターンのキャラを取得
                var character = _speedCharacterTurnQueue.Dequeue();
                //終了したターンのキャラを末尾に追加する
                _speedCharacterTurnQueue.Enqueue(character);
                //現在のターンのキャラが麻痺状態でなければ、ターンを確定する
                var cChara = _speedCharacterTurnQueue.Peek();
                var status = Characters[cChara].StatusEffectSystem;
                if (!status.IsParalysisStatus())
                {
                    CurrentTurnCharacter = cChara;
                    break;
                }

                //ターンアイコンもターンスキップに連動させる
                _turnIconManager.DestroyCharacterIcon(cChara);
                CharacterIconSetUp();
                //麻痺でターンがスキップされても状態異常があれば処理を行う
                if (status.IsUnderAbnormalStatus())
                {
                    status.StatusEffectStart();
                }
                index--;
            }
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
        if(!Characters.TryGetValue(character, out var info)) return;
        
        // キャラクターを死亡扱い
        info.StateMachine.ChangeCharacterState(CharacterStateType.Dead);
        // アイコンとキャラクターの削除
        _turnIconManager.DeathCharacterIconDestroy(character);
        _turnOrderManager.RemoveCharacter(character);
        // 生存しているキャラクターのみでターン順を再構築
        _fieldCharacter = _turnOrderManager.TurnOrder.ToList();
        CurrentTurnCharacter = _turnOrderManager.CurrentTurnCharacter;
        // 状態異常の解除を行ってから、死亡したキャラ情報の削除を行う
        info.StatusEffectSystem.StatusAilmentsClear();
        
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
            Characters[character].CommandsSystem.ShowCommandUI(flag);
        }
    }

    /// <summary>
    /// キャラクターが操作キャラか判定をする
    /// </summary>
    /// <param name="character">判定を行うキャラ</param>
    /// <returns>true：操作キャラ（Player）　false：操作キャラじゃない（Enemy）</returns>
    public bool PlayableCharacterJudgment(GameObject character)
    {
        if(Characters[character].IsPlayer) return true;
        return false;
    }

    /// <summary>
    /// 全てのキャラクターのステートを一括変更する
    /// </summary>
    /// <param name="state">変更したいステート</param>
    private void AllCharacterStateChanged(CharacterStateType state)
    {
        foreach (var character in Characters.Values)
        {
            character.StateMachine.ChangeCharacterState(state);
        }
    }

    /// <summary>
    /// プレイヤーキャラクターのステートを一括変更する
    /// </summary>
    /// <param name="state">変更したいステート</param>
    private void PlayerCharacterStateChanged(CharacterStateType state)
    {
        foreach (var character in Characters.Values)
        {
            if(!character.IsPlayer) continue;
            character.StateMachine.ChangeCharacterState(state);
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
        if(!Characters[CurrentTurnCharacter].IsPlayer) return;
        Characters[CurrentTurnCharacter].CommandsSystem.ChangeCommandState(state);
    }
}
