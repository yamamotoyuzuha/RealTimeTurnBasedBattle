using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    public static EncounterManager instance;

    [Header("参照")]
    [Header("BattleManager")]
    [SerializeField] private BattleManager battleManager;

    [Header("エンカウントUI")]
    [SerializeField] private GameObject encounterUIObj;

    [Header("中間地点から離す距離")]
    [SerializeField] private float midpointDistance;

    [Header("エンカウントし、フィールドの設定が終了")]
    [SerializeField] private bool isFieldSettingsComplete;
    public bool IsFieldSettingsComplete => isFieldSettingsComplete;

    //フィールドのキャラクター
    private GameObject player;
    private GameObject buddyMon;
    private GameObject enemy;
    private Vector3 encounterPos; //エンカウントした位置

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// フィールドに設置するキャラクターの取得
    /// </summary>
    /// <param name="player">プレイヤー</param>
    /// <param name="buddyMon">バディモン</param>
    /// <param name="enemy">敵</param>
    /// <param name="encounterPos">エンカウントした位置の中間地点</param>
    public void SetFieldCharacter(GameObject player, GameObject buddyMon, GameObject enemy, Vector3 encounterPos)
    {
        this.player = player;
        this.buddyMon = buddyMon;
        this.enemy = enemy;
        this.encounterPos = encounterPos;

        Debug.Log("キャラクターを取得する");
        
        FieldSettings();
    }

    /// <summary>
    /// バトルフィールドに設置する
    /// </summary>
    private void FieldSettings()
    {
        //プレイヤーから敵の方向ベクトルを取得する
        var direction = (enemy.transform.position - player.transform.position).normalized;

        //中間地点から離れたところにプレイヤーと敵を配置する
        player.transform.position = encounterPos - direction * midpointDistance;
        enemy.transform.position = encounterPos + direction * midpointDistance;

        //プレイヤーとエネミーが正面に来るように位置を合わせる
        //斜めからエンカウントしたときに位置がプレイヤーとエネミーであってないXが
        //それを修正する

        //バディモンは、プレイヤーの横に配置する
        var side = player.transform.right; //プレイヤーの右を取得する
        var buddyMonT = buddyMon.transform.position;

        //右側を取得して、プレイヤーとの正確な位置を合わせる
        buddyMonT = side * midpointDistance;
        buddyMonT.z = player.transform.position.z;
        buddyMonT.y = 1f;
        buddyMon.transform.position = buddyMonT;

        isFieldSettingsComplete = true;

        battleManager.ActionOrderCalculation(player, buddyMon, enemy);
    }
}
