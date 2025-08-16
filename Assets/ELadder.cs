using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELadder : MonoBehaviour
{
    public GameObject ladder; // Prefab của thang
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Hiện thang khi người chơi vào vùng trigger
            ladder.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Ẩn thang khi người chơi rời khỏi vùng trigger
            ladder.SetActive(false);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
