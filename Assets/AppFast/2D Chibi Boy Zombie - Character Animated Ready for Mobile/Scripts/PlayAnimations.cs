using UnityEngine;
using System.Collections;

namespace NetDinamica.AppFast
{
    public class PlayAnimations : MonoBehaviour
    {

        // Use this for initialization
        Animator anim;
        bool facingRight = true;

        void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.Play("Idle");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                anim.Play("Walk");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                anim.Play("Attack");
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                anim.Play("Hurt");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                anim.Play("Die");
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                Flip();
            }
        }

        void Flip()
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;

        }
    }

}