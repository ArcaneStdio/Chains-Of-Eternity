using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class ProjectileSpellRuntime : MonoBehaviour
{
    [Header("Runtime References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = false;
    [SerializeField] private bool showTrajectory = false;

    private ProjectileData data;
    private Vector2 direction;
    private Transform caster;
    private Vector3 spawnPos;
    private IMovementPattern movement;
    private bool initialized = false;
    private int hitCount = 0;
    private Enemy enemy;
    private PlayerStats playerStats;

    public System.Action<GameObject> OnTargetHit;
    public System.Action OnProjectileDestroyed;

    private void Awake()
    {
        rb ??= GetComponent<Rigidbody2D>();
        col ??= GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        if (initialized) movement?.UpdateMovement(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == caster || other.transform.IsChildOf(caster))
            return;
        if (((1 << other.gameObject.layer) & data.targetLayerMask) != 0)
        {
            ApplyDamageAndKnockback(other.gameObject);
            OnTargetHit?.Invoke(other.gameObject);
            hitCount++;
            if (data.destroyOnHit || hitCount >= data.maxPierceCount) DestroyProjectile();
        }
    }

    private void OnDestroy() => OnProjectileDestroyed?.Invoke();

    /// <summary>
    /// Initialize projectile with caster and data. Calculates direction toward the mouse.
    /// </summary>
    public void Initialize(ProjectileData d, Transform c, Vector2 Direction)
    {
        if (d == null) { Debug.LogError("ProjectileData null"); return; }

        data = d;
        caster = c;
        spawnPos = transform.position;

        // Calculate direction toward mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        direction = Direction;

        SetupRigidbody();

        movement = CreateMovementPattern();
        movement.Initialize(this, data, direction);

        StartCoroutine(DestroyAfterLifetime());
        initialized = true;

        if (enableDebugLogs) Debug.Log($"Projectile launched toward mouse. Speed: {data.projectileSpeed}");
    }

    public Vector2 GetCurrentVelocity() => rb.linearVelocity;
    public void SetVelocity(Vector2 v) => rb.linearVelocity = v;
    public Vector3 GetPosition() => transform.position;
    public Vector3 GetSpawnPosition() => spawnPos;
    public Transform GetCaster() => caster;

    private void SetupRigidbody()
    {
        rb.gravityScale = 0;
        rb.linearDamping = 0;
        rb.angularDamping = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (data.useCustomPhysicsMaterial && data.customPhysicsMaterial != null)
            col.sharedMaterial = data.customPhysicsMaterial;
    }

    private IMovementPattern CreateMovementPattern() => data.movementPath switch
    {
        ProjectileData.ProjectilePath.Straight => new StraightMovementPattern(),
        ProjectileData.ProjectilePath.Arc => new ArcMovementPattern(),
        ProjectileData.ProjectilePath.Homing => new HomingMovementPattern(),
        ProjectileData.ProjectilePath.ZigZag => new ZigZagMovementPattern(),
        ProjectileData.ProjectilePath.Random => new RandomMovementPattern(),
        ProjectileData.ProjectilePath.Circular => new CircularMovementPattern(),
        _ => new StraightMovementPattern()
    };

    private void ApplyDamageAndKnockback(GameObject target)
    {
        enemy = target.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(data.damage, caster.position, data.knockbackForce, data.knockbackForce > 0, true, true, "Magical");
            Debug.Log($"Projectile hit {target.name}, dealt {data.damage} damage.");
        }
        if (enemy == null)
        {
            Debug.Log("Attacking MP Player", target);
            playerStats = target.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(data.damage, transform.position, data.knockbackForce);
                Debug.Log("Player takes damage  "+data.damage);
            }
            else
            {
                Debug.Log("Player stats is null");
            }

        }

        var trgRb = target.GetComponent<Rigidbody2D>();
        // if (trgRb != null)
        //     trgRb.AddForce((target.transform.position - transform.position).normalized * data.knockbackForce, ForceMode2D.Impulse);
    }

    private void DestroyProjectile()
    {
        if (data.hitEffectPrefab) Instantiate(data.hitEffectPrefab, transform.position, transform.rotation);
        if (data.hitSound) AudioSource.PlayClipAtPoint(data.hitSound, transform.position);
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(data.projectileLifeTime);
        DestroyProjectile();
    }

    private void OnDrawGizmos()
    {
        if (!showTrajectory || !initialized) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(spawnPos, transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)GetCurrentVelocity() * 0.1f);
    }
}

