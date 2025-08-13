// BossEnemy.cs - Fixed version: boss doesn't shift when turning, zones follow correctly
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BossEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float chaseSpeed = 2f;

    [Header("Detection & Attack")]
    public float detectionRange = 5f;
    public Transform attackZone;
    public Transform skillZone;
    public float attackZoneRadius = 2f;
    public float skillZoneRadius = 3f;
    public LayerMask playerLayer;
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
    private enum BossState { Idle, Chasing, Attacking, Casting }
    private BossState currentState = BossState.Idle;
    private SpriteRenderer spriteRenderer;
    private Vector3 attackZoneLocalPos;
    private Vector3 skillZoneLocalPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        initialScaleX = transform.localScale.x;

        currentHealth = maxHealth;

        Transform fill = transform.Find("EnemyHealthBar/Background/Fill");
        if (fill != null)
            healthFillImage = fill.GetComponent<Image>();

        if (attackZone != null)
            attackZoneLocalPos = attackZone.localPosition;
        if (skillZone != null)
            skillZoneLocalPos = skillZone.localPosition;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        UpdateHealthBar();

        if (currentState == BossState.Attacking || currentState == BossState.Casting)
            return;

        bool isPlayerInAttackZone = Physics2D.OverlapCircle(attackZone.position, attackZoneRadius, playerLayer);
        bool isPlayerInSkillZone = Physics2D.OverlapCircle(skillZone.position, skillZoneRadius, playerLayer);

        if (isPlayerInAttackZone && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else if (isPlayerInSkillZone && Time.time >= lastSkillTime + skillCooldown)
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
            currentState = BossState.Idle;
        }
    }

    void ChasePlayer()
    {
        float dir = player.position.x - transform.position.x;
        float moveDir = Mathf.Sign(dir);

        // Turn boss by scaling, not flipX
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(initialScaleX) * moveDir;
        transform.localScale = scale;
        UpdateZoneDirection();
        rb.velocity = new Vector2(moveDir * chaseSpeed, rb.velocity.y);
        animator.SetBool("isWalking", true);
    }

    void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        animator.SetBool("isWalking", false);
    }

    void Attack()
    {
        if (currentState == BossState.Attacking || isDead || player == null) return;

        currentState = BossState.Attacking;
        rb.velocity = Vector2.zero;
        rb.Sleep();

        animator.SetTrigger("attack");
        lastAttackTime = Time.time;

        float damage = Random.Range(damageRange.x, damageRange.y);
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            float dist = Vector2.Distance(attackZone.position, player.position);
            if (dist <= attackZoneRadius + 0.3f)
            {
                pc.TakeDamage((int)damage, this.transform);
            }
        }

        Invoke(nameof(ResetToIdle), 1f);
    }

    void CastSpell()
    {
        if (currentState == BossState.Casting || isDead || player == null) return;

        currentState = BossState.Casting;
        StopMoving();

        animator.SetTrigger("cast");
        lastSkillTime = Time.time;

        Invoke(nameof(SpawnSpell), 0.7f);
        Invoke(nameof(ResetToIdle), 1.2f);
    }

    void SpawnSpell()
    {
        if (spellPrefab == null || player == null) return;
        Vector3 offset = new Vector3(0, 2f, 0);
        Instantiate(spellPrefab, player.position + offset, Quaternion.identity);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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
        SceneManager.LoadScene("Win");
    }

    void UpdateHealthBar()
    {
        if (healthFillImage == null) return;

        Transform bar = healthFillImage.transform.parent.parent;
        bar.position = transform.position + healthBarOffset;
    }

    void ResetToIdle()
    {
        currentState = BossState.Idle;
    }

    void OnDrawGizmos()
    {
        if (attackZone != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackZone.position, attackZoneRadius);
        }
        if (skillZone != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(skillZone.position, skillZoneRadius);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    void UpdateZoneDirection()
    {
        int facingDir = (int)Mathf.Sign(transform.localScale.x);

        if (attackZone != null)
        {
            attackZone.localPosition = new Vector3(
                Mathf.Abs(attackZoneLocalPos.x) * facingDir,
                attackZoneLocalPos.y,
                attackZoneLocalPos.z
            );
        }

        if (skillZone != null)
        {
            skillZone.localPosition = new Vector3(
                Mathf.Abs(skillZoneLocalPos.x) * facingDir,
                skillZoneLocalPos.y,
                skillZoneLocalPos.z
            );
        }
    }

}