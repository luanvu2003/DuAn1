using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float chaseSpeed = 2f;

    [Header("Detection & Attack")]
    public float detectionRange = 5f;
    public float attackRange = 2f;
    public float skillRange = 3f;
    public float attackCooldown = 2f;
    private float lastAttackTime = 0f;

    [Header("Damage")]
    public Vector2 damageRange = new Vector2(10, 20);

    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;
    private float initialScaleX;


    [Header("Health Bar")]
    private Image healthFillImage;
    public Vector3 healthBarOffset = new Vector3(0, 1.2f, 0);

    [Header("Skill Casting")]
    public GameObject spellPrefab;
    public Transform spellSpawnPoint;
    public float skillCooldown = 6f;
    private float lastSkillTime = -999f;

    [Header("Components")]
    private Animator animator;
    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        initialScaleX = transform.localScale.x;

        currentHealth = maxHealth;

        Transform fill = transform.Find("EnemyHealthBar/Background/Fill");
        if (fill != null)
            healthFillImage = fill.GetComponent<Image>();

    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        UpdateHealthBar();

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            Debug.Log("Ready to attack");

        }
        else if (distance <= skillRange && Time.time >= lastSkillTime + skillCooldown)
        {
            CastSpell();
        }
        else if (distance <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            StopMoving();
        }

        FacePlayer();
    }

    void ChasePlayer()
    {
        float direction = player.position.x - transform.position.x;
        rb.velocity = new Vector2(Mathf.Sign(direction) * chaseSpeed, rb.velocity.y);
        animator.SetBool("isWalking", true);
    }

    void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool("isWalking", false);
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        if (player.position.x > transform.position.x)
        scale.x = Mathf.Abs(initialScaleX); // Hướng phải
        else
        scale.x = -Mathf.Abs(initialScaleX); // Hướng trái

        transform.localScale = scale;
    }


    void Attack()
    {
        StopMoving();
        animator.SetTrigger("attack");
        lastAttackTime = Time.time;

        float damage = Random.Range(damageRange.x, damageRange.y);
        Debug.Log($"Boss attacks player for {damage}");

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
            pc.TakeDamage((int)damage);
    }

    void CastSpell()
    {
        StopMoving();
        animator.SetTrigger("cast");
        lastSkillTime = Time.time;
        Invoke(nameof(SpawnSpell), 0.7f); // delay to sync animation
    }

    void SpawnSpell()
    {
        if (spellPrefab != null && player != null)
        {
            Vector3 offset = new Vector3(0, 2f, 0); // spawn tren dau
            Instantiate(spellPrefab, player.position + offset, Quaternion.identity);
        }
    }


    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Boss took {amount} damage. Current HP: {currentHealth}");

        animator.SetTrigger("hurt");

        if (healthFillImage != null)
            healthFillImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("die");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    void UpdateHealthBar()
    {
        if (healthFillImage == null) return;

        Transform bar = healthFillImage.transform.parent.parent;
        bar.position = transform.position + healthBarOffset;
    }
}
