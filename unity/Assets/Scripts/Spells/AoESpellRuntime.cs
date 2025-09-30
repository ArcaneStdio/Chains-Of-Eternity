using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runtime controller for an AoE spell in a top-down 2D game.
/// Handles all shapes, particle effects, damage application, and lifecycle management.
/// </summary>
public class AoESpellRuntime : MonoBehaviour
{
    [SerializeField] private AoEData data;
    [SerializeField] private Transform caster;
    [SerializeField] private Vector2 targetDirection = Vector2.up; // For directional casting

    // Visual Components
    private GameObject effectInstance;
    private GameObject warningInstance;
    private ParticleSystem mainParticles;
    private ParticleSystem warningParticles;
    
    // Audio
    private AudioSource audioSource;
    
    // State Management
    private float elapsedTime;
    private float warningElapsedTime;
    private bool isActive = false;
    private bool isWarning = true;
    
    // Damage Tracking
    private Coroutine damageRoutine;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
    private List<GameObject> currentTargetsInRange = new List<GameObject>();
    
    // Movement
    private Vector2 currentVelocity;

    #region Initialization
    /// <summary>
    /// Initialize the AoE with data and caster reference
    /// </summary>
    public void Initialize(AoEData aoeData, Transform casterTransform, Vector2? direction = null)
    {
        data = aoeData;
        caster = casterTransform;
        
        if (direction.HasValue)
            targetDirection = direction.Value.normalized;
        else if (caster != null)
            targetDirection = caster.up; // Default to caster facing direction
            
        StartAoE();
    }

    private void StartAoE()
    {
        if (data == null)
        {
            Debug.LogError("AoEData is missing!");
            Destroy(gameObject);
            return;
        }

        // Set initial position
        SetInitialPosition();
        
        // Initialize movement
        if (data.movementSpeed > 0 && data.movementDirection != Vector2.zero)
        {
            currentVelocity = data.movementDirection * data.movementSpeed;
        }

        // Show warning effect first
        if (data.warningDuration > 0 && data.warningEffectPrefab != null)
        {
            CreateWarningEffect();
        }
        else
        {
            // Skip warning, activate immediately
            ActivateAoE();
        }

        // Play cast sound
        PlaySound(data.castSound);
    }

