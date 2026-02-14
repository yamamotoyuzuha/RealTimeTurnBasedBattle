using System;
using UnityEngine;

/// <summary>
/// エフェクト関連の情報
/// ・エフェクトの名前
/// ・エフェクトの生成場所
/// </summary>
[Serializable]
public class EffectData
{
    public string _effectName;
    public Transform _effectTransform;
}
