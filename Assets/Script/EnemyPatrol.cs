using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Transform patrolPointA;
    public Transform patrolPointB;
    private Transform currentTarget;
    private bool movingRight = true;
    private Vector3 initialPosition;
    private bool isReturning = false;

    [Header("Detection & Attack")]
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Vector2 damageRange = new Vector2(5, 15);
    private Transform player;
    private bool playerDetected = false;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("Jumping Over Obstacles")]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public Transform obstacleCheck;
    public float obstacleCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    private float originalScaleX;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentTarget = patrolPointA;
        originalScaleX = transform.localScale.x;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            playerDetected = true;
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
            ChasePlayer();
        }
        else
        {
            if (playerDetected)
            {
                playerDetected = false;
                isReturning = true;
            }

            if (isReturning)
            {
                ReturnToStart();
            }
            else
            {
                Patrol();
            }
        }
    }

    // ========================= Movement =========================

    void Patrol()
    {
        animator.Play("Patrol");

        float direction = currentTarget.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * patrolSpeed, rb.velocity.y);

        FlipDirectionIfNeeded(direction);

        if (Mathf.Abs(direction) < 0.3f)
        {
            currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
        }

        TryJumpIfObstacleAhead();
    }

    void ChasePlayer()
    {
        animator.Play("Chase");

        float direction = player.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.velocity.y);

        FlipDirectionIfNeeded(direction);

        TryJumpIfObstacleAhead();
    }

    void ReturnToStart()
    {
        animator.Play("Return");

        float direction = initialPosition.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * patrolSpeed, rb.velocity.y);

        FlipDirectionIfNeeded(direction);

        if (Mathf.Abs(direction) < 0.3f)
        {
            isReturning = false;
            currentTarget = patrolPointA;
        }

        TryJumpIfObstacleAhead();
    }

    void TryJumpIfObstacleAhead()
    {
        if (IsGrounded() && IsObstacleAhead())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.Play("Jump");
        }
    }

    void FlipDirectionIfNeeded(float direction)
    {
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

        // player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
    }

    void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = originalScaleX * (faceRight ? 1 : -1);
        transform.localScale = scale;
    }

    // ========================= Ground & Obstacle Checks =========================

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    bool IsObstacleAhead()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(obstacleCheck.position, direction, obstacleCheckDistance, groundLayer);
        return hit.collider != null;
    }

    // ========================= Health =========================

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float damage = Random.Range(damageRange.x, damageRange.y);
            Debug.Log($"Enemy triggered player for {damage} damage");
            // other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
