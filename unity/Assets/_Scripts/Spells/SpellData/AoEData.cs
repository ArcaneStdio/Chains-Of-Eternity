using System;
using UnityEngine;

/// <summary>
/// Configuration data for AoE spells in 2D Unity projects.
/// Defines shape, damage, visuals, and behavior.
/// </summary>
[CreateAssetMenu(fileName = "New AoE Spell", menuName = "Spell System/AoE Spell Data")]
[Serializable]
public class AoEData : ScriptableObject
{
    public enum AoEShape { Circle, Cone, Rectangle, Line }
    public enum DamageType { Instant, OverTime }
    public enum TargetingMode { CasterPosition, MousePosition, Direction }

    [Header("Basic Settings")]
    [Tooltip("Spell name for identification")]
    public string spellName = "New AoE Spell";

    [Tooltip("How this AoE deals damage: instant burst or damage over time.")]
    public DamageType damageType = DamageType.Instant;

    [Tooltip("Where the AoE originates from")]
    public TargetingMode targetingMode = TargetingMode.CasterPosition;

    [Tooltip("Base damage (instant or per tick depending on type).")]
    public float damage = 10f;

    [Tooltip("Knockback applied to targets inside the AoE.")]
    public float knockbackForce = 5f;

    [Tooltip("Maximum number of targets that can be hit (0 = unlimited)")]
    [Range(0, 50)]
    public int maxTargets = 0;

    [Header("AoE Shape Settings")]      
    [Tooltip("Shape of the AoE area")]
    public AoEShape shape = AoEShape.Circle;

    [Tooltip("Radius (circle), length (cone/rect/line)")]
    [Range(0.5f, 50f)]
    public float range = 5f;

    [Tooltip("Angle in degrees (only for cone)")]
    [Range(1f, 360f)]
    public float angle = 45f;

    [Tooltip("Size of rectangular AoE (only for rectangle shape)")]
    public Vector2 rectSize = new Vector2(5f, 3f);

    [Tooltip("Width of line AoE (only for line shape)")]
    [Range(0.1f, 10f)]
    public float lineWidth = 1f;

    [Tooltip("Offset from origin point")]
    public Vector2 positionOffset = Vector2.zero;

    [Header("Timing Settings")]
    [Tooltip("Delay before AoE activates (seconds)")]
    [Range(0f, 10f)]
    public float activationDelay = 0f;

    [Tooltip("How long the AoE warning is shown before activation")]
    [Range(0f, 5f)]
    public float warningDuration = 0.5f;

    [Header("Damage Over Time Settings")]
    [Tooltip("Damage dealt per tick (only for OverTime AoEs).")]
    public float damagePerTick = 10f;

    [Tooltip("Time in seconds between each damage tick (only for OverTime AoEs).")]
    [Range(0.05f, 5f)]
    public float tickInterval = 1f;

    [Tooltip("Total duration of the AoE in seconds (only for OverTime AoEs).")]
    [Range(0.1f, 60f)]
    public float duration = 5f;

    [Tooltip("Can the same target be hit multiple times?")]
    public bool canHitSameTargetMultipleTimes = true;

    [Header("Targeting")]
    [Tooltip("Which targets are affected by this AoE")]
    public LayerMask targetLayerMask = -1;

    [Tooltip("Tags that should be ignored")]
    public string[] ignoreTags;

    [Tooltip("Whether the AoE is destroyed when caster dies")]
    public bool destroyOnCasterDeath = true;

    [Tooltip("Whether caster can be affected by their own AoE")]
    public bool canAffectCaster = false;

    [Header("Movement & Behavior")]
    [Tooltip("Whether AoE follows the caster (e.g. aura)")]
    public bool followCaster = false;

    [Tooltip("Movement speed if AoE moves (0 = stationary)")]
    public float movementSpeed = 0f;

    [Tooltip("Direction of movement (normalized)")]
    public Vector2 movementDirection = Vector2.zero;

