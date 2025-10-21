using System.Collections;
using UnityEngine;

public static class SpellFactory
{
    public static void CastSpell(SpellObject spell, Transform caster)
    {
        switch (spell.attackSubtype)
        {
            case AttackSubtype.Projectile:
                CreateProjectileSpell(spell.attackData.projectileData, spell.visualPrefab, caster);
                break;
            case AttackSubtype.AoE:
                CreateAoESpell(spell.attackData.aoeData, spell.visualPrefab, caster);
                break;
        }
        // else if (spell.category == SpellCategory.Buff)
        // {
        //     ApplyBuff(spell.buffData, caster.gameObject);
        // }
    }

    public static void CreateProjectileSpell(ProjectileData data, GameObject prefab, Transform caster)
    {
        if (data.delayBetweenProjectiles > 0f)
        {
            caster.GetComponent<MonoBehaviour>().StartCoroutine(SpawnProjectilesWithDelay(data, prefab, caster));
        }
        else
        {
            int N = data.numberOfProjectiles;
            Vector2 baseDir = GetMouseDirection(caster).normalized;

            for (int i = 0; i < N; i++)
            {
                // Apply direction override or fallback to mouse
                Vector2 direction = (i < data.directions.Count-1) ?
                    (baseDir + data.directions[i+1]).normalized :
                    baseDir;

                // Apply staggered launch angle
                float angleOffset = data.staggeredLaunchAngle * (i - (N - 1) / 2f);
                direction = Quaternion.Euler(0, 0, angleOffset) * direction;

                // Get spawn offset (safe access)
                Vector2 offset = (i < data.spawnOffsets.Count) ? data.spawnOffsets[i] : Vector2.zero;

                Debug.Log("Shooting one projectile");
                SpawnOneProjectile(data, prefab, caster, direction, offset);
            }
        }
    }

    private static IEnumerator SpawnProjectilesWithDelay(ProjectileData data, GameObject prefab, Transform caster)
    {
        int N = data.numberOfProjectiles;
        Vector2 baseDir = GetMouseDirection(caster).normalized;

        for (int i = 0; i < N; i++)
        {
            // Apply direction override or fallback to mouse
            Vector2 direction = (i < data.directions.Count-1) ?
                (baseDir + data.directions[i+1]).normalized :
                baseDir;

            // Apply staggered launch angle
            float angleOffset = data.staggeredLaunchAngle * (i - (N - 1) / 2f);
            direction = Quaternion.Euler(0, 0, angleOffset) * direction;

            // Get spawn offset (safe access)
            Vector2 offset = (i < data.spawnOffsets.Count) ? data.spawnOffsets[i] : Vector2.zero;

            SpawnOneProjectile(data, prefab, caster, direction, offset);

            yield return new WaitForSeconds(data.delayBetweenProjectiles);
        }
    }


    private static Vector2 GetMouseDirection(Transform caster)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        return ((Vector2)(mouseWorld - caster.position)).normalized;
    }

    private static void SpawnOneProjectile(ProjectileData data, GameObject prefab, Transform caster, Vector2 direction, Vector2 offset)
    {
        float zAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 spawnPos = caster.position + (Vector3)offset;
        GameObject projectile = GameObject.Instantiate(prefab, spawnPos, Quaternion.Euler(0, 0, zAngle));
        var runtime = projectile.AddComponent<ProjectileSpellRuntime>();
        projectile.transform.localScale *= data.projectileSize; // Set projectile size
        runtime.Initialize(data, caster, direction); // Updated ProjectileSpellRuntime now calculates mouse direction internally
    }

    private static void CreateAoESpell(AoEData data, GameObject prefab, Transform caster)
    {
        if (data == null || prefab == null)
        {
            Debug.LogError("AoEData or prefab is missing for AoE spell!");
            return;
        }

        if (caster == null)
        {
            Debug.LogError("Caster is missing for AoE spell!");
            return;
        }

        // Determine spawn position based on targeting mode
        Vector3 spawnPos = caster.position;
        Vector2 direction = Vector2.up; // Default direction

        switch (data.targetingMode)
        {
            case AoEData.TargetingMode.CasterPosition:
                spawnPos = caster.position;
                direction = caster.up; // Use caster's facing direction
                break;

            case AoEData.TargetingMode.MousePosition:
                // Get mouse position in world space
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                spawnPos = new Vector3(mousePos.x, mousePos.y, 0);
                // Calculate direction from caster to mouse
                direction = (spawnPos - caster.position).normalized;
                break;

            case AoEData.TargetingMode.Direction:
                // For directional spells, spawn at caster position
                spawnPos = caster.position;
                // Direction will be set based on caster's facing or mouse direction
                Vector3 mousePosDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                direction = (new Vector3(mousePosDir.x, mousePosDir.y, 0) - caster.position).normalized;
                break;
        }

        // Apply position offset
        spawnPos += (Vector3)data.positionOffset;
        spawnPos.z = 0; // Ensure 2D positioning

        // Determine rotation for directional spells
        Quaternion rotation = Quaternion.identity;
        if (data.IsCone() || data.IsLine() || data.targetingMode == AoEData.TargetingMode.Direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
        }

        // Spawn AoE object
        GameObject aoeObj = GameObject.Instantiate(prefab, spawnPos, rotation);

        // Add runtime behavior if it doesn't exist
        var runtime = aoeObj.GetComponent<AoESpellRuntime>();
        if (runtime == null)
        {
            runtime = aoeObj.AddComponent<AoESpellRuntime>();
        }

        // Initialize the AoE spell with proper data and direction
        runtime.Initialize(data, caster, direction);

        Debug.Log($"Spawned AoE spell: {data.spellName} at {spawnPos} with direction {direction}");
    }

    private static void CreateShortRangeSpell(ShortRangeData data, GameObject prefab, Transform caster)
    {
        // To be implemented
    }

    // private static void ApplyBuff(BuffData data, GameObject target)
    // {
    //     // To be implemented
    // }
}
