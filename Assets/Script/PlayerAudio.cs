using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource footstepSource; // dành cho bước chân (loop)
    public AudioSource effectSource;   // dành cho âm ngắn như chém

    public AudioClip footstepClip;
    public AudioClip swordSwingClip;
    public AudioClip swordHitClip;
    public AudioClip swordSkill1Clip;
    public AudioClip swordSkill2Clip;
    public AudioClip swordSkill3Clip;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Kiểm tra animation "Run"
        bool isRunning = animator.GetCurrentAnimatorStateInfo(0).IsName("run");

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
    }
    public void PlayAttackSound()
    {
        if (swordSwingClip != null && effectSource != null)
        {
            effectSource.PlayOneShot(swordSwingClip);
        }
    }
    public void PlayHitSound()
    {
        if (swordHitClip != null && effectSource != null)
        {
            effectSource.PlayOneShot(swordHitClip);
        }
    }
    public void PlaySkill1Sound()
    {
        if (swordSkill1Clip != null && effectSource != null)
        {
            effectSource.PlayOneShot(swordSkill1Clip);
        }
    }
    public void PlaySkill2Sound()
    {
        if (swordSkill2Clip != null && effectSource != null)
        {
            effectSource.PlayOneShot(swordSkill2Clip);
        }
    }
    public void PlaySkill3Sound()
    {
        if (swordSkill3Clip != null && effectSource != null)
        {
            effectSource.PlayOneShot(swordSkill3Clip);
        }
    }
}
