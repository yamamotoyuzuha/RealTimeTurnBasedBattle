using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CommandUI : MonoBehaviour
{
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
    [Header("各魔法の切り替えUI")]
    [SerializeField] private GameObject _eachMagicChangeUI;
    [Header("切り替えUI（左）")]
    [SerializeField] private EachMagicChangeUI _changeUILeft;
    [Header("切り替えUI（右）")]
    [SerializeField] private EachMagicChangeUI _changeUIRight;
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
    //キャラクター関連
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
    
    /// <summary>
    /// 各魔法の切り替えUI
    /// </summary>
    [Serializable]
    public class EachMagicChangeUI
    {
        [Header("Dot")]
        [SerializeField] private Image _dotImage;
        [Header("Arrow")]
        [SerializeField] private Image _arrowImage;

        public EachMagicChangeUI(Image dot, Image arrow)
        {
            _dotImage = dot;
            _arrowImage = arrow;
        }
        
        /// <summary>
        /// Imageの色を変更
        /// </summary>
        /// <param name="color">変更する色</param>
        public void ImageChange(Color color)
        {
            _dotImage.color = color;
            _arrowImage.color = color;
        }

        /// <summary>
        /// Imageの表示、非表示を行う
        /// </summary>
        /// <param name="isDisplay">true：表示　false：非表示</param>
        public void ImageDisplay(bool isDisplay)
        {
            _dotImage.gameObject.SetActive(isDisplay);
            _arrowImage.gameObject.SetActive(isDisplay);
        }
    }
    
    void Awake()
    {
        //親オブジェクトであるキャラクターのデータを取得する
        characterObj = transform.root.gameObject;
        characterStatus = characterObj.GetComponent<Status>();
        characterBaseData = characterStatus.GetData();
        Debug.Log(characterBaseData);
        ToggleCommandUI(false);
        EachMagicUIGenerate();
        MagicUIHidden();
        EachActionUIIconSettings();
    }

    void Start()
    {
        //ToggleCommandUI(false);
        //EachMagicUIGenerate();
        //MagicUIHidden();
        //EachActionUIIconSettings();
    }
    
    /// <summary>
    /// 各行動UIアイコンの設定
    /// ・魔法
    /// ・攻撃
    /// ・アイテム
    /// </summary>
    private void EachActionUIIconSettings()
    {
        var magic = magicUIObj.transform.GetChild(1).GetComponent<Image>();
        var attack = attackUIObj.transform.GetChild(1).GetComponent<Image>();
        var item = itemUIObj.transform.GetChild(1).GetComponent<Image>();
        var ope = OperationDataManager.Instance.OperationUIData;

        magic.sprite = ope.CommandInputSprites[0];
        attack.sprite = ope.CommandInputSprites[1];
        item.sprite = ope.CommandInputSprites[2];
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
                _eachMagicChangeUI.SetActive(true);
                IsCurrentSelectedMagic = true;
                
                CommandUIChangeHidden(false);
                EachMagicChangeUIToggle(true);
                break;
            
            case CommandState.Attack:
                CommandUIChangeHidden(false);
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
                    
                    EachMagicChangeUIToggle(false);
                }
                else //右側が表示されている状態
                {
                    notSelectedParentLeft.gameObject.SetActive(false);
                    notSelectedParentRight.gameObject.SetActive(true);
                    eachMagicParentLeft.gameObject.SetActive(true);
                    eachMagicParentRight.gameObject.SetActive(false);
                    IsCurrentSelectedMagic = true;
                    
                    EachMagicChangeUIToggle(true);
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
        _eachMagicChangeUI.SetActive(false);
    }

    /// <summary>
    /// 各魔法の切り替えUIの切り替え
    /// </summary>
    /// <param name="isDisplay">true：左　false：右</param>>
    private void EachMagicChangeUIToggle(bool isDisplay)
    {
        if (isDisplay)
        {
            _changeUILeft.ImageChange(Color.white);
            _changeUIRight.ImageChange(Color.black);
            _changeUILeft.ImageDisplay(true);
            _changeUIRight.ImageDisplay(false);
        }
        else
        {
            _changeUILeft.ImageChange(Color.black);
            _changeUIRight.ImageChange(Color.white);
            _changeUILeft.ImageDisplay(false);
            _changeUIRight.ImageDisplay(true);
        }
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

        //魔法のデータ分UIを生成する
        for (int i = 0; i < characterBaseData.MagicBaseData.Length; i++)
        {
            var index = i % 3;
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
                var mpText = eachMagic.transform.GetChild(4).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                mpText.text = magicBaseDatas[i].ConsumptionMp.ToString();
                
                //操作アイコンを設定
                var opImage = eachMagic.transform.GetChild(1).GetComponent<Image>();
                opImage.sprite = OperationDataManager.Instance.OperationUIData.CommandInputSprites[index];
                
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
                var mpText = eachMagic.transform.GetChild(4).transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                mpText.text = magicBaseDatas[i].ConsumptionMp.ToString();
                
                var opImage = eachMagic.transform.GetChild(1).GetComponent<Image>();
                opImage.sprite = OperationDataManager.Instance.OperationUIData.CommandInputSprites[index];
                
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
