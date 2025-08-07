using UnityEngine;
using System.Collections.Generic;
public class HealthPickup : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Heal(healAmount);
            gameObject.SetActive(false);
        }
    }
}
