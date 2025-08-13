using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float jumpForce = 5f;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.Instance.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ScoreZone"))
        {
            GameManager.Instance.AddScore();
        }
    }
}
