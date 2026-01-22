using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MagicPanel : MonoBehaviour
{
    [Header("すべてのマスデータ")]
    [SerializeField] private AllMagicMassDatas _allMagicMassData;
    [Header("縦のサイズ")]
    [SerializeField] private int verticalSize;
    [Header("横のサイズ")]
    [SerializeField] private int horizontalSize;
    [Header("キャンバス")]
    [SerializeField] private Transform canvasTransform;
    [Header("マスの親オブジェクト")]
    [SerializeField] private Transform massParent;
    [Header("マスのオブジェクト")]
    [SerializeField] private GameObject massObj;
    [Header("現在のマスの表示")]
    [SerializeField] private GameObject currentMass;
    private RectTransform currentMassRect;

    public bool IsPanelOpen => isPanelOpen;
    private bool isPanelOpen; //パネルを開いているか

    private GameObject[,] panel;
    private Image[,] panelImage;
    private RectTransform[,] rectTransforms;
    private MassStatus[,] panelMassStatus; //生成したマスのMassStatusを格納
    private List<GameObject> passedConnectMass; //通ったマスの保持

    private int currentX; //現在のマスのXの保持
    private int currentY; //現在のマスのYの保持
    private int effectMassCount; //通った効果マスの数
    private int beforeMassX; //一個前のマスのXの保持
    private int beforeMassY; //一個前のマスのYの保持
    private int lastX; //ゴールマスの保持
    private int lastY;
    private Vector2Int startMass; //スタートマスの位置
    private Vector2Int goalMass;

    private bool isPanelClearCheck; //生成したパネルがゴール出来るか
    private bool isMagicPanelClear; //魔法パネルをクリアしたか　追加　いるのかわからない
    public bool IsMagicPanelClear => isMagicPanelClear;

    /// <summary>
    /// このキャラクターのステータスを保持
    /// </summary>
    private Status status;
    
    
    //TODO：再生成は完了
    //TODO：途中で縦横の幅を変更すると、配列がぐちゃぐちゃになるから初期化して配列の大きさを変更する

    // Start is called before the first frame update
    void Start()
    {
        //パネルの初期化
        panel = new GameObject[verticalSize, horizontalSize];
        panelImage = new Image[verticalSize, horizontalSize];
        rectTransforms = new RectTransform[verticalSize, horizontalSize];
        panelMassStatus = new MassStatus[verticalSize, horizontalSize];
        currentX = 0;
        currentY = 0;

        //グリッドレイアウトグループの縦の制約を設定する
        massParent.GetComponent<GridLayoutGroup>().constraintCount = verticalSize;

        //現在のマスのRectTransformを取得する
        currentMassRect = currentMass.GetComponent<RectTransform>();
        
        //ステータスを取得
        status = GetComponent<Status>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPanelClearCheck)
        {
            if (!BFSCheck(startMass, goalMass))
            {
                Debug.Log("到達不可");
                ResetEffectMass();
                RandomMassEffect();
            }
            else
            {
                Debug.Log("到達可能");
                isPanelClearCheck = false;
            }
        }
    }

    /// <summary>
    /// パネルサイズの設定
    /// </summary>
    /// <param name="verticalSize">縦</param>
    /// <param name="horizontalSize">横</param>
    public void PanelSettings(int verticalSize, int horizontalSize)
    {
        this.verticalSize = verticalSize;
        this.horizontalSize = horizontalSize;
    }

    /// <summary>
    /// マジックパネルの表示切り替え
    /// </summary>
    public void MagicPanelToggle()
    {
        if (!isPanelOpen) //表示
        {
            isPanelOpen = true;

            currentMass.gameObject.SetActive(true);
            GenerationPanelMass();
        }
        else //非表示
        {
            isPanelOpen = false;

            currentMass.gameObject.SetActive(false);
            DestroyMass();
        }
    }

    /// <summary>
    /// 生成されたマスを削除する
    /// </summary>
    private void DestroyMass()
    {
        //生成されているマスを消す
        foreach (Transform massChild in massParent)
        {
            Destroy(massChild.gameObject);
        }
    }

    /// <summary>
    /// パネルにマスを生成する
    /// </summary>
    private void GenerationPanelMass()
    {
        //通ったマスを保持するリストを初期化
        passedConnectMass = new List<GameObject>();

        currentX = 0;
        currentY = 0;
        effectMassCount = 0;

        //マスの生成
        for (int i = 0; i < verticalSize; i++)
        {
            for (int j = 0; j < horizontalSize; j++)
            {
                //マスの生成し、パネルに追加していく
                var obj = Instantiate(massObj, Vector3.zero, canvasTransform.rotation, massParent);
                obj.transform.localPosition = Vector3.zero;
                panel[i, j] = obj;
                
                //生成したマスのMassStatusを取得して、データを代入
                var data = _allMagicMassData.NormalMassData;
                panelMassStatus[i, j] = panel[i, j].GetComponent<MassStatus>();
                panelMassStatus[i, j].SetMassData(data);
                panelMassStatus[i, j].SetMassColor(data.Color);
            }
        }

        CornerErase();
        StartMassSet();
        GoalMassSetUp();
        NoEntryMassSetUp();
        RandomMassEffect();

        startMass = new Vector2Int(0, 2);
        goalMass = new Vector2Int(lastY, lastX);
        isPanelClearCheck = true;
    }

    /// <summary>
    /// 生成したマスの角を削除して、丸くする
    /// </summary>
    private void CornerErase()
    {
        //左上
        panelMassStatus[0, 0].MassImage.enabled = false;
        panelMassStatus[0, 1].MassImage.enabled = false;
        panelMassStatus[1, 0].MassImage.enabled = false;
        //左下
        panelMassStatus[verticalSize - 1, 0].MassImage.enabled = false;
        panelMassStatus[verticalSize - 1, 1].MassImage.enabled = false;
        panelMassStatus[verticalSize - 2, 0].MassImage.enabled = false;
        //右上
        panelMassStatus[0, horizontalSize - 1].MassImage.enabled = false;
        panelMassStatus[0, horizontalSize - 2].MassImage.enabled = false;
        panelMassStatus[1, horizontalSize - 1].MassImage.enabled = false;
        //右下
        panelMassStatus[verticalSize - 1, horizontalSize - 1].MassImage.enabled = false;
        panelMassStatus[verticalSize - 1, horizontalSize - 2].MassImage.enabled = false;
        panelMassStatus[verticalSize - 2, horizontalSize - 1].MassImage.enabled = false;

        //非表示にしたマスをNoneにする
        for(int i = 0; i < verticalSize; i++)
        {
            for(int j = 0; j < horizontalSize; j++)
            {
                //非表示かどうか判定し、マスデータを設定する
                if (!panelMassStatus[i, j].MassImage.enabled)
                {
                    var data = _allMagicMassData.NoneMassData;
                    panelMassStatus[i, j].SetMassData(data);
                }
            }
        }
    }

    /// <summary>
    /// 通行禁止マスを配置する
    /// </summary>
    private void NoEntryMassSetUp() //通行禁止マスをゴールマスの縦横斜めのどこかに配置する　追加
    {
        //ゴールマスの位置を保持用
        var goalMassVertical = 0;
        var goalMassHorizontal = 0;
        //ゴールマスの位置を取得する
        for (int i = 0; i < verticalSize; i++)
        {
            for (int j = 0; j < horizontalSize; j++)
            {
                //ゴールマスを見つけたら、位置を保持しておく
                var massData = panelMassStatus[i, j].MagicMassData.MassType;
                if (massData == MassType.Goal)
                {
                    goalMassVertical = i;
                    goalMassHorizontal = j;
                    break;
                }
            } 
        }
        
        //ゴールマスの位置から縦横斜めの位置を取得する
        MassStatus[] noEntry =
        {
            panelMassStatus[goalMassVertical - 1, goalMassHorizontal], //左
            panelMassStatus[goalMassVertical + 1, goalMassHorizontal], //右
            panelMassStatus[goalMassVertical, goalMassHorizontal - 1], //上
            panelMassStatus[goalMassVertical, goalMassHorizontal + 1], //下
            panelMassStatus[goalMassVertical - 1, goalMassHorizontal - 1], //左上
            panelMassStatus[goalMassVertical + 1, goalMassHorizontal - 1], //右上
            panelMassStatus[goalMassVertical - 1, goalMassHorizontal + 1], //左下
            panelMassStatus[goalMassVertical + 1, goalMassHorizontal + 1], //右下
        };

        //位置をランダムで取得する
        var random = Random.Range(0, noEntry.Length);
        //通行禁止マスを設定する
        var noEntryData = _allMagicMassData.NoEntryMassData;
        noEntry[random].SetMassData(noEntryData);
        //色を変更する
        noEntry[random].SetMassColor(noEntryData.Color);
    }

    /// <summary>
    /// 最初のマスを設定する
    /// </summary>
    private void StartMassSet()
    {
        //anchoredPositionを取得するために、レイアウトを即座に更新する
        LayoutRebuilder.ForceRebuildLayoutImmediate(massParent.GetComponent<RectTransform>());

        //一度GridLayoutGroupをオフにしてanchoredPositionを取得する
        var grid = massParent.GetComponent<GridLayoutGroup>();
        grid.enabled = false;
        for (int i = 0; i < verticalSize; i++)
        {
            for (int j = 0; j < horizontalSize; j++)
            {
                rectTransforms[i, j] = panel[i, j].GetComponent<RectTransform>();
            }
        }
        grid.enabled = true;

        //最初のマスに、現在選択されているマスを移動させる
        currentMassRect.anchoredPosition = rectTransforms[0, 2].anchoredPosition;
        //データを設定する
        var data = _allMagicMassData.StartMassData;
        panelMassStatus[0, 2].SetMassData(data);
        panelMassStatus[0, 2].SetMassColor(data.Color);

        //スタートマスに位置を合わせる
        currentY = 0;
        currentX = 2;
    }
    
    /// <summary>
    /// ゴールマスを配置する
    /// </summary>
    private void GoalMassSetUp() //ゴールマスをパネルの中心に配置する　追加
    {
        //縦横が偶数か奇数かで判定する
        if (verticalSize % 2 == 0)
        {
            //偶数の場合は中央値を求める
            var medianVertical = verticalSize / 2;
            //求めた中央値から、横を求める
            var medianVSub = medianVertical - 1;
            
            //中央のマスのリストを作成する
            MassStatus[] medians = 
            {
                panelMassStatus[medianVSub, medianVSub],
                panelMassStatus[medianVSub, medianVertical],
                panelMassStatus[medianVertical, medianVSub],
                panelMassStatus[medianVertical, medianVertical]
            };
            
            //作成したリストからランダムに選ぶ
            var random = Random.Range(0, medians.Length);
            
            //選ばれたリストの場所をゴールマスに設定する
            var goalData = _allMagicMassData.GoalMassData;
            medians[random].SetMassData(goalData);
            //色を変更する
            medians[random].SetMassColor(goalData.Color);
            
            //ゴールマスの位置を特定する
            for (int i = 0; i < verticalSize; i++)
            {
                for (int j = 0; j < horizontalSize; j++)
                {
                    //ゴールマスのデータとマスのデータが同じだったら位置を保持する
                    var massData = panelMassStatus[i, j].MagicMassData.MassType;
                    if (goalData.MassType == massData)
                    {
                        lastY = i;
                        lastX = j;
                        break;
                    }
                }
            }
        }
        else
        {
            //中央にゴールマスを設置する
            var medianVertical = verticalSize / 2;
            var goalData = _allMagicMassData.GoalMassData;
            panelMassStatus[medianVertical, medianVertical].SetMassData(goalData);
            panelMassStatus[medianVertical, medianVertical].SetMassColor(goalData.Color);
            
            //ゴールマスの位置を設定
            lastX = medianVertical;
            lastY = medianVertical;
        }
    }

    /// <summary>
    /// 生成したマスにランダムに効果を付与する
    /// </summary>
    private void RandomMassEffect()
    {
        //効果を付与するマスの個数分回す
        for (int i = 0; i < 3; i++)
        {
            //ランダムな数字を取得する
            var randomX = Random.Range(0, horizontalSize);
            var randomY = Random.Range(0, verticalSize);

            //同じマス、スタートマス、ゴールマス、非表示マスには効果を付与しない
            var massData = panelMassStatus[randomY, randomX].MagicMassData.MassType;
            if(massData == MassType.Effect || massData == MassType.Start || massData == MassType.Goal || massData == MassType.None || massData == MassType.NoEntry)
            {
                //同じマスが選ばれた場合、デクリメントしてなかったことにする
                i--;
                continue;
            }
            
            //マスにデータを設定する
            var dataE = _allMagicMassData.EffectMassData;
            panelMassStatus[randomY, randomX].SetMassData(dataE);
            //効果を付与する場所の色を変更する
            panelMassStatus[randomY, randomX].SetMassColor(dataE.Color);
        }
    }

    /// <summary>
    /// 効果マスをNormalに置き換える
    /// </summary>
    private void ResetEffectMass()
    {
        for (int i = 0; i < verticalSize; i++)
        {
            for (int j = 0; j < horizontalSize; j++)
            {
                //効果マスだったら、Normalに変更する
                var data = panelMassStatus[i, j].MagicMassData.MassType;
                var normalData = _allMagicMassData.NormalMassData;
                if (data == MassType.Effect)
                {
                    panelMassStatus[i, j].SetMassData(normalData);
                    panelMassStatus[i, j].SetMassColor(normalData.Color);
                }
            }
        }
    }
    
    /// <summary>
    /// 現在いるマスの移動
    /// </summary>
    /// <param name="massMove">プレイヤーからの入力</param>
    public void PanelMassMovement(Vector2 massMove)
    {
        //一個前のマスの保持
        beforeMassX = currentX;
        beforeMassY = currentY;

        //入力の値を判定する
        if(massMove.x > 0) //右
        {
            if(currentX < horizontalSize - 1)
            {
                currentX++;
                currentMassRect.anchoredPosition = rectTransforms[currentY, currentX].anchoredPosition;
                MassColorChange();
            }
        }
        else if(massMove.x < 0) //左
        {
            if(currentX > 0)
            {
                currentX--;
                currentMassRect.anchoredPosition = rectTransforms[currentY, currentX].anchoredPosition;
                MassColorChange();
            }
        }

        if(massMove.y > 0) //上
        {
            if(currentY > 0)
            {
                currentY--;
                currentMassRect.anchoredPosition = rectTransforms[currentY, currentX].anchoredPosition;
                MassColorChange();
            }
        }
        else if (massMove.y < 0) //下
        {
            if(currentY < horizontalSize - 1)
            {
                currentY++;
                currentMassRect.anchoredPosition = rectTransforms[currentY, currentX].anchoredPosition;
                MassColorChange();
            }
        }
    }

    /// <summary>
    /// 通ったマスの色を変更する
    /// </summary>
    private void MassColorChange()
    {
        //マスのデータを取得する
        var currentMassData = panelMassStatus[currentY, currentX].MagicMassData;
        var beforeMassData = panelMassStatus[beforeMassY, beforeMassX].MagicMassData;
        var effectMassData = _allMagicMassData.EffectMassData; //各マスデータを取得する
        var normalMassData = _allMagicMassData.NormalMassData;

        //非表示マス、通行禁止マスに移動しようとしている場合
        if(currentMassData.MassType == MassType.None || currentMassData.MassType == MassType.NoEntry)
        {
            //一個前の移動先に戻して、移動を無かったことにする
            MovementNotAllowed();
            Debug.Log("Noneマス、通行禁止マスを通ろうとした");
            return;
        }

        //通ったマスがゴールマスだったら
        if(currentMassData.MassType == MassType.Goal)
        {
            ConnectMass();
            return;
        }

        //スタートマスに行こうとしたとき
        if (currentMassData.MassType == MassType.Start)
        {
            if (IsOrderCheck()) return;

            //スタートマスに戻る前のマスが効果マスの場合
            if (beforeMassData.MassType == MassType.Effect)
            {
                passedConnectMass.Remove(panel[beforeMassY, beforeMassX]);
                panelMassStatus[beforeMassY, beforeMassX].SetMassColor(effectMassData.Color);
            }
            else //普通のマスの場合
            {
                passedConnectMass.Remove(panel[beforeMassY, beforeMassX]);
                panelMassStatus[beforeMassY, beforeMassX].SetMassColor(normalMassData.Color);
            }

            return;
        }

        //効果マスを通ったとき
        if (currentMassData.MassType == MassType.Effect)
        {
            //通ったマスを追加していく、重複は無しで
            if (!passedConnectMass.Contains(panel[currentY, currentX]))
            {
                passedConnectMass.Add(panel[currentY, currentX]);
            }
            else //効果マスに戻ったら、進んでいたマスの色を元に戻す
            {
                if (IsOrderCheck()) return;

                //前回のマスが、効果マスに入っているか判定する
                if (beforeMassData.MassType == MassType.Effect)
                {
                    //前回のマスも効果マスのため、効果マスの色に変更する
                    passedConnectMass.Remove(panel[beforeMassY, beforeMassX]);
                    panelMassStatus[beforeMassY, beforeMassX].SetMassColor(effectMassData.Color);
                    
                }
                else //入っていない場合
                {
                    passedConnectMass.Remove(panel[beforeMassY, beforeMassX]);
                    panelMassStatus[beforeMassY, beforeMassX].SetMassColor(normalMassData.Color);
                }
            }
            return;
        }
        else if (beforeMassData.MassType == MassType.Effect) //前回のマスが効果マスの場合
        {
            //すでに通ったマスの場合
            if (passedConnectMass.Contains(panel[currentY, currentX]))
            {
                //順番どうりかチェックをする
                if (IsOrderCheck()) return;

                //前に通ったマスが効果マスだったため、効果マスの色に変更する
                passedConnectMass.Remove(panel[beforeMassY, beforeMassX]);
                panelMassStatus[beforeMassY, beforeMassX].SetMassColor(effectMassData.Color);
                return;
            }
        }

        //通ったマスを追加していく
        if (!passedConnectMass.Contains(panel[currentY, currentX]))
        {
            passedConnectMass.Add(panel[currentY, currentX]);
            panelMassStatus[currentY, currentX].SetMassColor(Color.white);
        }
        else //通ったマスを戻るときは、通ったマスを削除して色を元に戻す
        {
            if (IsOrderCheck()) return;

            //一つ前のマスを通ったときは、色を変更する
            passedConnectMass.Remove(panel[beforeMassY, beforeMassX]);
            panelMassStatus[beforeMassY, beforeMassX].SetMassColor(normalMassData.Color);
        }
    }

    /// <summary>
    /// 移動先がNoneマス、NoEntryマスだったら、一個前の移動先に戻す
    /// </summary>
    private void MovementNotAllowed()
    {
        //一個前のマスの位置に移動させる
        currentX = beforeMassX;
        currentY = beforeMassY;
        currentMassRect.anchoredPosition = rectTransforms[currentY, currentX].anchoredPosition;
    }

    /// <summary>
    /// 順番どうりに戻ろうとしているか確認する
    /// </summary>
    /// <returns>trueなら、一個前のマスに戻す</returns>
    private bool IsOrderCheck()
    {
        GameObject lastPassed = null;
        if(passedConnectMass.Count >= 2)
        {
            //一つ前のマスを取得する
            lastPassed = passedConnectMass[passedConnectMass.Count - 2];
        }

        if (lastPassed != null && panel[currentY, currentX] != lastPassed) //現在のマスが、一つ前のマスじゃないとき
        {
            Debug.Log("逆順でしか戻れない");

            //移動出来ないように、一個前のマスの位置に移動させる
            currentX = beforeMassX;
            currentY = beforeMassY;
            currentMassRect.anchoredPosition = rectTransforms[currentY, currentX].anchoredPosition;
            return true;
        }

        return false;
    }

    /// <summary>
    /// ゴール（仮）マスまで行ったときに、通ったマスの判定をする
    /// </summary>
    private void ConnectMass()
    {
        Debug.Log("ゴールまでつなげられた");
        
        //魔法パネルをクリア判定にする
        isMagicPanelClear = true;

        //効果マスの保持　追加
        List<MagicMassData> effectMass =  new List<MagicMassData>();
        
        //通ったマスの判定をしていく
        foreach(var connect in passedConnectMass)
        {
            //通ったマスの中に効果マスがあるかどうか判定する
            var connectData = connect.GetComponent<MassStatus>().MagicMassData;
            if(connectData.MassType == MassType.Effect)
            {
                effectMassCount++;
                Debug.Log("通った効果マスの数" + effectMassCount);
                
                //効果マスを追加する　追加
                effectMass.Add(connectData);
            }
        }

        //内容を判定
        EffectSquareContentAssessment(effectMass);

        //マジックパネルを非表示にする
        MagicPanelToggle();
    }

    /// <summary>
    /// 効果マスの内容判定
    /// <param name="effectMass">効果マス</param>>
    /// </summary>
    private void EffectSquareContentAssessment(List<MagicMassData> effectMass)
    {
        //キャラクターのステータスを取得
        var playerStatus = status.GetCharacterStatus();
        
        //効果マスの内容
        for (int i = 0; i < effectMass.Count; i++)
        {
            //効果マスの効果を取得
            var effect = effectMass[i].EffectMassData;
            switch (effect.Type) //効果の内容判定
            {
                case EffectMassType.Attribute:
                    break;
                case EffectMassType.AttackPowerDown:
                    break;
                case EffectMassType.DefensePowerDown:
                    break;
                case EffectMassType.AttackPowerUp:
                    playerStatus.AttackPowerUp(effect.BuffPercent);
                    break;
                case EffectMassType.DefensePowerUp:
                    playerStatus.DefensePowerUp(effect.BuffPercent);
                    break;
                //case EffectMassType.CriticalRate:
                    //break;
                //case EffectMassType.CriticalDamage:
                    //break;
            }
        }
    }

    /// <summary>
    /// ステータス以外に効果を付与する　追加
    /// </summary>
    private void EachEffectExceptionAction()
    {
        
    }
    
    /// <summary>
    /// 現在の状態から、移動した新しい情報を作成して返す
    /// </summary>
    /// <param name="massInfo">今の状態</param>
    /// <param name="next">移動先</param>
    /// <returns>移動した新しい状態を返す</returns>
    private MassInfo MoveState(MassInfo massInfo, Vector2Int next)
    {
        //リストのコピーを作成する
        List<Vector2Int> nextStack = new List<Vector2Int>(massInfo.stack);
        List<Vector2Int> nextLog = new List<Vector2Int>(massInfo.pathLog);
        
        nextLog.Add(next);

        //移動するマスが直前のマスの場合
        if (nextStack.Count >= 2 &&  next == nextStack[nextStack.Count - 2])
        {
            //現在いるマスを消す
            nextStack.RemoveAt(nextStack.Count - 1);
        }
        else
        {
            //新しくマスを追加する
            nextStack.Add(next);
        }
        
        //移動した新しい状態を返す
        return new MassInfo(next, nextStack, nextLog);
    }
    
    /// <summary>
    /// 現在の状態から、次に移動出来る全てのマスを返す
    /// </summary>
    /// <param name="massInfo">現在の状態</param>
    /// <returns>移動出来るマスのリストを返す</returns>
    private List<Vector2Int> GetMoveMass(MassInfo massInfo)
    {
        //移動出来るマスを格納する
        List<Vector2Int> moveMass = new List<Vector2Int>();

        //移動出来る４方向をチェックする
        foreach (var dir in massInfo.Dirs)
        {
            Vector2Int next = massInfo.pos + dir; //次に移動する方向

            if (!IsMagicPanelSize(next)) continue; //パネルの範囲外ならスキップ
            if (!IsMassPassage(next)) continue; //移動するマスが通ることが出来ないマスの場合は、スキップ

            if (!massInfo.stack.Contains(next)) //まだ、通ったことのないマスの場合
            {
                moveMass.Add(next);
            }
            else
            {
                if (massInfo.stack.Count >= 2)
                {
                    //直前のマス
                    Vector2Int before = massInfo.stack[massInfo.stack.Count - 2];
                    if (next == before) //移動しようとしているマスが直前のマスの場合
                    {
                        //直前のマスは、移動可能なため追加する
                        moveMass.Add(next);
                    }
                }
            }
        }
        return moveMass;
    }
    
    /// <summary>
    /// 効果マスを取得する
    /// </summary>
    /// <returns>効果マスのリストを返す</returns>
    private List<Vector2Int> AllGetEffectMass()
    {
        //効果マスを追加していく
        List<Vector2Int> effectMass = new List<Vector2Int>();
        for (int i = 0; i < verticalSize; i++)
        {
            for (int j = 0; j < horizontalSize; j++)
            {
                var mass = panelMassStatus[i, j].MagicMassData.MassType;
                if (mass == MassType.Effect)
                {
                    effectMass.Add(new Vector2Int(i, j));
                }
            }
        }
        return effectMass;
    }

    /// <summary>
    /// 今の位置と通ってきた道を文字列にして、状態の名前にする
    /// </summary>
    /// <param name="massInfo">現在の状態</param>
    /// <returns></returns>
    private string StateKey(MassInfo massInfo)
    {
        return massInfo.pos.x + "," + massInfo.pos.y + ":" +
               string.Join("-", massInfo.stack.Select(v => v.x + "_" + v.y));
    }

    /// <summary>
    /// 魔法パネルの範囲外かの判定
    /// </summary>
    /// <param name="pos">移動先</param>
    /// <returns>trueなら魔法パネルの範囲内　falseなら魔法パネルの範囲外</returns>
    private bool IsMagicPanelSize(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < verticalSize &&
               pos.y >= 0 && pos.y < horizontalSize;
    }

    /// <summary>
    /// 移動先が通ることの出来るマスかどうか
    /// </summary>
    /// <param name="pos">移動先</param>
    /// <returns>trueなら移動できるマス　falseなら移動できないマス</returns>
    private bool IsMassPassage(Vector2Int pos)
    {
        var mass = panelMassStatus[pos.x, pos.y].MagicMassData.MassType;
        return mass != MassType.None && mass != MassType.NoEntry;
    }
    
    /// <summary>
    /// 生成した魔法パネルが、効果マスを全ての通りゴールマスに行けるかどうか判定する
    /// </summary>
    /// <param name="start">スタートマス</param>
    /// <param name="goal">ゴールマス</param>
    /// <returns>trueなら可能　falseなら不可能</returns>
    private bool BFSCheck(Vector2Int start, Vector2Int goal)
    {
        //効果マスを取得する
        var allEffectMass = AllGetEffectMass();
        
        //最初の状態にする
        MassInfo startSet = new MassInfo(start, new List<Vector2Int>(),  new List<Vector2Int>());
        startSet.stack.Add(start);
        startSet.pathLog.Add(start);
        
        //これから調べるマス
        Queue<MassInfo> queue = new Queue<MassInfo>();
        queue.Enqueue(startSet);
        //調べ済みのマス
        HashSet<string> visited = new HashSet<string>();
        visited.Add(StateKey(startSet));
        
        while (queue.Count > 0)
        {
            //先頭から調べるものを取り出す
            MassInfo massInfo = queue.Dequeue();
            //全ての効果マスを踏んだかどうか
            bool isAllEffectMassPassed = allEffectMass.All(e => massInfo.stack.Contains(e));

            //全ての効果マスを踏んだ状態で、ゴールマスに行くことが出来たか
            if (isAllEffectMassPassed && massInfo.pos == goal)
            {
                //ColorPath(massInfo.pathLog, Color.green);
                return true;
            }

            foreach (var next in GetMoveMass(massInfo)) //次の移動候補
            {
                //もし、次の移動候補がゴールマスの場合　追加
                if (next == goal)
                {
                    //まだ、効果マスを踏み終わっていない段階でゴールマスを踏んだらスキップ
                    if(!isAllEffectMassPassed) continue;
                }
                
                MassInfo move = MoveState(massInfo, next); //次の状態を作成する
                string key = StateKey(move); //次の状態を文字列にして、状態の名前にする
                if (visited.Contains(key)) continue; //重複したら、スキップ
                
                //新しく状態を追加する
                visited.Add(key);
                queue.Enqueue(move);
            }
        }
        
        return false;
    }
    void ColorPath(List<Vector2Int> path, Color color)
    {
        foreach (var p in path)
        {
            panelImage[p.x, p.y].color = color;
        }
    }
}

/// <summary>
/// BFSによる探索中の１つの状態を表す
/// </summary>
public class MassInfo
{
    public Vector2Int pos; //現在のマスの位置
    public List<Vector2Int> stack; //通ってきた順番
    public List<Vector2Int> pathLog; //デバック用

    //移動出来る方向
    private Vector2Int[] dirs = new Vector2Int[]
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };
    public Vector2Int[] Dirs => dirs;

    public MassInfo(Vector2Int pos, List<Vector2Int> stack, List<Vector2Int> pathLog)
    {
        this.pos = pos;
        this.stack = stack;
        this.pathLog = pathLog;
    }
}