    [Header("Visual Effects")]
    [Tooltip("Particle effect prefab for the main AoE")]
    public GameObject aoeEffectPrefab;

    [Tooltip("Warning effect shown before AoE activates")]
    public GameObject warningEffectPrefab;

    [Tooltip("Effect played when AoE hits targets")]
    public GameObject impactEffectPrefab;

    [Tooltip("Tint color applied to the AoE effect")]
    public Color aoeColor = Color.red;

    [Tooltip("Warning color for telegraph effect")]
    public Color warningColor = Color.yellow;

    [Tooltip("Scale multiplier for visual effects")]
    [Range(0.1f, 10f)]
    public float effectScale = 1f;

    [Tooltip("Smoothly fade out visuals when AoE ends")]
    public bool fadeOutOnEnd = true;

    [Header("Audio")]
    [Tooltip("Sound played when AoE is cast")]
    public AudioClip castSound;

    [Tooltip("Looped sound while AoE is active")]
    public AudioClip aoeSound;

    [Tooltip("Sound played when AoE hits targets")]
    public AudioClip impactSound;

    [Range(0f, 1f)]
    public float audioVolume = 1f;

    #region Validation
    private void OnValidate()
    {
        range = Mathf.Max(0.5f, range);
        damage = Mathf.Max(0, damage);
        damagePerTick = Mathf.Max(0, damagePerTick);
        tickInterval = Mathf.Max(0.05f, tickInterval);
        duration = Mathf.Max(0.1f, duration);
        lineWidth = Mathf.Max(0.1f, lineWidth);
        effectScale = Mathf.Max(0.1f, effectScale);
        maxTargets = Mathf.Max(0, maxTargets);

        if (rectSize.x < 0.1f) rectSize.x = 0.1f;
        if (rectSize.y < 0.1f) rectSize.y = 0.1f;

        // Normalize movement direction if it's not zero
        if (movementDirection != Vector2.zero)
            movementDirection = movementDirection.normalized;
    }
    #endregion

    #region Utility Methods
    /// <summary>
    /// Returns true if this AoE is circular.
    /// </summary>
    public bool IsCircle() => shape == AoEShape.Circle;

    /// <summary>
    /// Returns true if this AoE is cone-shaped.
    /// </summary>
    public bool IsCone() => shape == AoEShape.Cone;

    /// <summary>
    /// Returns true if this AoE is rectangular.
    /// </summary>
    public bool IsRectangle() => shape == AoEShape.Rectangle;

    /// <summary>
    /// Returns true if this AoE is line-shaped.
    /// </summary>
    public bool IsLine() => shape == AoEShape.Line;

    /// <summary>
    /// Returns true if this AoE deals damage instantly.
    /// </summary>
    public bool IsInstant() => damageType == DamageType.Instant;

    /// <summary>
    /// Returns true if this AoE deals damage over time.
    /// </summary>
    public bool IsOverTime() => damageType == DamageType.OverTime;

    /// <summary>
    /// Returns true if AoE originates at caster position.
    /// </summary>
    public bool IsCasterTargeted() => targetingMode == TargetingMode.CasterPosition;

    /// <summary>
    /// Returns true if AoE originates at mouse position.
    /// </summary>
    public bool IsMouseTargeted() => targetingMode == TargetingMode.MousePosition;

    /// <summary>
    /// Returns true if AoE is cast in a specific direction.
    /// </summary>
    public bool IsDirectionTargeted() => targetingMode == TargetingMode.Direction;

    /// <summary>
    /// Gets the total number of damage ticks for DoT spells.
    /// </summary>
    public int GetTotalTicks()
    {
        if (damageType != DamageType.OverTime) return 1;
        return Mathf.CeilToInt(duration / tickInterval);
    }

    /// <summary>
    /// Gets the total damage this AoE will deal.
    /// </summary>
    public float GetTotalDamage()
    {
        return damageType == DamageType.Instant ? damage : (damagePerTick * GetTotalTicks());
    }
    #endregion
}