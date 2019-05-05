using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    public GameObject ground;

    bool movePlayer = false;
    public float changeYPos;


    public ParticleSystem portle;

    private SpriteRenderer spriteR;
    public Sprite buttonPressed;

    public GameObject previousSequence;

    public GameObject textBox;
    public Text textDisplay;

    Animator myAnimator;

    void Start()
    {
        portle.Stop(); //stop the particle system from playing until button is pressed

        spriteR = gameObject.GetComponent<SpriteRenderer>();

        // get access to the animator of the player, i want to stop clays throwing anim here in this script
        myAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(movePlayer)
        {
			// pause player collisions and simulations while being moved
			player.GetComponent<Rigidbody2D>().simulated = false;
			player.GetComponent<RaycastPlayerCont>().setState(constants.ObjState.Paused);
            Vector3 pos;
            pos = player.transform.position;
            pos.y += changeYPos;
            player.transform.position = pos;
            myAnimator.SetBool("IsGravitating", true);
            myAnimator.SetBool("isIdle", false);

            if(pos.y >= 21.07f)
            {
                movePlayer = false;
				// no longer ignore the collision between the player and the ground!!!
				//Physics2D.IgnoreCollision(ground.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);

				// re-activate collisions and simulation
				player.GetComponent<Rigidbody2D>().simulated = true;
				player.GetComponent<RaycastPlayerCont>().setState(constants.ObjState.Active);
				// reset crystal and other variables in player
				player.GetComponent<RaycastPlayerCont>().takeCrystal();
				player.GetComponent<RaycastPlayerCont>().stop();

				Destroy(previousSequence);
                textBox.SetActive(false);
                textDisplay.text = " ";
            }
            
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "crystal") //probably makes more since for the player to hit the button with the crystal, rather than themselves
        {
            //move the  player up
            movePlayer = true;

            // ignore the collision between the player and the ground!!!
            //Physics2D.IgnoreCollision(ground.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), true);

            //change the sprite to be the button pressed
            spriteR.sprite = buttonPressed;

            //effect the particle system
            portle.Simulate(2);
            portle.Play();
        }
    }
        
}
