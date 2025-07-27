using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Transform patrolPointA; // Mốc trái
    public Transform patrolPointB; // Mốc phải
    private Transform currentTarget;
    private bool movingRight = true;

    [Header("Detection & Attack")]
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Vector2 damageRange = new Vector2(5, 15);
    private Transform player;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    // Lưu scale gốc để lật mặt đúng
    private float originalScaleX;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentTarget = patrolPointA; // Bắt đầu di chuyển sang phải

        originalScaleX = transform.localScale.x; // lưu scale ban đầu
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // ========================= Movement =========================

    void Patrol()
    {
        animator.Play("Patrol");

        float direction = currentTarget.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * patrolSpeed, rb.velocity.y);

        // Flip mặt nếu cần
        bool shouldFaceRight = direction < 0;
        if (shouldFaceRight != movingRight)
        {
            movingRight = shouldFaceRight;
            Flip(movingRight);
        }

        // Đổi mục tiêu khi gần điểm đến
        if (Mathf.Abs(direction) < 0.3f)
        {
            currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
        }
    }

    void ChasePlayer()
    {
        animator.Play("Chase");

        float direction = player.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.velocity.y);

        bool shouldFaceRight = direction > 0;
        if (shouldFaceRight != movingRight)
        {
            movingRight = shouldFaceRight;
            Flip(movingRight);
        }
    }

    void AttackPlayer()
    {
        animator.Play("Attack");

        float damage = Random.Range(damageRange.x, damageRange.y);
        Debug.Log($"Enemy attacked player for {damage} damage");

        // Gọi damage thực tế vào player nếu có health system
        // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    }

    void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = originalScaleX * (faceRight ? 1 : -1);
        transform.localScale = scale;
    }

    // ========================= Health System =========================

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"Enemy took {amount} damage. Current HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.Play("Hurt");
        }
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.Play("Die");
        Debug.Log("Enemy died.");
        Destroy(gameObject, 1.5f);
    }

    // ========================= Damage on Trigger =========================

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float damage = Random.Range(damageRange.x, damageRange.y);
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            Debug.Log($"Enemy triggered player for {damage} damage");
        }
    }
}
