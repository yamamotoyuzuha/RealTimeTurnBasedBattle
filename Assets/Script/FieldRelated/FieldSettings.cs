using UnityEngine;

/// <summary>
/// キャラクターの位置を管理
/// </summary>
public class FieldSettings : MonoBehaviour
{
    [Header("プレイヤーキャラクターの位置")]
    [SerializeField] private Transform[] _playerCharaPos;
    [Header("Enemyの位置")] 
    [SerializeField] private Transform _enemyCharaPos;
    
    public Transform[] PlayerCharaPos => _playerCharaPos;
    public Transform EnemyCharaPos => _enemyCharaPos;
}
