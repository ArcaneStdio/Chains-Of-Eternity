using System;
using UnityEngine;

[Serializable]
public class BuffData
{
    public enum BuffType { Health, Mana, Energy, SpeedBoost, Shield, AttackBoost }

    [Header("Buff Settings")]
    public BuffType buffType;
    public float duration = 5f;
    public float magnitude = 1.5f; // could be % increase or absolute
}
