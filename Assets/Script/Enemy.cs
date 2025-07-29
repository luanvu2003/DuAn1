using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float chaseSpeed = 3.5f;

    [Header("Detection & Attack")]
    public float detectionRange = 5f;
    public Vector2 damageRange = new Vector2(5, 15); // không dùng nữa nếu dùng projectile
    private Transform player;
    private bool playerDetected = false;
    private bool isReturning = false;
    private Vector3 initialPosition;

    [Header("Ranged Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float skillCooldown = 2f;
    public float projectileDamage = 10f;
    private float lastSkillTime = -Mathf.Infinity;

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

    if (distanceToPlayer <= detectionRange)
    {
        playerDetected = true;
        isReturning = false;

        // luôn nhìn về hướng player
        float direction = player.position.x - transform.position.x;
        FlipDirectionIfNeeded(direction);

        UseRangedSkill();
    }
    else
    {
        if (playerDetected)
        {
            playerDetected = false;
            isReturning = true;
        }

        if (isReturning)
            ReturnToStart();
    }

    // Cập nhật vị trí thanh máu
    if (healthFillImage != null)
    {
        Transform bar = healthFillImage.transform.parent.parent;
        bar.position = transform.position + healthBarOffset;
    }
}


    void UseRangedSkill()
    {
        float timeSinceLastSkill = Time.time - lastSkillTime;
        if (timeSinceLastSkill >= skillCooldown)
        {
            lastSkillTime = Time.time;

            // Tạo projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            EnemyProjectile proj = projectile.GetComponent<EnemyProjectile>();
            if (proj != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                proj.SetDirection(direction);
                proj.damage = projectileDamage;
            }

            animator?.SetTrigger("UseSkill");
            Debug.Log("Enemy used ranged skill!");
        }
    }

    void ReturnToStart()
    {
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
    if (Mathf.Abs(direction) < 0.05f) return;

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
        //else
            //animator.Play("Hurt");
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        //animator.Play("Die");
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
}
