using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int maxHP = 100;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;
    public Image healthBarImage; // 🔄 Image dạng Fill
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
        UpdateHealthBar();
        UpdateScoreText();
        UpdateCoinText();
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
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Vector2 attackDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, attackDirection, 1.5f, LayerMask.GetMask("Enemy"));

        if (hit.collider != null)
        {
            float damage = Random.Range(15f, 25f);
            if (hit.collider.TryGetComponent(out EnemyPatrol ep))
            {
                ep.TakeDamage(damage);
                AddScore(50);
            }
            else if (hit.collider.TryGetComponent(out EnemyNormal en))
            {
                en.TakeDamage(damage);
                AddScore(50);
            }
            else if (hit.collider.TryGetComponent(out Enemy e))
            {
                e.TakeDamage(damage);
                AddScore(50);
            }
        }
    }

    void UseSkill() {}

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (isBlocking)
        {
            Debug.Log("Blocked damage!");
            return;
        }

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHP);
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBarImage != null)
        {
            float percent = (float)currentHP / maxHP;
            healthBarImage.fillAmount = percent;
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        if (healthBarImage != null)
            healthBarImage.transform.parent.gameObject.SetActive(false);

        Debug.Log("Player died.");
        Invoke("LoadGameOverScene", 0.5f);
    }

    void LoadGameOverScene()
    {
        SceneManager.LoadScene("Lose");
    }

    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }

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

    public int GetCurrentHP()
    {
        return currentHP;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            AddCoins(1);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Trap"))
        {
            Debug.Log("Player va vào bẫy!");
            Die();
        }
    }
}
