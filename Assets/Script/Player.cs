using UnityEngine;
using UnityEngine.UI; // dùng cho Slider và Text
using TMPro; // nếu dùng TextMeshPro

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int maxHP = 100;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;

    public Slider healthBar;           // Gán trong Inspector
    public TMP_Text scoreText;         // Gán trong Inspector
    public TMP_Text coinText;          // Gán trong Inspector

    private int currentHP;
    private int score = 0;
    private int coin = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private bool isClimbing = false;
    private bool isBlocking = false;
    private float ScalePlayer = 0.6125f;
    private float inputHorizontal;
    private float inputVertical;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentHP = maxHP;

        if (healthBar != null) healthBar.maxValue = maxHP;
        UpdateUI();
    }

    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        Move();
        Jump();
        Climb();
        HandleAnimation();

        if (Input.GetMouseButtonDown(0) && !isBlocking)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill();
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(inputHorizontal * moveSpeed, rb.velocity.y);
        if (inputHorizontal > 0)
            transform.localScale = new Vector3(ScalePlayer, ScalePlayer, ScalePlayer);
        if (inputHorizontal < 0)
            transform.localScale = new Vector3(ScalePlayer * (-1), ScalePlayer, ScalePlayer);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Climb()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1f, ladderLayer);
        if (hit.collider != null && Mathf.Abs(inputVertical) > 0)
        {
            isClimbing = true;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(rb.velocity.x, inputVertical * moveSpeed);
        }
        else if (isClimbing && hit.collider == null)
        {
            isClimbing = false;
            rb.gravityScale = 1f;
        }
    }

    void HandleAnimation()
    {
        bool grounded = IsGrounded();
        bool running = inputHorizontal != 0;

        // Ưu tiên leo thang
        animator.SetBool("isClimbing", isClimbing);

        if (!grounded)
        {
            // Ưu tiên animation nhảy
            animator.SetBool("isJumping", true);
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", running);
        }
    }

    void Attack()
{
    animator.SetTrigger("Attack");

    // Xác định hướng tấn công
    Vector2 attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    // Tạo raycast để kiểm tra kẻ địch
    RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDirection, 1.5f, LayerMask.GetMask("Enemy"));

        if (hit.collider != null)
        {
            // Kiểm tra xem có phải Enemy hay không
        EnemyPatrol enemyPatrol = hit.collider.GetComponent<EnemyPatrol>();
        if (enemyPatrol != null)
        {
            float damage = Random.Range(15f, 25f);
            enemyPatrol.TakeDamage(damage);
            AddScore(50);
            Debug.Log($"Hit enemy patrol for {damage} damage.");
        }



        EnemyNormal enemyNormal = hit.collider.GetComponent<EnemyNormal>();
        if (enemyNormal != null)
        {
            float damage = Random.Range(15f, 25f);
            enemyNormal.TakeDamage(damage);
            AddScore(50);
            Debug.Log($"Hit enemy normal for {damage} damage.");
        }

        }
    }


    void UseSkill()
    {
        animator.SetTrigger("Skill");
    }

    public void TakeDamage(int damage)
    {
        if (isBlocking)
        {
            Debug.Log("Blocked damage!");
            return;
        }

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        animator.SetTrigger("Hurt");
        Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHP);

        UpdateUI();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died.");
        // TODO: Gọi animation chết, disable điều khiển, reload scene,...
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

    void UpdateUI()
    {
        if (healthBar != null) healthBar.value = currentHP;
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (coinText != null) coinText.text = "Coins: " + coin;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void AddCoin(int amount)
    {
        coin += amount;
        UpdateUI();
    }
}
