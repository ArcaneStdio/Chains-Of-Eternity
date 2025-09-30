using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Configuration data for projectile spells in 2D Unity projects.
/// This ScriptableObject contains all the parameters needed to define how a projectile behaves.
/// </summary>
/// 
[CreateAssetMenu(fileName = "New Projectile Spell", menuName = "Spell System/Projectile Spell Data")]
    #region Range of Values
public class ProjectileData : ScriptableObject
{   
    [Header("Target Settings")]
    [Tooltip("Layers that this projectile can hit")]
    public LayerMask targetLayerMask =11; // Enemy by default

    [Header("Combat Settings")]
    [Tooltip("Damage dealt to targets on hit")]
    [Range(0f, 50f)]
    public int damage = 25;
    
    [Tooltip("Knockback force applied to hit targets")]
    public float knockbackForce = 10f;
    
    [Tooltip("Whether the projectile destroys itself on hit")]
    public bool destroyOnHit = true;
    
    [Tooltip("Whether the projectile can pierce through multiple targets")]
    public bool canPierce = false;
    
    [Tooltip("Maximum number of targets this projectile can hit before being destroyed")]
    public int maxPierceCount = 2;
    
    [Header("Movement Settings")]
    [Tooltip("Type of movement pattern for this projectile")]
    public ProjectilePath movementPath = ProjectilePath.Straight;
    
    [Tooltip("Speed of the projectile")]
    [Range(0f, 50f)]
    public float projectileSpeed = 15f;
    
    [Tooltip("Lifetime of the projectile in seconds")]
    public float projectileLifeTime = 3.5f;
    
    [Header("Spawn Settings")]
    [Tooltip("Projectile Size Multiplier")]
    [Range(0.75f, 1.5f)]
    public float projectileSize = 1f;
    [Tooltip("Number of projectiles to spawn")]
    [Range(1, 10)]
    public int numberOfProjectiles = 1;
    
    [Tooltip("Delay between multiple projectiles in seconds")]
    [Range(0f, 1f)]
    public float delayBetweenProjectiles = 0f;
    
    [Tooltip("Spawn position offsets relative to the caster")]
    public List<Vector2> spawnOffsets = new List<Vector2> { Vector2.zero };
    
    [Tooltip("Angle offset for staggered launches in degrees")]
    [Range(-90f, 90f)]
    public float staggeredLaunchAngle = 0f;
    
    [Tooltip("Initial directions for the projectiles , Affect from the direction of mouse")]
    public List<Vector2> directions = new List<Vector2> { new Vector2(1,1).normalized };
    
    [Header("Movement Pattern Specific Settings")]
    [Header("ZigZag Settings")]
    [Tooltip("Amplitude of the zigzag movement")]
    [Range(5f, 30f)]
    public float zigzagAmplitude = 10f;
    
    [Tooltip("Frequency of the zigzag movement")]
    [Range(1f, 25f)]
    public float zigzagFrequency = 10f;

    [Header("Homing Settings")]
    [Tooltip("Delay before homing behavior starts")]
    [Range(0f, 2f)]
    public float homingDelay = 0.1f;

    [Tooltip("Radius within which to search for targets")]
    [Range(1f, 20f)]
    public float homingRadius = 10f;
    
    [Tooltip("How often to update homing direction")]
    public float homingUpdateRate = 0.2f;
    
    [Header("Circular Settings")]
    [Tooltip("Initial radius of the circular movement")]
    [Range(0.1f, 4f)]
    public float circularInitialRadius = 1f;
    
    [Tooltip("Speed of circular rotation")]
    [Range(0.1f, 10f)]
    public float circularSpeed = 4f;
    
    [Tooltip("Speed at which the radius expands")]
    [Range(-5f, 5f)]
    public float circularRadialSpeed = 0.8f;
    
    [Header("Random Settings")]
    [Tooltip("Maximum random offset for direction variation")]
    [Range(0f, 1f)]
    public float randomDirectionOffset = 0.2f;
    
    [Header("Arc Settings")]
    [Tooltip("Gravity scale for arc movement")]
    [Range(0f, 5f)]
    public float arcGravityScale = 1f;
    
