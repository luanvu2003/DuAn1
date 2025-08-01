using UnityEngine;
using UnityEngine.UI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public Transform patrolPointA;
    public Transform patrolPointB;
    public float moveSpeed = 2f;
    public float idleDuration = 2f;

    [Header("Detection")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    [Header("Attack")]
    public float attackDamage = 10f;
    public float attackCooldown = 2f;

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

<<<<<<< Updated upstream
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
=======
    // ✅ NEW: Slider để hiển thị thanh máu
    public Slider healthBar;
>>>>>>> Stashed changes

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private Transform currentTarget;
    private float originalScaleX;
    private float idleTimer;
    private float attackTimer;
    private Vector3 initialPosition;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentTarget = patrolPointA;
        originalScaleX = transform.localScale.x;
        initialPosition = transform.position;

<<<<<<< Updated upstream
        // 🔧 Gắn sẵn Health Bar trong prefab (không Instantiate)
        Transform fill = transform.Find("EnemyHealthBar/Background/Fill");
        if (fill != null)
        {
            healthFillImage = fill.GetComponent<Image>();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy EnemyHealthBar/Background/Fill!");
=======
        // ✅ Khởi tạo thanh máu
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
>>>>>>> Stashed changes
        }
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        // Nếu bạn muốn giữ UI đúng vị trí (optional)
        if (healthFillImage != null)
        {
            Transform bar = healthFillImage.transform.parent.parent;
            bar.position = transform.position + healthBarOffset;
        }
    }

<<<<<<< Updated upstream
    // ========== Movement & Flip ==========

    void Patrol()
    {
        //animator.Play("Patrol");
        float direction = currentTarget.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * patrolSpeed, rb.velocity.y);
        FlipDirectionIfNeeded(direction);

        if (Mathf.Abs(direction) < 0.3f)
            currentTarget = currentTarget == patrolPointA ? patrolPointB : patrolPointA;
=======
    void Patrol()
    {
        animator.SetBool("isRunning", true);

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            idleTimer += Time.deltaTime;
            rb.velocity = Vector2.zero;
>>>>>>> Stashed changes

            if (idleTimer >= idleDuration)
            {
                idleTimer = 0f;
                currentTarget = (currentTarget == patrolPointA) ? patrolPointB : patrolPointA;
                Flip();
            }
        }
        else
        {
            Vector2 direction = (currentTarget.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
    }

    void ChasePlayer()
    {
<<<<<<< Updated upstream
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
=======
        animator.SetBool("isRunning", true);
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        if ((direction.x > 0 && transform.localScale.x < 0) || (direction.x < 0 && transform.localScale.x > 0))
        {
            Flip();
        }
    }

    void Attack()
>>>>>>> Stashed changes
    {
        attackTimer += Time.deltaTime;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);

        if (attackTimer >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            attackTimer = 0f;

            // Gây sát thương cho Player nếu trong vùng
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, attackRange, playerLayer);
            foreach (Collider2D hit in hitPlayers)
            {
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null && player.GetCurrentHP() > 0)
                {
                    player.TakeDamage((int)attackDamage);
                }
            }
        }
    }

<<<<<<< Updated upstream
    void Flip(bool faceRight)
=======
    void Flip()
>>>>>>> Stashed changes
    {
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }

<<<<<<< Updated upstream
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
=======
    public void TakeDamage(float amount)
>>>>>>> Stashed changes
    {
    animator.Play("Attack");
    float damage = Random.Range(damageRange.x, damageRange.y);
    Debug.Log($"Enemy attacked player for {damage} damage");

<<<<<<< Updated upstream
    if (player != null)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage((int)damage); // ✅ Gây sát thương vào PlayerController
        }
=======
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Enemy took {amount} damage. Current HP: {currentHealth}");

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator.Play("Hurt");
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        // ✅ Tắt thanh máu nếu muốn:
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        Destroy(gameObject, 1f); // Delay cho animation
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
>>>>>>> Stashed changes
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
            pc.TakeDamage((int)damage); // ✅ Gây sát thương vào PlayerController
        }
    }
}
}
