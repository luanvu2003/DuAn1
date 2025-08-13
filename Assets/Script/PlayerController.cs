using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 20f;
    public int maxHP = 200;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;
    private string currentScene;

    public Image healthBarImage;
    public TMP_Text scoreText;
    public TMP_Text coinText;

    [Header("Skill Casting")]
    public GameObject spellPrefab;    // Skill 1
    public GameObject spellPrefab2;   // Skill 2
    public GameObject spellPrefab3;   // Skill 3
    public float skillCooldown = 5f;

    private float lastSkillTime = -999f;
    private bool isCastingSkill = false;
    [Header("Skill UI")]
    public Image skill1Fill;
    public TMP_Text skill1CooldownText;

    public Image skill2Fill;
    public TMP_Text skill2CooldownText;

    public Image skill3Fill;
    public TMP_Text skill3CooldownText;

    private float skill1LastTime = -999f;
    private float skill2LastTime = -999f;
    private float skill3LastTime = -999f;
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

    public float attackCooldown = 0.3f;
    private float lastAttackTime = -Mathf.Infinity;
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;
    private bool isFacingRight = true;
    private Vector3 attackPointOffset;
    public static bool shouldResetUI = false;

    [Header("Knockback")]
    public float knockbackDuration = 0.2f;
    public float knockbackForce = 20f;
    private bool isKnockedBack = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        currentScene = SceneManager.GetActiveScene().name;

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
        if (shouldResetUI)
        {
            resetUI();
            shouldResetUI = false;
        }
        UpdateUI();
    }
    void Update()
    {
        if (isKnockedBack || isDead) return;

        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        Move();
        Jump();
        Climb();
        HandleAnimation();
        HandleSkills();

        if (Input.GetMouseButtonDown(0) && !isBlocking)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(10, this.transform);
        }
        UpdateSkillUI(skill1Fill, skill1CooldownText, skill1LastTime);
        UpdateSkillUI(skill2Fill, skill2CooldownText, skill2LastTime);
        UpdateSkillUI(skill3Fill, skill3CooldownText, skill3LastTime);

    }
    void HandleSkills()
    {
        // Mặc định: không dùng skill ở map1
        bool canUseSkill1 = false;
        bool canUseSkill2 = false;
        bool canUseSkill3 = false;

        switch (currentScene)
        {
            case "map2":
                canUseSkill1 = true;
                break;

            case "map3":
                canUseSkill1 = true;
                canUseSkill2 = true;
                break;

            case "mapboss":
                canUseSkill1 = true;
                canUseSkill2 = true;
                canUseSkill3 = true;
                break;
        }

        if (!isCastingSkill)
        {
            if (canUseSkill1 && Time.time >= skill1LastTime + skillCooldown && Input.GetKeyDown(KeyCode.T))
            {
                skill1LastTime = Time.time;
                StartCoroutine(CastSkill(1));
            }
            else if (canUseSkill2 && Time.time >= skill2LastTime + skillCooldown && Input.GetKeyDown(KeyCode.Y))
            {
                skill2LastTime = Time.time;
                StartCoroutine(CastSkill(2));
            }
            else if (canUseSkill3 && Time.time >= skill3LastTime + skillCooldown && Input.GetKeyDown(KeyCode.U))
            {
                skill3LastTime = Time.time;
                StartCoroutine(CastSkill(3));
            }
        }
    }


    void Move()
    {
        rb.velocity = new Vector2(inputHorizontal * moveSpeed, rb.velocity.y);

        if (inputHorizontal > 0 && !isFacingRight) Flip();
        else if (inputHorizontal < 0 && isFacingRight) Flip();
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

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOrigin + (Vector3)(attackDir * attackRange), attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            float damage = Random.Range(15f, 30f);
            float knockbackForce = 500f;
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;

            // ✅ Gây damage cho EnemyNormal
            if (enemy.TryGetComponent<EnemyNormal>(out var normal))
            {
                normal.TakeDamage(damage);
                normal.Knockback(knockbackDirection, knockbackForce);
                AddScore(50);
            }
            // ✅ Gây damage cho EnemyPatrol
            else if (enemy.TryGetComponent<EnemyPatrol>(out var patrol))
            {
                patrol.TakeDamage(damage);
                patrol.Knockback(knockbackDirection, knockbackForce);
                AddScore(50);
            }
            // ✅ Gây damage cho Enemy (base)
            else if (enemy.TryGetComponent<Enemy>(out var baseEnemy))
            {
                baseEnemy.TakeDamage(damage);
                baseEnemy.Knockback(knockbackDirection, knockbackForce);
                AddScore(50);
            }
            // ✅ Gây damage cho BossEnemy (không knockback)
            else if (enemy.TryGetComponent<BossEnemy>(out var boss))
            {
                boss.TakeDamage(damage);
                AddScore(100);
            }
        }
    }


    public void TakeDamage(int damage, Transform attacker)
    {
        if (isDead) return;
        if (isBlocking) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        Vector2 knockbackDirection = (transform.position - attacker.position).normalized;
        Knockback(knockbackDirection, knockbackForce);

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

        Invoke("LoadGameOverScene", 0.5f);
        resetUI();
    }

    private void Knockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(direction * force);
        StartCoroutine(StopKnockback());
    }

    private IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockedBack = false;
        rb.velocity = Vector2.zero;
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

    public int GetScore() => score;
    public int GetCoin() => coin;
    public int GetCurrentHP() => currentHP;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            AddCoin(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Trap"))
        {
            Die();
        }

        if (other.CompareTag("EnemyFirePoint"))
        {
            TakeDamage(2, this.transform);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    IEnumerator CastSkill(int skillNumber)
    {
        isCastingSkill = true;
        lastSkillTime = Time.time;
        rb.velocity = Vector2.zero;

        if (animator != null)
            animator.SetTrigger("Skill");

        // Delay để phù hợp với animation cast skill
        yield return new WaitForSeconds(0.7f);

        switch (skillNumber)
        {
            case 1:
                Skill1_SummonAtNearestEnemy();
                break;
            case 2:
                StartCoroutine(Skill2_RainSpells());
                break;
            case 3:
                Skill3_AOEDamage();
                break;
        }

        // Đợi thêm 0.5s để kết thúc casting
        yield return new WaitForSeconds(0.5f);
        isCastingSkill = false;
    }

    void Skill1_SummonAtNearestEnemy()
    {
        if (spellPrefab == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, 10f, enemyLayers);
        if (enemies.Length == 0) return;

        Transform nearestEnemy = enemies[0].transform;
        float minDistance = Vector2.Distance(transform.position, nearestEnemy.position);

        foreach (Collider2D enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearestEnemy = enemy.transform;
            }
        }

        Instantiate(spellPrefab, nearestEnemy.position, Quaternion.identity);
    }

    IEnumerator Skill2_RainSpells()
    {
        if (spellPrefab2 == null) yield break;

        int count = 5; // số lượng spell rơi xuống
        float radius = 5f; // bán kính rơi quanh player
        float height = 8f; // độ cao rơi

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * radius;
            spawnPos.y += height; // tăng y lên để spell rơi từ trên trời

            Instantiate(spellPrefab2, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void Skill3_AOEDamage()
    {
        if (spellPrefab3 == null) return;

        // Gọi spell gây sát thương diện rộng ngay quanh player
        Instantiate(spellPrefab3, transform.position, Quaternion.identity);

        // Ví dụ bạn có thể thêm logic sát thương ở đây hoặc bên trong prefab spellPrefab3
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

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

    public void resetUI()
    {
        currentHP = maxHP;
        score = 0;
        coin = 0;
        UpdateUI();
    }
    void UpdateSkillUI(Image fillImg, TMP_Text cdText, float lastTime)
    {
        if (fillImg == null || cdText == null) return;

        float remaining = (lastTime + skillCooldown) - Time.time;
        if (remaining > 0)
        {
            fillImg.fillAmount = remaining / skillCooldown;
            cdText.text = Mathf.CeilToInt(remaining).ToString();
        }
        else
        {
            fillImg.fillAmount = 0;
            cdText.text = "";
        }
    }
}
