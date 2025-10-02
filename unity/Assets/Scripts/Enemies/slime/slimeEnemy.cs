using UnityEngine;

public class slimeEnemy : Enemy
{      


    public override void PerformAttack()
    {
        
        Debug.Log("Slime Enemy Attacks with Slime Splash!");
        // Example: Instantiate a slime projectile or perform a splash attack
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Slime Enemy collided with Player!");
            // Example: Apply damage or effects to the player
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position,knockbackForce: knockbackForce , applyKnockback: true, applyStun: true, damageType: "Slime");
                StateMachine.ChangeState(CooldownState);
            }
        }
    }
    // Additional logic updates specific to slime enemy can be added here
}
