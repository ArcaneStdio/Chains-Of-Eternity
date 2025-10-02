//using System;
//using System.Collections.Generic;
//using Thirdweb.Unity;
//using UnityEngine;

///// <summary>
///// Converts SpellObject and related data into JSON-safe DTOs
///// (ignores UnityEngine.Object references like GameObject, AudioClip, etc.)
///// </summary>
//public static class SpellJsonConverter
//{

//    private class SpellDTO
//    {
//        public string spellName;
//        public ElementType element;
//        public float manaCost;
//        public float cooldown;

//        public AttackSubtype attackSubtype;
//        public AttackDataDTO attackData;
//    }
//    private class AttackDataDTO
//    {
//        public AoEDataDTO aoeData;
//        public ProjectileDataDTO projectileData;
//        public ShortRangeData shortRangeData;
//    }
//    private class AoEDataDTO
//    {
//        public string spellName;
//        public string damageType;
//        public string targetingMode;
//        public float damage;
//        public float knockbackForce;
//        public int maxTargets;
//        public string shape;
//        public float range;
//        public float angle;
//        public Vector2 rectSize;
//        public float lineWidth;
//        public Vector2 positionOffset;

//        public float activationDelay;
//        public float warningDuration;

//        public float damagePerTick;
//        public float tickInterval;
//        public float duration;
//        public bool canHitSameTargetMultipleTimes;

//        public int targetLayerMask;
//        public string[] ignoreTags;
//        public bool destroyOnCasterDeath;
//        public bool canAffectCaster;

//        public bool followCaster;
//        public float movementSpeed;
//        public Vector2 movementDirection;

//        public Color aoeColor;
//        public Color warningColor;
//        public float effectScale;
//        public bool fadeOutOnEnd;

//        public float audioVolume;
//    }

//    private class ProjectileDataDTO
//    {
//        public int targetLayerMask;
//        public int damage;
//        public float knockbackForce;
//        public bool destroyOnHit;
//        public bool canPierce;
//        public int maxPierceCount;

//        public string movementPath;
//        public float projectileSpeed;
//        public float projectileLifeTime;

//        public float projectileSize;
//        public int numberOfProjectiles;
//        public float delayBetweenProjectiles;
//        public List<Vector2> spawnOffsets;
//        public float staggeredLaunchAngle;
//        public List<Vector2> directions;

//        public float zigzagAmplitude;
//        public float zigzagFrequency;

//        public float homingDelay;
//        public float homingRadius;
//        public float homingUpdateRate;

//        public float circularInitialRadius;
//        public float circularSpeed;
//        public float circularRadialSpeed;

//        public float randomDirectionOffset;
//        public float arcGravityScale;

//        public bool rotateToFaceDirection;
//        public List<string> projectileTags;
//        public bool useCustomPhysicsMaterial;
//    }

//    // --- Converter Methods ---
//    public static string ToJson(SpellObject spell)
//    {
//        SpellDTO dto = new SpellDTO
//        {
//            spellName = spell.spellName,
//            element = spell.element,
//            manaCost = spell.manaCost,
//            cooldown = spell.cooldown,
//            attackSubtype = spell.attackSubtype,
//            attackData = ConvertAttackData(spell.attackData)
//        };

//        return JsonUtility.ToJson(dto, true);
//    }

//    private static AttackDataDTO ConvertAttackData(AttackData attackData)
//    {
//        if (attackData == null) return null;

//        return new AttackDataDTO
//        {
//            aoeData = attackData.aoeData ? ConvertAoEData(attackData.aoeData) : null,
//            projectileData = attackData.projectileData ? ConvertProjectileData(attackData.projectileData) : null,
//            shortRangeData = attackData.shortRangeData // already serializable (all primitives + Vector2)
//        };
//    }

//    private static AoEDataDTO ConvertAoEData(AoEData aoe)
//    {
//        return new AoEDataDTO
//        {
//            spellName = aoe.spellName,
//            damageType = aoe.damageType.ToString(),
//            targetingMode = aoe.targetingMode.ToString(),
//            damage = aoe.damage,
//            knockbackForce = aoe.knockbackForce,
//            maxTargets = aoe.maxTargets,
//            shape = aoe.shape.ToString(),
//            range = aoe.range,
//            angle = aoe.angle,
//            rectSize = aoe.rectSize,
//            lineWidth = aoe.lineWidth,
//            positionOffset = aoe.positionOffset,

//            activationDelay = aoe.activationDelay,
//            warningDuration = aoe.warningDuration,

//            damagePerTick = aoe.damagePerTick,
//            tickInterval = aoe.tickInterval,
//            duration = aoe.duration,
//            canHitSameTargetMultipleTimes = aoe.canHitSameTargetMultipleTimes,

//            targetLayerMask = aoe.targetLayerMask,
//            ignoreTags = aoe.ignoreTags,
//            destroyOnCasterDeath = aoe.destroyOnCasterDeath,
//            canAffectCaster = aoe.canAffectCaster,

//            followCaster = aoe.followCaster,
//            movementSpeed = aoe.movementSpeed,
//            movementDirection = aoe.movementDirection,

//            aoeColor = aoe.aoeColor,
//            warningColor = aoe.warningColor,
//            effectScale = aoe.effectScale,
//            fadeOutOnEnd = aoe.fadeOutOnEnd,

//            audioVolume = aoe.audioVolume
//        };
//    }

//    private static ProjectileDataDTO ConvertProjectileData(ProjectileData proj)
//    {
//        return new ProjectileDataDTO
//        {
//            targetLayerMask = proj.targetLayerMask,
//            damage = proj.damage,
//            knockbackForce = proj.knockbackForce,
//            destroyOnHit = proj.destroyOnHit,
//            canPierce = proj.canPierce,
//            maxPierceCount = proj.maxPierceCount,

//            movementPath = proj.movementPath.ToString(),
//            projectileSpeed = proj.projectileSpeed,
//            projectileLifeTime = proj.projectileLifeTime,

//            projectileSize = proj.projectileSize,
//            numberOfProjectiles = proj.numberOfProjectiles,
//            delayBetweenProjectiles = proj.delayBetweenProjectiles,
//            spawnOffsets = proj.spawnOffsets,
//            staggeredLaunchAngle = proj.staggeredLaunchAngle,
//            directions = proj.directions,

//            zigzagAmplitude = proj.zigzagAmplitude,
//            zigzagFrequency = proj.zigzagFrequency,

//            homingDelay = proj.homingDelay,
//            homingRadius = proj.homingRadius,
//            homingUpdateRate = proj.homingUpdateRate,

//            circularInitialRadius = proj.circularInitialRadius,
//            circularSpeed = proj.circularSpeed,
//            circularRadialSpeed = proj.circularRadialSpeed,

//            randomDirectionOffset = proj.randomDirectionOffset,
//            arcGravityScale = proj.arcGravityScale,

//            rotateToFaceDirection = proj.rotateToFaceDirection,
//            projectileTags = proj.projectileTags,
//            useCustomPhysicsMaterial = proj.useCustomPhysicsMaterial
//        };
//    }
//}
