using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public int maxHP = 100;
    public LayerMask groundLayer;
    public LayerMask ladderLayer;

    private int currentHP;
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
    }

    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        Move();
        Jump();
        Climb();
        //HandleBlock();
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

    // void HandleBlock()
    // {
    //     if (Input.GetMouseButton(1))
    //     {
    //         isBlocking = true;
    //     }
    //     else
    //     {
    //         isBlocking = false;
    //     }
    // }

    void HandleAnimation()
    {
        animator.SetBool("isRunning", inputHorizontal != 0);
        animator.SetBool("isJumping", !IsGrounded());
        animator.SetBool("isClimbing", isClimbing);
        //animator.SetBool("isBlocking", isBlocking);
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
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
        animator.SetTrigger("Hurt");

        Debug.Log("Player took damage: " + damage + " | Current HP: " + currentHP);

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
}
