using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 20f;
    public int maxHP = 100;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;

    public Image healthBarImage; // 🔄 Thay Slider bằng Image có fillAmount
    public TMP_Text scoreText;
    public TMP_Text coinText;

    private int currentHP;
    private int score = 0;
    private int coin = 0;

    private Rigidbody2D rb;
    private Animator animator;
    private BoxCollider2D boxCollider;

    private bool isClimbing = false;
    private bool isBlocking = false;
    private bool isDead = false;
    private float inputHorizontal;
    private float inputVertical;

    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    private bool isFacingRight = true;
    private Vector3 attackPointOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (PlayerPrefs.HasKey("player_hp"))
        {
            currentHP = PlayerPrefs.GetInt("player_hp");
            score = PlayerPrefs.GetInt("player_score");
            coin = PlayerPrefs.GetInt("player_coin");
        }
        else
        {
            currentHP = maxHP;
            score = 0;
            coin = 0;
        }
        attackPointOffset = attackPoint.localPosition;
        Debug.Log("Offset attack ban đầu: " + attackPoint.localPosition);
        UpdateUI();
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
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
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

        if (inputHorizontal > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (inputHorizontal < 0 && isFacingRight)
        {
            Flip();
        }
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
        if (isDead) return;

        animator.SetTrigger("Attack");

        Vector2 attackDir = isFacingRight ? Vector2.right : Vector2.left;
        Vector3 attackOrigin = attackPoint.position;

        // Vẽ debug để bạn nhìn thấy trong Scene
        Debug.DrawRay(attackOrigin, attackDir * attackRange, Color.red, 0.5f);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOrigin + (Vector3)(attackDir * attackRange), attackRange, enemyLayers);
        Debug.Log("Enemies hit: " + hitEnemies.Length);

        foreach (Collider2D enemy in hitEnemies)
        {
            float damage = Random.Range(15f, 30f);

            if (enemy.TryGetComponent<EnemyPatrol>(out var patrol))
            {
                patrol.TakeDamage(damage);
                AddScore(50);
            }
            else if (enemy.TryGetComponent<EnemyNormal>(out var normal))
            {
                normal.TakeDamage(damage);
                AddScore(50);
            }
            else if (enemy.TryGetComponent<Enemy>(out var baseEnemy))
            {
                baseEnemy.TakeDamage(damage);
                AddScore(50);
            }
            else if (enemy.TryGetComponent<Enemy>(out var bossEnemy))
            {
                bossEnemy.TakeDamage(damage);
                AddScore(100);
            }

            Debug.Log($"Hit {enemy.name} for {damage} damage.");
        }
    }


    void UseSkill()
    {
        //animator.SetTrigger("Skill");
    }

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

        //animator.SetTrigger("Hurt");
        Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHP);

        UpdateUI();
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

        // ✅ Chuyển sang scene thua sau 1.5 giây
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

    public int GetScore()
    {
        return score;
    }

    public int GetCoin()
    {
        return coin;
    }

    public int GetCurrentHP()
    {
        return currentHP;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            AddCoin(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Trap"))
        {
            Debug.Log("Player va vào bẫy!");
            Die();
        }
        if (other.CompareTag("EnemyFirePoint"))
        {
            TakeDamage(2);
        }

    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip nhân vật
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        // Flip cả attackPoint theo
        Vector3 attackScale = attackPoint.localScale;
        attackScale.x *= -1;
        attackPoint.localScale = attackScale;
    }
    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateUI();
    }
    public void SavePlayerProgress()
    {
        PlayerPrefs.SetInt("player_hp", currentHP);
        PlayerPrefs.SetInt("player_score", score);
        PlayerPrefs.SetInt("player_coin", coin);
        PlayerPrefs.Save();
    }

}