using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CommandUI : MonoBehaviour
{
    /*
    [Header("CommandInputManager")]
    [SerializeField] private CommandInputManager commandInputManager;
    */
    [Header("魔法UI")]
    [SerializeField] private GameObject magicUIObj;
    [Header("各魔法UI")]
    [SerializeField] private GameObject eachMagicUIObjLeft;
    [SerializeField] private GameObject eachMagicUIObjRight;
    [Header("各魔法UIの生成場所")]
    [SerializeField] private Transform eachMagicParentLeft;
    [SerializeField] private Transform eachMagicParentRight;
    [Header("各魔法UIのPrefab")]
    [SerializeField] private GameObject eachMagicObj;
    public List<MagicBaseData> EachMagicLeft { get; private set; } = new List<MagicBaseData>();
    public List<MagicBaseData> EachMagicRight { get; private set; } = new List<MagicBaseData>();

    [Header("魔法の名前UIの生成場所")] 
    [SerializeField] private Transform notSelectedParentLeft;
    [SerializeField] private Transform notSelectedParentRight;
    [Header("魔法の名前UIのPrefab")]
    [SerializeField] private GameObject notSelectedMagicObj;
    [Header("攻撃UI")]
    [SerializeField] private GameObject attackUIObj;
    private List<GameObject> eachAttack = new List<GameObject>();
    [Header("アイテムUI")]
    [SerializeField] private GameObject itemUIObj;
    private List<GameObject> eachItem = new List<GameObject>();

    private GameObject characterObj; //キャラクター
    private Status characterStatus; //キャラクターのステータスを保持
    private CharacterBaseData characterBaseData; //キャラクターのデータを保持

    /// <summary>
    /// true：左　false：右
    /// </summary>
    public bool IsCurrentSelectedMagic { get; private set; }

    //各魔法のUIのImageColorControllerを保持する
    private ImageColorController[] leftColorController;
    private ImageColorController[] rightColorController;
    private ImageColorController[] notSelectedLeftColorController;
    private ImageColorController[] notSelectedRightColorController;

    void Awake()
    {
        //親オブジェクトであるキャラクターのデータを取得する
        characterObj = transform.root.gameObject;
        characterStatus = characterObj.GetComponent<Status>();
        characterBaseData = characterStatus.GetData();
        Debug.Log(characterBaseData);
    }

    void Start()
    {
        ToggleCommandUI(false);
        EachMagicUIGenerate();
        MagicUIHidden();
    }

    /// <summary>
    /// コマンドUIの表示を切り替える
    /// </summary>
    /// <param name="isFlag">true：表示　false：非表示</param>
    public void ToggleCommandUI(bool isFlag)
    {
        magicUIObj.SetActive(isFlag);
        attackUIObj.SetActive(isFlag);
        itemUIObj.SetActive(isFlag);
        
        /*
        //CommandInputManagerにCommandUIを渡す
        if(isFlag) commandInputManager.GetCommandUI(this);
        else
        {
            commandInputManager.GetCommandUI(null);
            MagicUIHidden();
        }
        */
        if(isFlag) CommandInputManager.Instance.GetCommandUI(this);
        else
        {
            CommandInputManager.Instance.GetCommandUI(null);
            MagicUIHidden();
        }
    }

    /// <summary>
    /// 魔法、攻撃、アイテムの各UIを表示する
    /// </summary>
    public void ShowCommandUI(CommandState commandState)
    {
        switch (commandState)
        {
            case CommandState.Magic:
                notSelectedParentLeft.gameObject.SetActive(false);
                notSelectedParentRight.gameObject.SetActive(true);
                eachMagicParentLeft.gameObject.SetActive(true);
                eachMagicParentRight.gameObject.SetActive(false);
                IsCurrentSelectedMagic = true;
                
                CommandUIChangeHidden(false);
                break;
            
            case CommandState.Attack:
                break;
            
            case CommandState.Item:
                break;
        }
    }
    
    /// <summary>
    /// 魔法、攻撃、アイテムのUIを非表示にする
    /// <param name="isFlag">true：表示　false：非表示</param>>
    /// </summary>
    public void CommandUIChangeHidden(bool isFlag)
    {
        magicUIObj.SetActive(isFlag);
        attackUIObj.SetActive(isFlag);
        itemUIObj.SetActive(isFlag);
    }

    /// <summary>
    /// 選択したコマンドのUIを表示する
    /// <param name="commandState">表示するコマンド</param>>
    /// </summary>
    public void ChangeCommandUI(CommandState commandState)
    {
        switch (commandState)
        {
            case CommandState.Magic:
                if (IsCurrentSelectedMagic) //左側が表示されている状態
                {
                    notSelectedParentLeft.gameObject.SetActive(true);
                    notSelectedParentRight.gameObject.SetActive(false);
                    eachMagicParentLeft.gameObject.SetActive(false);
                    eachMagicParentRight.gameObject.SetActive(true);
                    IsCurrentSelectedMagic = false;
                }
                else //右側が表示されている状態
                {
                    notSelectedParentLeft.gameObject.SetActive(false);
                    notSelectedParentRight.gameObject.SetActive(true);
                    eachMagicParentLeft.gameObject.SetActive(true);
                    eachMagicParentRight.gameObject.SetActive(false);
                    IsCurrentSelectedMagic = true;
                }
                break;
            
            case CommandState.Attack:
                break;
            
            case CommandState.Item:
                break;
        }
    }

    /// <summary>
    /// 魔法UIを非表示にする
    /// </summary>
    public void MagicUIHidden()
    {
        eachMagicParentLeft.gameObject.SetActive(false);
        eachMagicParentRight.gameObject.SetActive(false);
        notSelectedParentLeft.gameObject.SetActive(false);
        notSelectedParentRight.gameObject.SetActive(false);
    }

    /// <summary>
    /// 各魔法のUIの生成
    /// プレイヤーの所持しているスキルによって変わるようにする
    /// </summary>
    private void EachMagicUIGenerate()
    {
        //キャラクターのデータから魔法を全て取得する
        List<MagicBaseData> magicBaseDatas = new List<MagicBaseData>();
        foreach (var magic in characterBaseData.MagicBaseData)
        {
            magicBaseDatas.Add(magic);
        }
        
        //MP消費量の順番でソート
        magicBaseDatas = magicBaseDatas.OrderBy(i => i.ConsumptionMp).ToList();

        /*
        //その魔法の情報をUIに反映
        for (int i = 0; i < 6; i++)
        {
            if (i > 2) //右側に生成
            {
                //UIを生成
                var eachMagic = Instantiate(eachMagicObj, eachMagicParentRight);
                var notSelected = Instantiate(notSelectedMagicObj, notSelectedParentRight);

                //生成したUIからTextを取得して、魔法の情報を反映させる
                var magicName = eachMagic.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                magicName.text = magicBaseDatas[i].MagicName;
                var magicExplanation = eachMagic.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                magicExplanation.text = magicBaseDatas[i].MagicExplanation;
                
                var notSelectedName = notSelected.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                notSelectedName.text = magicBaseDatas[i].MagicName;
                
                //MPの消費コストをUIに反映
                //TODO：テキストを取得して、魔法データから情報を反映させる
                
                EachMagicRight.Add(magicBaseDatas[i]);
            }
            else //左側に生成
            {
                var eachMagic = Instantiate(eachMagicObj,  eachMagicParentLeft);
                var notSelected = Instantiate(notSelectedMagicObj, notSelectedParentLeft);
                
                //生成したUIからTextを取得して、魔法の情報を反映させる
                var magicName = eachMagic.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                magicName.text = magicBaseDatas[i].MagicName;
                var magicExplanation = eachMagic.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                magicExplanation.text = magicBaseDatas[i].MagicExplanation;
                
                var notSelectedName = notSelected.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                notSelectedName.text = magicBaseDatas[i].MagicName;
                
                //魔法をUIに基づいた順でリストに追加していく
                EachMagicLeft.Add(magicBaseDatas[i]);
            }
        }
        */
        //魔法のデータ分UIを生成する
        for (int i = 0; i < characterBaseData.MagicBaseData.Length; i++)
        {
            if (i > 2) //右側に生成
            {
                //UIを生成
                var eachMagic = Instantiate(eachMagicObj, eachMagicParentRight);
                var notSelected = Instantiate(notSelectedMagicObj, notSelectedParentRight);

                //生成したUIからTextを取得して、魔法の情報を反映させる
                var magicName = eachMagic.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                magicName.text = magicBaseDatas[i].MagicName;
                var magicExplanation = eachMagic.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                magicExplanation.text = magicBaseDatas[i].MagicExplanation;
                
                var notSelectedName = notSelected.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                notSelectedName.text = magicBaseDatas[i].MagicName;
                
                //MPの消費コストをUIに反映
                //TODO：テキストを取得して、魔法データから情報を反映させる
                
                EachMagicRight.Add(magicBaseDatas[i]);
            }
            else //左側に生成
            {
                var eachMagic = Instantiate(eachMagicObj,  eachMagicParentLeft);
                var notSelected = Instantiate(notSelectedMagicObj, notSelectedParentLeft);
                
                //生成したUIからTextを取得して、魔法の情報を反映させる
                var magicName = eachMagic.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
                magicName.text = magicBaseDatas[i].MagicName;
                var magicExplanation = eachMagic.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
                magicExplanation.text = magicBaseDatas[i].MagicExplanation;
                
                var notSelectedName = notSelected.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                notSelectedName.text = magicBaseDatas[i].MagicName;
                
                //魔法をUIに基づいた順でリストに追加していく
                EachMagicLeft.Add(magicBaseDatas[i]);
            }
        }
        
        //左側は魔法の名前、右側は魔法の詳細を非表示にする
        notSelectedParentLeft.gameObject.SetActive(false);
        eachMagicParentRight.gameObject.SetActive(false);
        IsCurrentSelectedMagic = true;
       
        //配列のサイズを生成されたUI分にする
        leftColorController = new ImageColorController[eachMagicParentLeft.childCount];
        rightColorController = new ImageColorController[eachMagicParentRight.childCount];
        notSelectedLeftColorController = new ImageColorController[notSelectedParentLeft.childCount];
        notSelectedRightColorController = new ImageColorController[notSelectedParentRight.childCount];
        //各魔法のUIのImageColorControllerを取得する
        for (int i = 0; i < eachMagicParentLeft.childCount; i++)
        {
            //i番目のPrefabを取得して、ImageColorControllerを配列に格納
            /*
            var leftChild = eachMagicParentLeft.GetChild(i); //i番目のPrefabを取得
            leftColorController[i] = leftChild.GetChild(0).GetComponent<ImageColorController>();
            var rightChild = eachMagicParentRight.GetChild(i);
            rightColorController[i] = rightChild.GetChild(0).GetComponent<ImageColorController>();
            var notSelectedLeftChild = notSelectedParentLeft.GetChild(i);
            notSelectedLeftColorController[i] = notSelectedLeftChild.GetChild(0).GetComponent<ImageColorController>();
            var notSelectedRightChild = notSelectedParentRight.GetChild(i);
            notSelectedRightColorController[i] = notSelectedRightChild.GetChild(0).GetComponent<ImageColorController>();
            */
            //i番目のPrefabを取得して、ImageColorControllerに格納
            if (eachMagicParentLeft.childCount != 0)
            {
                var leftChild = eachMagicParentLeft.GetChild(i);
                var notSelectedLeftChild = notSelectedParentLeft.GetChild(i);
                leftColorController[i] = leftChild.GetChild(0).GetComponent<ImageColorController>();
                notSelectedLeftColorController[i] = notSelectedLeftChild.GetChild(0).GetComponent<ImageColorController>();
            }

            if (eachMagicParentRight.childCount != 0)
            {
                var rightChild = eachMagicParentRight.GetChild(i);
                var notSelectedRightChild = notSelectedParentRight.GetChild(i);
                rightColorController[i] = rightChild.GetChild(0).GetComponent<ImageColorController>();
                notSelectedRightColorController[i] = notSelectedRightChild.GetChild(0).GetComponent<ImageColorController>();
            }
        }
        
        CurrentMpMagicUsableChangeColor(characterStatus);
    }

    /// <summary>
    /// ターンが切り替わり、まだ魔法UIを開いていないときのみ呼び出す
    /// 現在のMPで使用出来る魔法のみUIの色を変更する
    /// <param name="status">キャラクターのステータス</param>>
    /// </summary>
    public void CurrentMpMagicUsableChangeColor(Status status)
    {
        //現在のMPを取得する
        var mp = status.GetMp();

        //キャラクターが保持している魔法の消費MPと比較
        for (int i = 0; i < EachMagicLeft.Count; i++)
        {
            //現在のMPが消費量MPより大きい場合、UIの色を変更する
            if (mp >= EachMagicLeft[i].ConsumptionMp) //明
            {
                leftColorController[i].AvailableColor();
                notSelectedLeftColorController[i].AvailableColor();
            }
            else //暗
            {
                leftColorController[i].UnavailableColor();
                notSelectedLeftColorController[i].UnavailableColor();
            }
        }

        for(int j = 0; j < EachMagicRight.Count; j++)
        {
            if (mp >= EachMagicRight[j].ConsumptionMp)
            {
                rightColorController[j].AvailableColor();
                notSelectedRightColorController[j].AvailableColor();
            }
            else
            {
                rightColorController[j].UnavailableColor();
                notSelectedRightColorController[j].UnavailableColor();
            }
        }
    }
}
