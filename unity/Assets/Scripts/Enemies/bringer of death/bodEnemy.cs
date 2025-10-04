using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BodEnemy : Enemy
{
    [SerializeField] private BoxCollider2D attackHitbox;
    [SerializeField] private float closeRange = 1.5f;
    [SerializeField] GameObject projectilePrefab;

    private float farRange = 3.0f;
    private bool startTimer = false;
    bool justAttacked = false;

    private void Awake()
    {
        if(animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        farRange = attackRange;
        Debug.Log(farRange);
    } 
    private void Update()
    {
        base.Update();
        float distanceToPlayer = Vector2.Distance(PlayerTransform.position, transform.position);
        if (distanceToPlayer < farRange)
        {
            //Debug.Log("Distance to player: " + distanceToPlayer);
            if (distanceToPlayer < closeRange && !justAttacked)
            {
                // Perform close-range attack
                animator.SetBool("closeRangeAttack", true);
            }
            else if(!justAttacked)
            {
                // Perform far-range attack
                animator.SetBool("closeRangeAttack", false);
            }
        }
        if(justAttacked)
        {
            Debug.Log("----------------------Just attacked, starting cooldown timer--------------------");
            justAttacked = false;
            StateMachine.ChangeState(CooldownState);
        }
    }
    public override void PerformAttack()
    {
        startTimer = true;
        // using playerTransform to determine the distance to the player
        float distanceToPlayer = Vector2.Distance(PlayerTransform.position, transform.position);
        //Debug.Log("Distance to player: " + distanceToPlayer);
        Debug.Log("-------------------------Performing asrasrt------------------");


        if(distanceToPlayer < farRange)
        {
            Debug.Log("Distance to player: " + distanceToPlayer);
            if (distanceToPlayer < closeRange)
            {
                // Perform close-range attack
                CloseRangeAttack();
            }
            else
            {
                // Perform far-range attack
                Debug.Log("Performing far-range attack");
                FarRangeAttack();
            }
        }
    }
    public override IEnumerator FlashOnHit()
    {
        GameObject myself = this.gameObject;
        SpriteRenderer spriteRenderer1 = myself.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer1 == null) yield break; // No sprite renderer to flash

        Color originalColor = spriteRenderer1.color;
        spriteRenderer1.color = Color.red*2; // Flash color
        yield return new WaitForSeconds(0.2f);
        spriteRenderer1.color = Color.white; // Reset to original color
    }

    private void CloseRangeAttack()
    {
        // Logic for close-range attack
        animator.SetBool("closeRangeAttack", true);
        Debug.Log("Performing close-range attack");
        // Enabling the hitbox for close-range attack
        attackHitbox.enabled = true;
    }

    private void FarRangeAttack()
    {
        // Logic for far-range attack
        animator.SetBool("closeRangeAttack", false);
        Debug.Log("Performing far-range attack");
        // instantiate the gameobject at 8.45 y above player's position
        Debug.Log("Instantiating projectile at player's position with an offset");
        Vector3 spawnPos = PlayerTransform.position + new Vector3(0f, 2f, 0f);

        // Instantiate projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);


        // Example: Instantiate a projectile or perform a ranged attack

    }

    public void AttackComplete()
    {
        Debug.Log("Attack complete event caught and Attack animation completed.");
        attackHitbox.enabled = false;
        justAttacked = true;
        animator.SetBool("closeRangeAttack", false);
        performedAttack = true;
        // Disable the hitbox after the attack is complete
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collider is the player
        if (collision.CompareTag("Player"))
        {
            // Logic for when the enemy collides with the player
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage, transform.position, knockbackForce: knockbackForce, applyKnockback: true, applyStun: true);
            }
            // Example: Deal damage to the player or trigger an effect
        }
    }
    // Additional logic updates specific to slime enemy can be added here
}
