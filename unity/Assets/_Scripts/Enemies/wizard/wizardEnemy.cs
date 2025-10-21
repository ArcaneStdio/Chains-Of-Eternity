using Nethereum.Contracts.Standards.ENS.ENSRegistry.ContractDefinition;
using UnityEngine;

public class wizardEnemy : Enemy
{
    public GameObject magicProjectilePrefab;
    public Transform StartPoint;
    public float projectileSpeed = 5f; // Speed of the magic projectile
    public float magicAttackRange = 10f; // Range for magic attacks

    public override void PerformAttack()
    {
        performedAttack = true;
        //Debug.Log("Performing Magic Attack");
        if (magicProjectilePrefab == null)
        {
            Debug.LogError("Magic Projectile Prefab is not assigned!");
            return;
        }

        Vector2 directionToPlayer = (PlayerTransform.position - StartPoint.position).normalized;

        // Calculate rotation angle in degrees
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        // Instantiate and rotate projectile
        GameObject magicProjectile = Instantiate(
            magicProjectilePrefab,
            StartPoint.position,
            Quaternion.Euler(0, 0, angle) // Rotate on Z-axis
        );

        // Apply velocity
        Rigidbody2D rb = magicProjectile.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = directionToPlayer * projectileSpeed;

        //Debug.Log("Wizard Enemy Attacks with Magic!");
    }


}
