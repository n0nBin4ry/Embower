using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// component to make player run and jump
// TODO**RICHARD**: jump 

public class PlayerMovement : MonoBehaviour
{
    SpriteRenderer myRenderer;
    Animator myAnimator;
	//APlayerController myController;

    public static string direction;

    void Start()
    {
        myAnimator = gameObject.GetComponent<Animator>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();
		//myController = gameObject.GetComponent<APlayerController>();
	}

    // Update is called once per frame
    void Update()
    {
		// can't move if the player is launching
		//if (myController.isLaunching())
			//return;

        if (Input.GetKey(KeyCode.D))
        {
            myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isIdle", false);
            myRenderer.flipX = false;
            this.gameObject.transform.position += new Vector3(0.15f, 0, 0);
            direction = "right";
        }
        else if (Input.GetKey(KeyCode.A))
        {
            myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isIdle", false);
            myRenderer.flipX = true;
            this.gameObject.transform.position += new Vector3(-0.15f, 0, 0);
            direction = "left";
        }
        else
        {
            myAnimator.SetBool("isIdle", true);
            myAnimator.SetBool("isWalking", false);
        }
    }
}
