using UnityEngine;

public class SpellEffect : MonoBehaviour
{
    public int damage = 25;
    public float duration = 2f;
    public LayerMask playerLayer;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & playerLayer) != 0)
        {
            PlayerController pc = col.GetComponent<PlayerController>();
            if (pc != null)
                pc.TakeDamage(damage);
        }
    }
}
