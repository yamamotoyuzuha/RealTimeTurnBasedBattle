/// <summary>
/// キャラクターのステートを管理する
/// </summary>
public class CharacterStateMachine
{
    public CharacterStateType CharacterState { get; private set; }
    
    public CharacterStateMachine(CharacterStateType characterState = CharacterStateType.Idle)
    {
        CharacterState = characterState;
    }
    
    public void ChangeCharacterState(CharacterStateType changeState)
    {
        CharacterState = changeState;
    }
}
