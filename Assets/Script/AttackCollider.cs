using UnityEngine;
using System.Collections.Generic;

public class AttackCollider : MonoBehaviour
{
    public PlayerController playerController;
    private HashSet<Collider2D> hitEnemies = new HashSet<Collider2D>();

    private void OnEnable()
    {
        // Mỗi lần bật collider, reset danh sách đã đánh
        hitEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hitEnemies.Contains(other)) return;

        float damage = Random.Range(15f, 30f);

        if (other.TryGetComponent<EnemyPatrol>(out var enemyPatrol))
        {
            enemyPatrol.TakeDamage(damage);
            playerController.AddScore(50);
            Debug.Log($"Hit enemy patrol for {damage} damage.");
        }
        else if (other.TryGetComponent<EnemyNormal>(out var enemyNormal))
        {
            enemyNormal.TakeDamage(damage);
            playerController.AddScore(50);
            Debug.Log($"Hit enemy normal for {damage} damage.");
        }
        else if (other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(damage);
            playerController.AddScore(50);
            Debug.Log($"Hit enemy for {damage} damage.");
        }

        hitEnemies.Add(other);
    }
}
