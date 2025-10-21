using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public Enemy enemy; 
    private Camera cam;

    public Vector3 offset = new Vector3(0, 1f, 0); // adjust height

    void Start()
    {
        cam = Camera.main;
    }

    public void SetTarget(Enemy targetEnemy)
    {
        enemy = targetEnemy;
        slider.maxValue = 1;
        slider.value = enemy.currentHealth/enemy.maxHealth;
    }

    void Update()
    {
        if (enemy == null)
        {   
            Debug.Log("Enemy is null, destroying health bar");
            Destroy(gameObject);
            return;
        }

        // Keep value updated
        slider.value = ((float)enemy.currentHealth)/enemy.maxHealth;

        // Make bar follow enemy
        Vector3 worldPos = enemy.transform.position + offset;
        transform.position = cam.WorldToScreenPoint(worldPos);
    }
}
