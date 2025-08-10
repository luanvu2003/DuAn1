using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        // Gây sát thương cho player
        PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage((int)damage, this.transform); // ✅ Gây sát thương vào PlayerController
            }

        Destroy(gameObject);
    }
    else if (collision.CompareTag("Ground")) // 🔹 Chạm đất thì huỷ
    {
        Destroy(gameObject);
    }
}

void OnCollisionEnter2D(Collision2D other)
{
    if (other.gameObject.CompareTag("Ground")) // 🔹 Chạm đất thì huỷ
    {
        Destroy(gameObject);
    }
}

}
