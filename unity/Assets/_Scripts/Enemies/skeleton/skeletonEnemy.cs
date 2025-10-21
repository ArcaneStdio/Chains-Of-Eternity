using UnityEngine;

public class skeletonEnemy : Enemy
{
    [SerializeField] BoxCollider2D topCollider;
    [SerializeField] BoxCollider2D bottomCollider;
    [SerializeField] BoxCollider2D rightCollider;
    public void attackOver()
    {
        topCollider.enabled = false;
        bottomCollider.enabled = false;
        rightCollider.enabled = false;
    }
    public override void PerformAttack()
    {
        
        Debug.Log("Slime Enemy Attacks with Slime Splash!");
        if (animator.GetBool("isAttacking"))
        {
            if (animator.GetBool("movingUp"))
            {
                topCollider.enabled = true;
                bottomCollider.enabled = false;
                rightCollider.enabled = false;
            }
            else if (animator.GetBool("movingDown"))
            {
                topCollider.enabled = false;
                bottomCollider.enabled = true;
                rightCollider.enabled = false;
            }
            else
            {
                topCollider.enabled = false;
                bottomCollider.enabled = false;
                rightCollider.enabled = true;
            }
        }else
        {
            topCollider.enabled = false;
            bottomCollider.enabled = false;
            rightCollider.enabled = false;
        }
        // Example: Instantiate a slime projectile or perform a splash attack
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Slime Enemy collided with Player!");
            // Example: Apply damage or effects to the player
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position,knockbackForce: knockbackForce , applyKnockback: true, applyStun: true);
                StateMachine.ChangeState(CooldownState);
            }
        }
    }
    // Additional logic updates specific to slime enemy can be added here
}
