using UnityEngine;
using UnityEngine.UI;
using TMPro;
<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
using UnityEngine.SceneManagement;
=======
>>>>>>> Stashed changes:Assets/Script/Player.cs

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int maxHP = 100;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;

<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
    public Image healthBarImage; // 🔄 Thay Slider bằng Image có fillAmount
=======
    [Header("UI")]
    public Image healthBarFill; // ✅ Dùng Image thay vì Slider
>>>>>>> Stashed changes:Assets/Script/Player.cs
    public TMP_Text scoreText;
    public TMP_Text coinText;

    private int currentHP;
    private int score = 0;
    private int coins = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private bool isClimbing = false;
    private bool isBlocking = false;
    private bool isDead = false;

    private float ScalePlayer = 0.6125f;
    private float inputHorizontal;
    private float inputVertical;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentHP = maxHP;

<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
        UpdateUI();
=======
        UpdateHealthBar();
        UpdateScoreText();
        UpdateCoinText();
>>>>>>> Stashed changes:Assets/Script/Player.cs
    }

    void Update()
    {
        if (isDead) return;

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

        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(10);
        }
    }

    void Move()
    {
        rb.velocity = new Vector2(inputHorizontal * moveSpeed, rb.velocity.y);
        if (inputHorizontal > 0)
            transform.localScale = new Vector3(ScalePlayer, ScalePlayer, ScalePlayer);
        if (inputHorizontal < 0)
            transform.localScale = new Vector3(-ScalePlayer, ScalePlayer, ScalePlayer);
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
<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
        bool grounded = IsGrounded();
        bool running = inputHorizontal != 0;

        animator.SetBool("isClimbing", isClimbing);

        if (!grounded)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", running);
        }
=======
        animator.SetBool("isRunning", inputHorizontal != 0);
        animator.SetBool("isJumping", !IsGrounded());
        animator.SetBool("isClimbing", isClimbing);
>>>>>>> Stashed changes:Assets/Script/Player.cs
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
<<<<<<< Updated upstream:Assets/Script/PlayerController.cs

        Vector2 attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDirection, 1.5f, LayerMask.GetMask("Enemy"));

        if (hit.collider != null)
        {
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

            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                float damage = Random.Range(15f, 25f);
                enemy.TakeDamage(damage);
                AddScore(50);
                Debug.Log($"Hit enemy for {damage} damage.");
            }
        }


    }

=======
    }
>>>>>>> Stashed changes:Assets/Script/Player.cs

    void UseSkill()
    {
        //animator.SetTrigger("Skill");
    }

    public void TakeDamage(int damage)
    {
<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
        if (isDead) return;

        if (isBlocking)
        {
            Debug.Log("Blocked damage!");
            return;
        }

        currentHP -= damage;
=======
        if (isBlocking || isDead) return;
currentHP -= damage;
>>>>>>> Stashed changes:Assets/Script/Player.cs
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        //animator.SetTrigger("Hurt");
        Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHP);

<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
        UpdateUI();
=======
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
        }
>>>>>>> Stashed changes:Assets/Script/Player.cs
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHP / maxHP;
        }
    }

    void Die()
    {
<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
    isDead = true;
    animator.SetTrigger("Die");
    rb.velocity = Vector2.zero;
    rb.bodyType = RigidbodyType2D.Static;

    if (healthBarImage != null)
        healthBarImage.transform.parent.gameObject.SetActive(false);

    Debug.Log("Player died.");

    // ✅ Chuyển sang scene thua sau 1.5 giây
    Invoke("LoadGameOverScene", 0.5f);
}

void LoadGameOverScene()
{
    SceneManager.LoadScene("Lose");
}

=======
        isDead = true;
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        if (healthBarFill != null)
            healthBarFill.transform.parent.gameObject.SetActive(false); // Ẩn toàn bộ thanh máu nếu có

        Debug.Log("Player died.");
    }
>>>>>>> Stashed changes:Assets/Script/Player.cs

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            0.1f,
            groundLayer
        );
        return hit.collider != null;
    }

<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
    void UpdateUI()
    {
        if (healthBarImage != null)
        {
            float percent = (float)currentHP / maxHP;
            healthBarImage.fillAmount = percent;

            if (currentHP <= 0)
            {
                Die();
            }
        }

        if (scoreText != null) scoreText.text = "Score: " + score;
        if (coinText != null) coinText.text = "Coins: " + coin;
    }

=======
>>>>>>> Stashed changes:Assets/Script/Player.cs
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinText();
    }

<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
=======
    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;
    }

>>>>>>> Stashed changes:Assets/Script/Player.cs
    public int GetCurrentHP()
    {
        return currentHP;
    }
<<<<<<< Updated upstream:Assets/Script/PlayerController.cs
    void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Coin"))
    {
        AddCoin(1);
        Destroy(collision.gameObject);
    }

    if (collision.CompareTag("Trap"))
    {
        Debug.Log("Player va vào bẫy!");
        Die();
    }
}
}
=======
}
>>>>>>> Stashed changes:Assets/Script/Player.cs
