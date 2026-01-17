using UnityEngine;

public class PlayerEncounterCheck : MonoBehaviour
{
    [Header("参照")]
    [Header("プレイヤー")]
    [SerializeField] private Player player;

    private bool isEncounter; //エンカウントした

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && !isEncounter)
        {
            isEncounter = true;

            //エンカウントして、プレイヤーと敵の中間地点を取得する
            var encounterPos = (transform.position + other.gameObject.transform.position) / 2;

            //フィールドに設置するキャラクターを設置する
            EncounterManager.instance.SetFieldCharacter(player.gameObject, player.CurrentBuddyMonster, other.gameObject, encounterPos);
        }
    }
}
