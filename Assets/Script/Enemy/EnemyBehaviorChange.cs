using UnityEngine;

/// <summary>
/// 行動変化を行う
/// </summary>
public class EnemyBehaviorChange
{
    
}

public enum EnemyBehaviorChangeState
{
    [InspectorName("通常")]
    Normal,
    [InspectorName("怒り")]
    Anger
}
