using UnityEngine;
using UnityEngine.UI;

public class EnemyNormal : MonoBehaviour
{
    [Header("Movement")]
    public float chaseSpeed = 3.5f;

    [Header("Detection & Attack")]
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Vector2 damageRange = new Vector2(5, 15);
    private Transform player;
    private bool playerDetected = false;
    private bool isReturning = false;
    private Vector3 initialPosition;
    

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
    private bool movingRight = true;
    public HealthItemPool healthItemPool;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        originalScaleX = transform.localScale.x;
        initialPosition = transform.position;

        Transform fill = transform.Find("EnemyHealthBar/Background/Fill");
        if (fill != null)
            healthFillImage = fill.GetComponent<Image>();
        else
            Debug.LogWarning("Không tìm thấy EnemyHealthBar/Background/Fill!");
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            playerDetected = true;
            isReturning = false;
            AttackPlayer();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
            isReturning = false;
            animator.SetBool("IsWalking", true);
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
            animator.SetBool("IsWalking", false);
            ReturnToStart();
        }

        // Cập nhật vị trí thanh máu
        if (healthFillImage != null)
        {
            Transform bar = healthFillImage.transform.parent.parent;
            bar.position = transform.position + healthBarOffset;
        }
    }

    void ChasePlayer()
    {
        float direction = player.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.velocity.y);
        FlipDirectionIfNeeded(direction);
        TryJumpIfObstacleAhead();
    }

    void ReturnToStart()
    {
        animator.SetTrigger("Return");
        float direction = initialPosition.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.velocity.y);
        FlipDirectionIfNeeded(direction);
        TryJumpIfObstacleAhead();

        if (Mathf.Abs(direction) < 0.3f)
        {
            rb.velocity = Vector2.zero;
            isReturning = false;
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
            //animator.Play("Jump");
        }
    }

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
        }

        if (currentHealth <= 0)
            Die();
<<<<<<< Updated upstream
        //else
        //animator.Play("Hurt");
=======
        else
            animator.SetTrigger("Hurt");
>>>>>>> Stashed changes
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
<<<<<<< Updated upstream
        //animator.Play("Die");
        if (healthItemPool != null)
        {
            healthItemPool.GetAvailableItem(transform.position);
        }
=======
        animator.SetBool("IsDead", true);
        animator.SetTrigger("Die");
>>>>>>> Stashed changes
        Destroy(gameObject, 0.5f);
    }

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

    void AttackPlayer()
    {
<<<<<<< Updated upstream
        animator.Play("Attack");
        float damage = Random.Range(damageRange.x, damageRange.y);
        Debug.Log($"Enemy attacked player for {damage} damage");
=======
    animator.SetBool("IsWalking", false); // Ngừng đi
    animator.SetTrigger("Attack");

    float damage = Random.Range(damageRange.x, damageRange.y);
    Debug.Log($"Enemy attacked player for {damage} damage");
>>>>>>> Stashed changes

        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.TakeDamage((int)damage); // ✅ Gây sát thương vào PlayerController
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
                pc.TakeDamage((int)damage); // ✅ Gây sát thương vào PlayerController
            }
        }
    }
}
