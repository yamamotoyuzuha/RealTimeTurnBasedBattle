using UnityEngine;

public class CharacterState : MonoBehaviour
{
    public CharacterStateType characterState;
    public CharacterStateType StateType => characterState;

    /// <summary>
    /// キャラクターの状態を切り替える
    /// </summary>
    /// <param name="changeState">変更する状態</param>
    public void ChangeCharacterState(CharacterStateType changeState)
    {
        characterState = changeState;
    }
}

/// <summary>
/// キャラクターの状態
/// </summary>
public enum CharacterStateType
{
    Idle, //待機中
    ActionChoice, //行動選択中
    InAction, //行動中
    ActionCompleted, //行動終了
    Stan, //行動不能
    Dead, //死亡
    
    //Enemyが攻撃中に行える行動
    BeforeAttack
}
