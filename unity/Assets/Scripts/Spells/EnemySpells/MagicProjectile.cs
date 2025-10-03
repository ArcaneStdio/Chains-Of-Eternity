using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private float lifetime = 4f;
    public float knockbackforce =15f; // time after which it explodes if nothing is hit
    private Animator animator;
    private bool hasHit = false;
    public int damage = 20;
    private Rigidbody2D rb;

    private void Start()
    {   
        
        animator = GetComponent<Animator>();
        animator.SetBool("isHit", false);
        rb = GetComponent<Rigidbody2D>();
        // Safety: Explode if nothing is hit after `lifetime` seconds
        Invoke(nameof(Explode), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        //Debug.Log("Magic Projectile Hit: " + collision.gameObject.name);
        if (hasHit) return; // prevent multiple hits
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Player"))  // ignore enemies, projectiles and player
        {
            Debug.Log("Calling TakeDamage   " + collision.gameObject.name);
            collision.gameObject.GetComponent<PlayerStats>().TakeDamage(damage, transform.position,knockbackforce);
            hasHit = true;
        }
        
        animator.SetBool("isHit", true);
        rb.linearVelocity /=3f; // stop the projectile
        CancelInvoke(); // stop the timer since it already hit
    }

    private void Explode()
    {
        if (hasHit) return;
        hasHit = true;
        animator.SetBool("isHit", true);
    }
}
    