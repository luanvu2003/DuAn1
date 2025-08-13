using UnityEngine;

public class SpellSkill1 : MonoBehaviour
{
    public float damage = 40f;
    public float lifetime = 2f; // ⏳ Điều chỉnh thời gian tồn tại trong Inspector
    private bool hasDealtDamage = false;

    void Start()
    {
        // Hủy spell sau khi hết thời gian tồn tại
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasDealtDamage) return;

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

            if (dealt) hasDealtDamage = true;
        }
    }
}