    private void SetInitialPosition()
    {
        Vector3 spawnPos = Vector3.zero;

        switch (data.targetingMode)
        {
            case AoEData.TargetingMode.MousePosition:
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spawnPos = new Vector3(mousePos.x, mousePos.y, 0);
                break;
                
            case AoEData.TargetingMode.CasterPosition:
                spawnPos = caster != null ? caster.position : transform.position;
                // Update direction to point towards mouse for caster-targeted spells
                UpdateDirectionToMouse();
                break;
                
            case AoEData.TargetingMode.Direction:
                Vector3 casterPos = caster != null ? caster.position : transform.position;
                spawnPos = casterPos + (Vector3)(targetDirection * data.range * 0.5f);
                break;
        }

        // Apply offset
        spawnPos += (Vector3)data.positionOffset;
        spawnPos.z = 0; // Ensure 2D positioning
        transform.position = spawnPos;

        // Set rotation for directional spells
        if (data.IsCone() || data.IsLine() || data.targetingMode == AoEData.TargetingMode.Direction || 
            (data.targetingMode == AoEData.TargetingMode.CasterPosition && (data.IsCone() || data.IsLine())))
        {
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward); // -90 because Unity sprites face up by default
        }
    }

    private void UpdateDirectionToMouse()
    {
        if (caster == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        Vector2 directionToMouse = (mouseWorldPos - caster.position).normalized;
        targetDirection = directionToMouse;
    }
    #endregion

    #region Visual Effects
    private void CreateWarningEffect()
    {
        if (data.warningEffectPrefab == null) return;

        warningInstance = Instantiate(data.warningEffectPrefab, transform);
        SetupEffectScale(warningInstance);
        
        // Setup warning particles
        warningParticles = warningInstance.GetComponent<ParticleSystem>();
        if (warningParticles != null)
        {
            var main = warningParticles.main;
            main.startColor = data.warningColor;
            
            // Adjust shape based on AoE type
            ConfigureParticleShape(warningParticles);
        }

        // Setup warning renderer - handle different material properties
        var renderer = warningInstance.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            SetMaterialColor(renderer.material, data.warningColor);
        }

        var spriteRenderer = warningInstance.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = data.warningColor;
            spriteRenderer.sortingOrder = 3; // Below main effect
        }
    }

    private void CreateMainEffect()
    {
        if (data.aoeEffectPrefab == null) return;

        effectInstance = Instantiate(data.aoeEffectPrefab, transform);
        SetupEffectScale(effectInstance);

        // Setup main particles
        mainParticles = effectInstance.GetComponent<ParticleSystem>();
        if (mainParticles != null)
        {
            var main = mainParticles.main;
            main.startColor = data.aoeColor;
            
            // Configure particle shape and settings
            ConfigureParticleShape(mainParticles);
        }

        // Setup main renderer - handle different material properties
        var renderer = effectInstance.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            SetMaterialColor(renderer.material, data.aoeColor);
        }

        var spriteRenderer = effectInstance.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = data.aoeColor;
            spriteRenderer.sortingOrder = 5; // Above ground and warning
        }
    }

    private void ConfigureParticleShape(ParticleSystem particles)
    {
        if (particles == null) return;

        // Stop the system before making changes
        if (particles.isPlaying)
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        var shape = particles.shape;
        var main = particles.main;
        var velocityOverLifetime = particles.velocityOverLifetime;
        
        // Configure basic shape
        switch (data.shape)
        {
            case AoEData.AoEShape.Circle:
                shape.shapeType = ParticleSystemShapeType.Circle;
                shape.radius = data.range;
                ConfigureCircleParticles(particles);
                break;
                
            case AoEData.AoEShape.Rectangle:
                shape.shapeType = ParticleSystemShapeType.Rectangle;
                shape.scale = new Vector3(data.rectSize.x, data.rectSize.y, 1f);
                ConfigureRectangleParticles(particles);
                break;
                
            case AoEData.AoEShape.Cone:
                shape.shapeType = ParticleSystemShapeType.Cone;
                shape.angle = data.angle;
                shape.radius = data.range;
                ConfigureConeParticles(particles);
                break;
                
            case AoEData.AoEShape.Line:
                shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
                shape.radius = data.range;
                ConfigureLineParticles(particles);
                break;
        }

        // Configure duration and loop settings properly
        if (data.IsOverTime())
        {
            main.duration = data.duration;
            main.loop = true;
        }
        else
        {
            main.duration = 1f; // Short burst for instant spells
            main.loop = false;
        }

        // Restart the system after configuration
        particles.Play();
    }

    private void ConfigureCircleParticles(ParticleSystem particles)
    {
        var shape = particles.shape;
        var emission = particles.emission;
        
        // For circle, particles can emanate outward or stay within area
        if (data.targetingMode == AoEData.TargetingMode.CasterPosition)
        {
            // Particles spread in all directions
            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = false; // Let natural particle behavior handle it
        }
    }

    private void ConfigureRectangleParticles(ParticleSystem particles)
    {
        var shape = particles.shape;
        // Rectangle particles typically stay within the area
        var velocityOverLifetime = particles.velocityOverLifetime;
        velocityOverLifetime.enabled = false;
    }

    private void ConfigureConeParticles(ParticleSystem particles)
    {
        var shape = particles.shape;
        var velocityOverLifetime = particles.velocityOverLifetime;
        var emission = particles.emission;

        // Configure cone to spread particles in proper cone area
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = data.angle;
        shape.radius = data.range;
        shape.radiusThickness = 0.1f; // Particles emit from edge, not center
        
        if (data.targetingMode == AoEData.TargetingMode.CasterPosition)
        {
            // Disable velocity override - let cone shape handle natural spread
            velocityOverLifetime.enabled = false;
            
            // Set emission rate for good cone coverage
            emission.rateOverTime = 30f;
            
            // Particles should naturally spread in cone area without forced direction
            var main = particles.main;
            main.startSpeed = data.range * 0.5f; // Speed based on range
            main.startLifetime = 2f; // Longer lifetime to fill cone area
        }
    }

    private void ConfigureLineParticles(ParticleSystem particles)
    {
        var shape = particles.shape;
        var velocityOverLifetime = particles.velocityOverLifetime;
        var emission = particles.emission;

        if (data.targetingMode == AoEData.TargetingMode.CasterPosition)
        {
            // Configure line to emit particles in a beam towards mouse direction
            shape.shapeType = ParticleSystemShapeType.SingleSidedEdge;
            shape.radius = data.lineWidth * 0.5f;
            
            // Enable velocity for beam effect
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.World;
            
            // Set velocity in the target direction
            float speed = data.range; // Faster for line/beam effects
            velocityOverLifetime.x = targetDirection.x * speed;
            velocityOverLifetime.y = targetDirection.y * speed;
            velocityOverLifetime.z = 0f;
            
            // High emission rate for solid beam effect
            emission.rateOverTime = 100f;
            
            // Set particle lifetime based on range and speed
            var main = particles.main;
            main.startLifetime = data.range / speed;
        }
    }

    private void SetupEffectScale(GameObject effect)
    {
        if (effect != null && data.effectScale != 1f)
        {
            effect.transform.localScale = Vector3.one * data.effectScale;
        }
    }
    #endregion

    #region Update Loop
    private void Update()
    {
        HandleWarningPhase();
        
        if (!isActive) return;

        // Update direction only for Line shape (continuous mouse tracking)
        if (!data.followCaster && data.targetingMode == AoEData.TargetingMode.CasterPosition && data.IsLine())
        {
            UpdateDirectionToMouse();
            UpdateParticleDirection();
            
            // Update rotation
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }

        UpdateMovement();
        UpdateFollowCaster();
        HandleDurationAndDestroy();
    }

    private void HandleWarningPhase()
    {
        if (!isWarning) return;

        warningElapsedTime += Time.deltaTime;
        
        if (warningElapsedTime >= data.warningDuration)
        {
            isWarning = false;
            ActivateAoE();
        }
    }

    private void UpdateMovement()
    {
        if (currentVelocity != Vector2.zero)
        {
            transform.Translate(currentVelocity * Time.deltaTime, Space.World);
        }
    }

    private void UpdateFollowCaster()
    {
        if (data.followCaster && caster != null)
        {
            Vector3 newPos = caster.position + (Vector3)data.positionOffset;
            newPos.z = 0;
            transform.position = newPos;
            
            // Update direction only for Line shape (not Cone)
            if (data.targetingMode == AoEData.TargetingMode.CasterPosition && data.IsLine())
            {
                UpdateDirectionToMouse();
                UpdateParticleDirection();
                
                // Update rotation
                float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            }
        }
    }

    private void UpdateParticleDirection()
    {
        // Update main particles direction (only for Line)
        if (mainParticles != null && data.IsLine())
        {
            var velocityOverLifetime = mainParticles.velocityOverLifetime;
            if (velocityOverLifetime.enabled)
            {
                float speed = data.range;
                velocityOverLifetime.x = targetDirection.x * speed;
                velocityOverLifetime.y = targetDirection.y * speed;
            }
        }
        
        // Update warning particles direction (only for Line)
        if (warningParticles != null && data.IsLine())
        {
            var velocityOverLifetime = warningParticles.velocityOverLifetime;
            if (velocityOverLifetime.enabled)
            {
                float speed = data.range;
                velocityOverLifetime.x = targetDirection.x * speed;
                velocityOverLifetime.y = targetDirection.y * speed;
            }
        }
    }

    private void HandleDurationAndDestroy()
    {
        elapsedTime += Time.deltaTime;

        // Check if should be destroyed
        if (data.IsOverTime() && elapsedTime >= data.duration)
        {
            EndAoE();
        }
        else if (data.destroyOnCasterDeath && (caster == null || !caster.gameObject.activeInHierarchy))
        {
            EndAoE();
        }
    }
    #endregion

    #region Activation
    private void ActivateAoE()
    {
        isActive = true;
        
        // Destroy warning effect
        if (warningInstance != null)
        {
            Destroy(warningInstance);
        }
        
        // Create main effect
        CreateMainEffect();
        
        // Setup looping sound
        SetupLoopingSound();
        
        // Start damage logic
        if (data.damageType == AoEData.DamageType.Instant)
        {
            ApplyDamageToAllTargets();
            
            if (!data.IsOverTime()) // Pure instant AoE
            {
                StartCoroutine(DelayedDestroy(0.5f)); // Give time for visuals
            }
        }
        else
        {
            damageRoutine = StartCoroutine(DamageTickLoop());
        }
    }

    private void SetupLoopingSound()
    {
        if (data.aoeSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = data.aoeSound;
            audioSource.loop = data.IsOverTime();
            audioSource.volume = data.audioVolume;
            audioSource.spatialBlend = 0f; // 2D sound
            audioSource.Play();
        }
    }
    #endregion

    #region Damage System
    private IEnumerator DamageTickLoop()
    {
        while (elapsedTime < data.duration)
        {
            ApplyDamageToAllTargets();
            yield return new WaitForSeconds(data.tickInterval);
        }
    }

    private void ApplyDamageToAllTargets()
    {
        currentTargetsInRange.Clear();
        FindTargetsInAoE();
        
        int targetsHit = 0;
        
        foreach (var target in currentTargetsInRange)
        {
            if (data.maxTargets > 0 && targetsHit >= data.maxTargets)
                break;
                
            if (ShouldHitTarget(target))
            {
                ApplyDamageAndEffects(target);
                targetsHit++;
                
                if (!data.canHitSameTargetMultipleTimes)
                {
                    hitTargets.Add(target);
                }
            }
        }
    }

    private void FindTargetsInAoE()
    {
        Collider2D[] hits = null;
        
        switch (data.shape)
        {
            case AoEData.AoEShape.Circle:
                hits = Physics2D.OverlapCircleAll(transform.position, data.range, data.targetLayerMask);
                break;
                
            case AoEData.AoEShape.Rectangle:
                hits = Physics2D.OverlapBoxAll(transform.position, data.rectSize, transform.eulerAngles.z, data.targetLayerMask);
                break;
                
            case AoEData.AoEShape.Line:
                // Use capsule for line detection
                hits = Physics2D.OverlapCapsuleAll(transform.position, new Vector2(data.lineWidth, data.range), 
                    CapsuleDirection2D.Vertical, transform.eulerAngles.z, data.targetLayerMask);
                break;
                
            case AoEData.AoEShape.Cone:
                // For cone, first get circle then filter by angle
                hits = Physics2D.OverlapCircleAll(transform.position, data.range, data.targetLayerMask);
                break;
        }

        if (hits == null) return;

        foreach (var hit in hits)
        {
            if (IsValidTarget(hit.gameObject))
            {
                currentTargetsInRange.Add(hit.gameObject);
            }
        }

        // Special handling for cone shape
        if (data.IsCone())
        {
            FilterConeTargets();
        }
    }

    private void FilterConeTargets()
    {
        var filteredTargets = new List<GameObject>();
        Vector2 coneDirection = targetDirection;
        
        foreach (var target in currentTargetsInRange)
        {
            Vector2 dirToTarget = (target.transform.position - transform.position).normalized;
            float angleToTarget = Vector2.Angle(coneDirection, dirToTarget);
            
            if (angleToTarget <= data.angle / 2f)
            {
                filteredTargets.Add(target);
            }
        }
        
        currentTargetsInRange = filteredTargets;
    }

    private bool IsValidTarget(GameObject target)
    {
        // Check if it's the caster
        if (!data.canAffectCaster && caster != null && target == caster.gameObject)
            return false;

        // Check ignored tags
        if (data.ignoreTags != null)
        {
            foreach (string tag in data.ignoreTags)
            {
                if (target.CompareTag(tag))
                    return false;
            }
        }

        return true;
    }

    private bool ShouldHitTarget(GameObject target)
    {
        // Check if target was already hit (for non-repeating damage)
        if (!data.canHitSameTargetMultipleTimes && hitTargets.Contains(target))
            return false;

        // Check if target has damageable component
        return target.GetComponent<IDamageable>() != null;
    }

    private void ApplyDamageAndEffects(GameObject target)
    {
        float damageAmount = data.IsInstant() ? data.damage : data.damagePerTick;
        
        // Apply damage
        var damageable = target.GetComponent<IDamageable>();
        damageable?.TakeDamage((int)damageAmount);

        // Apply knockback
        if (data.knockbackForce > 0)
        {
            var rb = target.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockDirection = (target.transform.position - transform.position).normalized;
                rb.AddForce(knockDirection * data.knockbackForce, ForceMode2D.Impulse);
            }
        }

        // Play impact effect
        if (data.impactEffectPrefab != null)
        {
            Instantiate(data.impactEffectPrefab, target.transform.position, Quaternion.identity);
        }

        // Play impact sound
        PlaySound(data.impactSound);
    }
    #endregion

    #region Audio
    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        // Create temporary audio source for one-shot sounds
        GameObject audioGO = new GameObject("AoE Audio");
        AudioSource tempSource = audioGO.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = data.audioVolume;
        tempSource.spatialBlend = 0f;
        tempSource.Play();

        // Destroy after clip finishes
        Destroy(audioGO, clip.length);
    }
    /// <summary>
    /// Safely sets material color, handling different shader property names
    /// </summary>
    private void SetMaterialColor(Material material, Color color)
    {
        if (material == null) return;

        // Try common color property names
        if (material.HasProperty("_Color"))
        {
            material.color = color;
        }
        else if (material.HasProperty("_TintColor"))
        {
            material.SetColor("_TintColor", color);
        }
        else if (material.HasProperty("_MainColor"))
        {
            material.SetColor("_MainColor", color);
        }
        else if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }
        else
        {
            // Fallback: try to set the main texture color if available
            if (material.HasProperty("_MainTex"))
            {
                // Can't set color directly, but at least we tried
                Debug.LogWarning($"Material '{material.name}' doesn't have a recognized color property. Color change ignored.");
            }
        }
    }

    #endregion

    #region Cleanup
    private void EndAoE()
    {
        if (damageRoutine != null)
        {
            StopCoroutine(damageRoutine);
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (data.fadeOutOnEnd)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FadeOutAndDestroy()
    {
        float fadeTime = 1f;
        float t = 0f;

        // Collect all renderers and particle systems
        var renderers = GetComponentsInChildren<Renderer>();
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        
        // Stop particle emission
        foreach (var ps in particleSystems)
        {
            var emission = ps.emission;
            emission.enabled = false;
        }

        // Store original colors
        var originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
            else if (renderers[i].material.HasProperty("_TintColor"))
                originalColors[i] = renderers[i].material.GetColor("_TintColor");
            else
                originalColors[i] = Color.white; // Fallback
        }

        while (t < fadeTime)
        {
            float alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
            
            // Fade renderers
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null && renderers[i].material != null)
                {
                    Color newColor = originalColors[i];
                    newColor.a = alpha;
                    SetMaterialColor(renderers[i].material, newColor);
                }
            }
            
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    #endregion

    #region Gizmos
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Vector3 pos = Application.isPlaying ? transform.position : transform.position + (Vector3)data.positionOffset;

        switch (data.shape)
        {
            case AoEData.AoEShape.Circle:
                Gizmos.DrawWireSphere(pos, data.range);
                break;

            case AoEData.AoEShape.Rectangle:
                Gizmos.matrix = Matrix4x4.TRS(pos, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, data.rectSize);
                Gizmos.matrix = Matrix4x4.identity;
                break;

            case AoEData.AoEShape.Line:
                Gizmos.matrix = Matrix4x4.TRS(pos, transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(data.lineWidth, data.range, 0.1f));
                Gizmos.matrix = Matrix4x4.identity;
                break;

            case AoEData.AoEShape.Cone:
                // Draw cone approximation
                Vector2 direction = Application.isPlaying ? targetDirection : (caster != null ? (Vector2)caster.up : Vector2.up);
                DrawConeGizmo(pos, direction, data.range, data.angle);
                break;
        }
    }

    private void DrawConeGizmo(Vector3 origin, Vector2 direction, float range, float angle)
    {
        float halfAngle = angle * 0.5f * Mathf.Deg2Rad;
        Vector2 leftDir = new Vector2(
            direction.x * Mathf.Cos(halfAngle) - direction.y * Mathf.Sin(halfAngle),
            direction.x * Mathf.Sin(halfAngle) + direction.y * Mathf.Cos(halfAngle)
        );
        Vector2 rightDir = new Vector2(
            direction.x * Mathf.Cos(-halfAngle) - direction.y * Mathf.Sin(-halfAngle),
            direction.x * Mathf.Sin(-halfAngle) + direction.y * Mathf.Cos(-halfAngle)
        );

        Gizmos.DrawLine(origin, origin + (Vector3)(leftDir * range));
        Gizmos.DrawLine(origin, origin + (Vector3)(rightDir * range));
        Gizmos.DrawLine(origin + (Vector3)(leftDir * range), origin + (Vector3)(rightDir * range));
    }
    #endregion
}