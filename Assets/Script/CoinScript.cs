using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Phát âm thanh tại vị trí đồng xu
            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject); // Xoá đồng xu
        }
    }
}