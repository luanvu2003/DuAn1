using UnityEngine;

public class SpellSkill2 : MonoBehaviour
{
    public float damage = 20f;
    public float fallSpeed = 5f;
    public float lifetime = 5f;

    private bool hasLanded = false;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!hasLanded)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasLanded) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") ||
            other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                bool dealt = false;

                if (other.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.TakeDamage(damage);
                    dealt = true;
                }

                if (other.TryGetComponent<EnemyNormal>(out var normal))
                {
                    normal.TakeDamage(damage);
                    dealt = true;
                }

                if (other.TryGetComponent<EnemyPatrol>(out var patrol))
                {
                    patrol.TakeDamage(damage);
                    dealt = true;
                }

                if (other.TryGetComponent<BossEnemy>(out var boss))
                {
                    boss.TakeDamage(damage);
                    dealt = true;
                }

                if (dealt)
                {
                    // knockbackDirection: hướng từ skill tới enemy
                    Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                    float knockbackForce = 300f;

                    if (enemy != null) enemy.Knockback(knockbackDir, knockbackForce);

                }
            }

            hasLanded = true;

            // Hủy object sau 0.3s để giữ lại hiệu ứng nếu có
            Destroy(gameObject, 0.3f);
        }
    }
}
