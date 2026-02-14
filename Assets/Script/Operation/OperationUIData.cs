using UnityEngine;

/// <summary>
/// 操作UIのデータ
/// ・画像
/// </summary>
[CreateAssetMenu(fileName = "OperationUIData", menuName = "ScriptableObject/OperationUIData")]
public class OperationUIData : ScriptableObject
{
    #region コマンド
    [Header("コマンド入力")]
    [SerializeField] private Sprite[] _commandInputSprites;
    [Header("スキル一覧の表示切り替え")]
    [SerializeField] private Sprite _skillChangeSprite;
    [Header("戻る")]
    [SerializeField] private Sprite _backSprite;
    [Header("実行")]
    [SerializeField] private Sprite _executeSprite;
    #endregion
    
    #region 魔法パネル
    [Header("魔法パネルのマス移動")]
    [SerializeField] private Sprite[] _moveSprites;
    #endregion

    #region 防御アクション
    [Header("パリィ入力")] 
    [SerializeField] private Sprite _parrySprite;
    [Header("ジャストガード入力")] 
    [SerializeField] private Sprite _justGuardSprite;
    #endregion

    #region タイトルの操作UI関連
    [Header("パーティー編成表示")] 
    [SerializeField] private Sprite _pfSprite;
    [Header("Enemy選択表示")]
    [SerializeField] private Sprite _esSprite;
    [Header("選択")]
    [SerializeField] private Sprite _selectSprite;
    [Header("選択解除")]
    [SerializeField] private Sprite _deSelectSprite;
    [Header("左矢印")]
    [SerializeField] private Sprite _leftArrowSprite;
    [Header("右矢印")]
    [SerializeField] private Sprite _rightArrowSprite;
    #endregion

    #region プロパティ
    public Sprite[] CommandInputSprites => _commandInputSprites;
    public Sprite SkillChangeSprite => _skillChangeSprite;
    public Sprite BackSprite => _backSprite;
    public Sprite ExecuteSprite => _executeSprite;

    public Sprite[] MoveSprites => _moveSprites;
    
    public Sprite ParrySprite => _parrySprite;
    public Sprite JustGuardSprite => _justGuardSprite;
    
    public Sprite PfSprite => _pfSprite;
    public Sprite EsSprite => _esSprite;
    public Sprite SelectSprite => _selectSprite;
    public Sprite DeSelectSprite => _deSelectSprite;
    public Sprite LeftArrowSprite => _leftArrowSprite;
    public Sprite RightArrowSprite => _rightArrowSprite;
    #endregion
}
