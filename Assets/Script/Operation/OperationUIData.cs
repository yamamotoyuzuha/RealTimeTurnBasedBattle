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

    #region プロパティ
    public Sprite[] CommandInputSprites => _commandInputSprites;
    public Sprite SkillChangeSprite => _skillChangeSprite;
    public Sprite BackSprite => _backSprite;
    public Sprite ExecuteSprite => _executeSprite;

    public Sprite[] MoveSprites => _moveSprites;
    
    public Sprite ParrySprite => _parrySprite;
    public Sprite JustGuardSprite => _justGuardSprite;
    #endregion
}
