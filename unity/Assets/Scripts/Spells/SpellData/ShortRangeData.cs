using System;
using UnityEngine;

[Serializable]
public class ShortRangeData
{
    [Header("Short Range Settings")]
    public float reachDistance = 2f;
    public float arcAngle = 90f; // cone shape
    public float damage = 20f;
    public float knockbackForce = 5f;
}