    [Header("Visual and Audio Settings")]
    [Tooltip("Particle effect to spawn on hit")]
    public GameObject hitEffectPrefab;
    
    [Tooltip("Sound effect to play on hit")]
    public AudioClip hitSound;
    
    [Tooltip("Sound effect to play on spawn")]
    public AudioClip spawnSound;
    
    [Tooltip("Whether to rotate the projectile sprite to face movement direction")]
    public bool rotateToFaceDirection = true;
    
    [Header("Advanced Settings")]
    [Tooltip("Custom tags for this projectile (useful for filtering)")]
    public List<string> projectileTags = new List<string>();
    
    [Tooltip("Whether to use custom physics material")]
    public bool useCustomPhysicsMaterial = false;
    
    [Tooltip("Custom physics material for the projectile")]
    public PhysicsMaterial2D customPhysicsMaterial;
    
    /// <summary>
    /// Different movement patterns available for projectiles.
    /// </summary>
    public enum ProjectilePath 
    { 
        [Tooltip("Moves in a straight line")]
        Straight, 
        
        [Tooltip("Moves in a zigzag pattern")]
        ZigZag, 
        
        [Tooltip("Moves with random direction variation")]
        Random, 
        
        [Tooltip("Moves in an arc affected by gravity")]
        Arc, 
        
        [Tooltip("Homes in on the nearest enemy")]
        Homing, 
        
        [Tooltip("Moves in a circular pattern around the caster")]
        Circular 
    }
    #endregion
    #region Validation
    
    private void OnValidate()
    {
        // Ensure minimum values
        damage = Mathf.Max(0, damage);
        knockbackForce = Mathf.Max(0, knockbackForce);
        projectileSpeed = Mathf.Max(0.1f, projectileSpeed);
        projectileLifeTime = Mathf.Max(0.1f, projectileLifeTime);
        numberOfProjectiles = Mathf.Max(1, numberOfProjectiles);
        delayBetweenProjectiles = Mathf.Max(0f, delayBetweenProjectiles);
        maxPierceCount = Mathf.Max(1, maxPierceCount);
        
        // Ensure spawn offsets list is not empty
        if (spawnOffsets.Count == 0)
        {
            spawnOffsets.Add(Vector2.zero);
        }
        
        // Ensure directions list is not empty
        if (directions.Count == 0)
        {
            directions.Add(new Vector2(1,1).normalized);
        }
        
        // Normalize directions
        for (int i = 0; i < directions.Count; i++)
        {
            if (directions[i].magnitude > 0)
            {
                directions[i] = directions[i].normalized;
            }
        }
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Gets a spawn offset at the specified index, with bounds checking.
    /// </summary>
    public Vector2 GetSpawnOffset(int index)
    {
        if (spawnOffsets.Count == 0) return Vector2.zero;
        return spawnOffsets[index % spawnOffsets.Count];
    }
    
    /// <summary>
    /// Gets a direction at the specified index, with bounds checking.
    /// </summary>
    public Vector2 GetDirection(int index)
    {
        if (directions.Count == 0) return Vector2.right;
        return directions[index % directions.Count];
    }
    
    /// <summary>
    /// Gets the total number of spawn configurations (spawnOffsets * directions).
    /// </summary>
    public int GetTotalSpawnConfigurations()
    {
        return spawnOffsets.Count * directions.Count;
    }
    
    /// <summary>
    /// Checks if this projectile has a specific tag.
    /// </summary>
    public bool HasTag(string tag)
    {
        return projectileTags.Contains(tag);
    }
    
    /// <summary>
    /// Adds a tag to this projectile if it doesn't already exist.
    /// </summary>
    public void AddTag(string tag)
    {
        if (!projectileTags.Contains(tag))
        {
            projectileTags.Add(tag);
        }
    }
    
    /// <summary>
    /// Removes a tag from this projectile.
    /// </summary>
    public void RemoveTag(string tag)
    {
        projectileTags.Remove(tag);
    }
    
    #endregion
}