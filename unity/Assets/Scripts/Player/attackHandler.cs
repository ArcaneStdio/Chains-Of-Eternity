using System.Collections.Generic;
using UnityEngine;

public class attackHandler : MonoBehaviour
{
    [SerializeField] private BoxCollider2D attackHitbox; // Reference to the attack hitbox GameObject
    [SerializeField] private LayerMask enemyLayer; // Layer mask for enemies
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track hit enemies to avoid multiple hits
    [SerializeField] private HeroData heroData;
    private Enemy enemy;
    [SerializeField] bool dealsKnockback = true;
    [SerializeField] bool dealsStun = false;
    [SerializeField] bool flashenemy = true; // Flash the enemy when hit
    [SerializeField] string weaponType = "Physical";
    [SerializeField] private Player player;
    public BoxCollider2D upHitbox;
    public BoxCollider2D downHitbox;
    private PlayerStats playerStats;

    private void Awake()
    {
        attackHitbox = GetComponent<BoxCollider2D>();
        player = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.enabled && !upHitbox.enabled && !downHitbox.enabled) return;

        if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer)) &&
            !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);
            //Debug.Log($"Hit enemy: {other.gameObject.name}");

            enemy = other.GetComponentInParent<Enemy>();
            //Players knockbackForce is currently hardcoded to 7, should be changed later
            if (enemy != null)
            {
                enemy.TakeDamage(heroData.offensiveStats.damage, player.transform.position, player.knockbackForce, dealsKnockback, dealsStun, flashenemy, weaponType);
            }
            if (enemy == null)
            {
                Debug.Log("Attacking MP Player", other);
                playerStats = other.GetComponent<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.TakeDamage(player.Stats.heroData.offensiveStats.damage, transform.position, player.knockbackForce);
                    Debug.Log("Player takes damage  " + playerStats.heroData.offensiveStats.damage);
                }
                else
                {
                    Debug.Log("Player stats is null");
                }

                //other.GetComponent<EnemyHealth>()?.TakeDamage(heroData.offensiveStats.attackPower);
            }
        }
    }

    public void ClearHitEnemies()
    {
        hitEnemies.Clear();
    }
}
