using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOnWheels : MonoBehaviour
{
    public int driveWidth;
    public float driveSpeed;
    public float waitSpeed;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetInteger("driveWidth", driveWidth);
        animator.SetFloat("driveSpeed", driveSpeed);
        animator.SetFloat("waitSpeed", waitSpeed);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("Player"))
        {
            // push em back a bit? react somehow? idk
        }
        else if (collision.transform.CompareTag("Projectile"))
        {
            
            Destroy(this.gameObject, 2);
            //play destroy animation

        }
    }
}
