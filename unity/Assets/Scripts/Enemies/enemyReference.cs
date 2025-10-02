using UnityEngine;

public class enemyReference : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] BodEnemy enemy;

    public void PerformAttack()
    {
        enemy.PerformAttack();
    }

    public void AttackComplete()
    {
        enemy.AttackComplete();
    }
}
