using UnityEngine;

public class DeathSpell : MonoBehaviour
{
    [SerializeField] BoxCollider2D spellHitbox;
    [SerializeField] int spellDamage = 25;
    

    public void ActivateSpell()
    {
        spellHitbox.enabled = true;
    }

    public void DeactivateSpell()
    {
        spellHitbox.enabled = false;
    }
    public void AnimationComplete()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats player = collision.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(spellDamage, transform.position, knockbackForce: 0f, applyKnockback: true, applyStun: true);
            }
        }
    }
}
