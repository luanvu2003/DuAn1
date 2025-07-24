using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource footstepSource; // dành cho bước chân (loop)
    public AudioSource effectSource;   // dành cho âm ngắn như chém

    public AudioClip footstepClip;
    public AudioClip swordSwingClip;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Kiểm tra animation "Run"
        bool isRunning = animator.GetCurrentAnimatorStateInfo(0).IsName("Run");

        // PHÁT hoặc DỪNG tiếng bước chân
        if (isRunning)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = footstepClip;
                footstepSource.loop = true;
                footstepSource.Play();
            }
        }
        else
        {
            footstepSource.Stop();
        }

        // PHÁT tiếng chém khi click chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            effectSource.PlayOneShot(swordSwingClip);
        }
    }
}
