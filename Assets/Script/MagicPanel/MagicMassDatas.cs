using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMassDatas : MonoBehaviour
{
    [Header("魔法パネルのスタートマス")]
    [SerializeField] private MagicMassData startMassData;
    [Header("魔法パネルのゴールマス")]
    [SerializeField] private MagicMassData goalMassData;

    public MagicMassData StartMassData => startMassData;
    public MagicMassData GoalMassData => goalMassData;

    [Header("魔法パネルの非表示のマス")]
    [SerializeField] private MagicMassData noneMassData;
    [Header("魔法パネルの普通マス")]
    [SerializeField] private MagicMassData normalMassData;
    [Header("魔法パネルの効果マス")]
    [SerializeField] private MagicMassData effectMassData;

    [Header("魔法パネルの通行禁止マス")]
    [SerializeField] private MagicMassData noEntryMassData;

    public MagicMassData NoneMassData => noneMassData;
    public MagicMassData NormalMassData => normalMassData;
    public MagicMassData EffectMassData => effectMassData;

    public MagicMassData NoEntryMassData => noEntryMassData;

}
