using UnityEngine;

public class slimeEnemy : Enemy
{      
    [Header("Slime Settings")]
    [Tooltip("Cooldown between collision damage (to prevent spamming)")]
    public float collisionDamageCooldown = 1.5f;
    
    private float lastDamageTime = -999f;

    public override void PerformAttack()
    {
        
        Debug.Log("Slime Enemy Attacks with Slime Splash!");
        // Example: Instantiate a slime projectile or perform a splash attack
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time - lastDamageTime < collisionDamageCooldown)
            {
                return;
            }

            Debug.Log("Slime Enemy collided with Player!");
            // Example: Apply damage or effects to the player
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position, knockbackForce: knockbackForce, applyKnockback: true, applyStun: true);
                lastDamageTime = Time.time;
                
                // Only change state if using State Machine (not Behavior Graph)
                if (!useBehaviorGraph && StateMachine != null)
                {
                    StateMachine.ChangeState(CooldownState);
                }
            }
        }
    }
    // Additional logic updates specific to slime enemy can be added here
}
