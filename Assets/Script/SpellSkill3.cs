using UnityEngine;

public class SpellSkill3 : MonoBehaviour
{
    public float damage = 30f;
    public float aoeRadius = 3f;
    public float duration = 0.5f;

    void Start()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, aoeRadius, LayerMask.GetMask("Enemy"));

        foreach (var enemyCol in enemies)
        {
            bool dealtDamage = false;

            var enemy = enemyCol.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                dealtDamage = true;
            }

            var boss = enemyCol.GetComponent<BossEnemy>();
            if (boss != null && !dealtDamage)
            {
                boss.TakeDamage(damage);
                dealtDamage = true;
            }

            var enemyPatrol = enemyCol.GetComponent<EnemyPatrol>();
            if (enemyPatrol != null && !dealtDamage)
            {
                enemyPatrol.TakeDamage(damage);
                dealtDamage = true;
            }

            var enemyNormal = enemyCol.GetComponent<EnemyNormal>();
            if (enemyNormal != null && !dealtDamage)
            {
                enemyNormal.TakeDamage(damage);
                dealtDamage = true;
            }
        }

        Destroy(gameObject, duration);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
