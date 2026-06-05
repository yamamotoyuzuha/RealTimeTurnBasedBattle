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
    [Header("キャラクターアイコン生成場所")]
    [SerializeField] private Transform _iconParent;
    [Header("キャラクターアイコンPrefab")]
    [SerializeField] private GameObject _characterIconObj;
    //生成したキャラクターアイコンを保持
    private Dictionary<GameObject, List<GameObject>> _characterIcons = new Dictionary<GameObject, List<GameObject>>(); 
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
    /// 割り込みなどを行ったキャラクター
    /// </summary>
    private GameObject _interruptionsEtcChara;
    /// <summary>
    /// 割り込みなどが行われる前に現在のターンだったキャラクター
    /// </summary>
    private GameObject _beforeInterruptionChara;

    // TODO：これをターン開始時に使用する
    // TODO：これがfasleの場合は、必殺技を即発動
    /// <summary>
    /// ターン開始
    /// true：開始中　false：開始してない
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
    
    private void Start()
    {
        //イベントの登録
        onNextTurnSetUp = async () =>
        {
            if (_isInterruptionsEtc)
            {
                RemoveSpecifiedCharacterIcon(_interruptionsEtcChara);
                RestoreBeforeInterruptSet();
                _isInterruptionsEtc = false;
            }
            else
            {
                CharacterIconDestroy(CurrentTurnCharacter);
                CharacterIconSetUp();
                await NextTurnCharacterSet();
            }
        };
        onTurnIconDisplay += TurnIconDisplay;
        onUpdateTurnOrder = () =>
        {
            RecalculatingTurnOrder();
            ResetTurnIcon();
        };
        onChangeTurnCharacter = (chara) =>
        {
            ChangeCurrentTurnCharacter(chara);
            GenerateSpecifiedCharacterIcon(chara);
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
            GenerateSpecifiedCharacterIcon(_fieldCharacter[0]);
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
        
        //速度順にソートしたキャラクターをキューに格納する
        _speedCharacterTurnQueue = new Queue<GameObject>(_fieldCharacter);
        //現在のターンのキャラを設定する
        CurrentTurnCharacter = _speedCharacterTurnQueue.Peek();
        
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
    /// バトル開始時に呼ぶ
    /// バトル開始時に、ターン順のキャラクターアイコンを一式生成する
    /// </summary>
    private void BattleStartCharacterIconGeneration()
    {
        //生成するキャラクターアイコンの数、生成する
        int fieldCharacterCount = _fieldCharacter.Count;
        int count = fieldCharacterCount * 2;
        for (int i = 0; i < count; i++)
        {
            int index = i % fieldCharacterCount; //人数を超えたら０に戻る
            GameObject character = _fieldCharacter[index];
            
            //生成したアイコンをキャラクターをKeyにして保存しておく
            GameObject icon = Instantiate(_characterIconObj, _iconParent);
            //アイコンをキャラクターデータのアイコンに設定する
            icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(character.GetComponent<Status>().GetCharacter().BaseData);
                
            if (!_characterIcons.ContainsKey(character)) //まだ、キャラクターをKeyに登録してない場合
            {
                //リストを生成する
                _characterIcons[character] = new List<GameObject>();
            }
            _characterIcons[character].Add(icon);
        }
    }
    
    /// <summary>
    /// ターンが終了したキャラクターアイコンを消す
    /// </summary>
    /// <param name="character">ターンを終えたキャラクター</param>
    private void CharacterIconDestroy(GameObject character)
    {
        //キャラクターが存在しない場合は、処理を行わない
        if (!_characterIcons.TryGetValue(character, out List<GameObject> icons)) return;
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
        CharacterIconGenerate();
        
        //最後に生成したキャラクターの更新
        _lastTurnCharacter = _nextTurnCharacter;
    }
    
    /// <summary>
    /// キャラクターアイコンの生成
    /// </summary>
    private void CharacterIconGenerate()
    {
        //次のターンキャラが存在しない場合、処理を行わない
        if (!_characterIcons.TryGetValue(_nextTurnCharacter, out List<GameObject> icons)) return;
        
        var icon = Instantiate(_characterIconObj, _iconParent);
        icons.Add(icon);
        
        //キャラクターアイコンを設定する
        icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(_nextTurnCharacter.GetComponent<Status>().GetCharacter().BaseData);
    }

    /// <summary>
    /// 指定したキャラクターのアイコンを生成する
    /// </summary>
    /// <param name="character">指定したキャラクター</param>
    private void GenerateSpecifiedCharacterIcon(GameObject character)
    {
        //指定したキャラクターが存在しない場合、処理を行わない
        if(!_characterIcons.TryGetValue(character, out List<GameObject> icons)) return;
        //アイコンを生成し、アイコンの設定を行う
        var icon = Instantiate(_characterIconObj, _iconParent);
        icons.Add(icon);
        var data = Characters[character].BaseData;
        icon.GetComponent<TurnCharacterIcon>().SetCharacterIcon(data);
        icon.transform.SetSiblingIndex(0);
    }

    /// <summary>
    /// 必殺技や割り込みなどによる場合でのアイコン生成に伴うアイコン削除
    /// </summary>
    /// <param name="character">必殺技や割り込みを行ったキャラクター</param>
    private void RemoveSpecifiedCharacterIcon(GameObject character)
    {
        //キャラクターが存在しない場合は、処理を行わない
        if (!_characterIcons.TryGetValue(character, out List<GameObject> icons)) return;
        if (icons.Count == 0) return;
        //UI上の一番上にあるものを削除する
        var icon = _iconParent.GetChild(0).gameObject;
        var index = 0;
        for (int i = 0; i < icons.Count; i++)
        {
            if (icons[i] == icon) index = i;
        }
        icons.RemoveAt(index);
        Destroy(icon);
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
    private void RestoreBeforeInterruptSet()
    {
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
            iconQueues[chara] = new Queue<GameObject>(_characterIcons[chara]);
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
        else
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
                    //割り込み前のキャラが麻痺状態でない場合、そのまま割り込み前のキャラが現在ターンキャラになる
                    var character = _beforeInterruptionChara;
                    var status = Characters[character].StatusEffectSystem;
                    if (!status.IsParalysisStatus())
                    {
                        CurrentTurnCharacter = character;
                        break;
                    }
                    
                    //ターンアイコンもターンスキップに連動させる
                    CharacterIconDestroy(character);
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
                    CharacterIconDestroy(cChara);
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
                CharacterIconDestroy(cChara);
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
        // 死亡しているキャラクターが存在している場合、処理を行う
        if (Characters.TryGetValue(character, out var info))
        {
            info.StateMachine.ChangeCharacterState(CharacterStateType.Dead);
            // 死亡したキャラクターのアイコンを取得し、全て削除する
            if(!_characterIcons.TryGetValue(character, out List<GameObject> icons)) return;
            foreach (var icon in icons)
            {
                Debug.Log("死亡" + info.CharacterObject.name);
                Destroy(icon);
            }
            
            _speedCharacterTurnQueue.Clear();
            List<GameObject> characters = new List<GameObject>();
            // 死亡していないキャラクターのみ追加
            foreach (var chara in _fieldCharacter)
            {
                if(!Characters.ContainsKey(chara)) continue;
                if (Characters[chara].StateMachine.CharacterState != CharacterStateType.Dead)
                {
                    characters.Add(chara);
                    Debug.Log(chara.name);
                }
            }
            _speedCharacterTurnQueue = new Queue<GameObject>(characters);
            CurrentTurnCharacter = _speedCharacterTurnQueue.Peek();
            
            // 状態異常の解除を行ってから、死亡したキャラ情報の削除を行う
            Characters[character].StatusEffectSystem.StatusAilmentsClear();
            Characters.Remove(character);
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

    /// <summary>
    /// ターンアイコンの表示、非表示
    /// </summary>
    /// <param name="flag">true：表示　false：非表示</param>
    private void TurnIconDisplay(bool flag)
    {
        //全てのターンアイコンの表示切り替えを行う
        foreach (var iconsValue in _characterIcons.Values)
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
