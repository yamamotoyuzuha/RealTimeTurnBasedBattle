using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObject/PlayerData")]
public class PlayerData : CharacterBaseData
{
    [Header("移動速度")]
    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    [Header("ジャンプ力")]
    [SerializeField] private float jumpAbility;
    public float JumpAbility => jumpAbility;
    [Header("地面に向けて出すRayの長さ")]
    [SerializeField] private float rayDistance;
    public float RayDistance => rayDistance;
}
