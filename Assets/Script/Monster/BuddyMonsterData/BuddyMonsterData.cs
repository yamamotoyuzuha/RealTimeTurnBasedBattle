using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BuddyMon", menuName = "ScriptableObject/BuddyMon")]
public class BuddyMonsterData : CharacterBaseData
{
    [Header("ライドアクションタイプ")]
    [SerializeField] private RideActionType actionType;
    [Header("移動速度")]
    [SerializeField] private float buddyMonMoveSpeed;
    [Header("ジャンプ力")]
    [SerializeField] private float jumpingAbility;
    [Header("地面に向けて出すRayの長さ")]
    [SerializeField] private float rayDistance;

    //フィールド上での咆哮が効くかの判定
    [Header("強さのランク")]
    [SerializeField] private int strongRank;
    
    
    public RideActionType BuddyMonActionType => actionType;
    public float BuddyMonMoveSpeed => buddyMonMoveSpeed;
    public float JumpingAbility => jumpingAbility;
    public float RayDistance => rayDistance;

    public int StrongRank => strongRank;
}

public enum RideActionType
{
    HighJump, //高いジャンプ
    Climb, //登る
    Flight, //飛行
}
