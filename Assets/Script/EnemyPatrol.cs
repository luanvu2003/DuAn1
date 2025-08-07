using UnityEngine;
using UnityEngine.UI;

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

    [Header("Health Bar (No Prefab)")]
    private Image healthFillImage;
    public Vector3 healthBarOffset = new Vector3(0, 1.2f, 0);

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
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentTarget = patrolPointA;
        originalScaleX = transform.localScale.x;
        initialPosition = transform.position;

        // 🔧 Gắn sẵn Health Bar trong prefab (không Instantiate)
        Transform fill = transform.Find("EnemyHealthBar/Background/Fill");
        if (fill != null)
        {
            healthFillImage = fill.GetComponent<Image>();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy EnemyHealthBar/Background/Fill!");
        }
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

        // Nếu bạn muốn giữ UI đúng vị trí (optional)
        if (healthFillImage != null)
        {
            Transform bar = healthFillImage.transform.parent.parent;
            bar.position = transform.position + healthBarOffset;
        }
    }

    // ========== Movement & Flip ==========

    void Patrol()
    {
        //animator.Play("Patrol");
        float direction = currentTarget.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * patrolSpeed, rb.velocity.y);
        FlipDirectionIfNeeded(direction);

        if (Mathf.Abs(direction) < 0.3f)
            currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;

        TryJumpIfObstacleAhead();
    }

    void ChasePlayer()
    {
        animator.SetBool("Chase", true);
        float direction = player.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.velocity.y);
        FlipDirectionIfNeeded(direction);
        TryJumpIfObstacleAhead();
    }

    void ReturnToStart()
    {
        animator.SetBool("Chase", false);
        //animator.Play("Return");
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

    void FlipDirectionIfNeeded(float direction)
    {
        bool shouldFaceRight = direction > 0;
        if (shouldFaceRight != movingRight)
        {
            movingRight = shouldFaceRight;
            Flip(movingRight);
        }
    }

    void Flip(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = originalScaleX * (faceRight ? 1 : -1);
        transform.localScale = scale;
    }

    void TryJumpIfObstacleAhead()
    {
        if (IsGrounded() && IsObstacleAhead())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.Play("Jump");
        }
    }

    // ========== Health ==========

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Enemy took {amount} damage. Current HP: {currentHealth}");

        if (healthFillImage != null)
        {
            float ratio = currentHealth / maxHealth;
            healthFillImage.fillAmount = ratio;
            Debug.Log($"Health bar updated to: {ratio}");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        //animator.Play("Die");
        Debug.Log("Enemy died.");

        Destroy(gameObject, 0.5f);
    }

    // ========== Checks ==========

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

    // ========== Attack ==========

    void AttackPlayer()
    {
        animator.Play("Attack");
        float damage = Random.Range(damageRange.x, damageRange.y);
        Debug.Log($"Enemy attacked player for {damage} damage");

        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage((int)damage); 
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float damage = Random.Range(damageRange.x, damageRange.y);
            Debug.Log($"Enemy triggered player for {damage} damage");

            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage((int)damage); 
            }
        }
    }
}