// Movement Interfaces & Patterns (unchanged)
public interface IMovementPattern { void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir); void UpdateMovement(float dt); }
public class StraightMovementPattern : IMovementPattern
{
    private ProjectileSpellRuntime p; private ProjectileData d; private Vector2 dir; public void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir)
    {
        this.p = p; this.d = d; this.dir = dir; p.SetVelocity(dir * d.projectileSpeed); if (d.rotateToFaceDirection) p.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
    public void UpdateMovement(float dt) { }
}
public class ArcMovementPattern : IMovementPattern
{
    private ProjectileSpellRuntime p;
    private ProjectileData d;
    private Vector2 dir;

    public void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir)
    {
        this.p = p;
        this.d = d;
        this.dir = dir;

        var rb = p.GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = d.arcGravityScale;

        // Launch with horizontal speed and upward velocity
        p.SetVelocity(new Vector2(dir.x * d.projectileSpeed, d.projectileSpeed));
    }

    public void UpdateMovement(float dt) { }
}

public class HomingMovementPattern : IMovementPattern
{
    private ProjectileSpellRuntime p;
    private ProjectileData d;
    private bool homing = false;

    public void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir)
    {
        this.p = p;
        this.d = d;
        p.SetVelocity(dir * d.projectileSpeed);
        p.StartCoroutine(HomingDelay());
    }

    public void UpdateMovement(float dt)
    {
        if (!homing) return;
        GameObject t = GetClosestEnemy();
        if (!t) return;

        Vector2 to = (t.transform.position - p.transform.position).normalized;
        p.SetVelocity(to * d.projectileSpeed);

        if (d.rotateToFaceDirection)
            p.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(to.y, to.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    private IEnumerator HomingDelay()
    {
        yield return new WaitForSeconds(d.homingDelay);
        homing = true;
        while (p != null)
            yield return new WaitForSeconds(d.homingUpdateRate);
    }

    private GameObject GetClosestEnemy()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(p.transform.position, d.homingRadius, d.targetLayerMask);
        GameObject closest = null;
        float min = Mathf.Infinity;

        foreach (var h in hits)
        {
            if (h.transform == p.GetCaster() || h.transform.IsChildOf(p.GetCaster()))
                continue;

            float dist = Vector2.Distance(p.transform.position, h.transform.position);
            if (dist < min) { min = dist; closest = h.gameObject; }
        }
        return closest;
    }
}

public class ZigZagMovementPattern : IMovementPattern
{
    private ProjectileSpellRuntime p; private ProjectileData d; private Vector2 dir; private float t = 0f;

    public void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir)
    {
        this.p = p;
        this.d = d;
        this.dir = dir;
    }

    public void UpdateMovement(float dt)
    {
        t += dt;
        Vector2 offset = Vector2.Perpendicular(dir).normalized * Mathf.Sin(t * d.zigzagFrequency) * d.zigzagAmplitude;
        p.SetVelocity(dir * d.projectileSpeed + offset);
    }
}
public class RandomMovementPattern : IMovementPattern
{
    private ProjectileSpellRuntime p;
    private ProjectileData d;

    public void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir)
    {
        this.p = p;
        this.d = d;

        float off = d.randomDirectionOffset;
        Vector2 rnd = new Vector2(Random.Range(-off, off), Random.Range(-off, off));
        p.SetVelocity((dir + rnd).normalized * d.projectileSpeed);
    }

    public void UpdateMovement(float dt) { }
}

public class CircularMovementPattern : IMovementPattern
{
    private ProjectileSpellRuntime p;
    private ProjectileData d;
    private float angle = 0f, radius;
    private Vector3 center;

    public void Initialize(ProjectileSpellRuntime p, ProjectileData d, Vector2 dir)
    {
        this.p = p;
        this.d = d;
        radius = d.circularInitialRadius;
        center = p.GetCaster().position;
        p.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }

    public void UpdateMovement(float dt)
    {
        angle += d.circularSpeed * dt;
        radius += d.circularRadialSpeed * dt;
        p.transform.position = center + new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
    }
}
public interface IDamageable { void TakeDamage(int dmg); }

