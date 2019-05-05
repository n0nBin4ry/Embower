using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CystalScript : MonoBehaviour
{
	protected Camera mainCam;

    Animator myAnimator;


    // crystal's velocity
    protected Vector2 my_vel = Vector2.zero;
	// bool to check if the cyrstal is stuck
	protected bool stuck = false;
	// time limit for how long the crystal sticks
	float stick_time_limit = 2f;
	float stick_time_cntr = 0f;
	// reference to any enemy that the crystal is stuck to
	protected GameObject enemy = null;
	// reference to player
	protected GameObject player;

	// reference to rigid body
	protected Rigidbody2D rb;

	float pixel_per_unit;

    AudioSource crystalStickSound;

    /*SCRATCH/REFERENCE*/
    Vector3 crystalPos;
	//bool changePos = false;
	//PlayerController player;

	// Start is called before the first frame update
	void Start()
	{
		//player = GameObject.Find("obj_player").GetComponent<PlayerController>();
		rb = this.gameObject.GetComponent<Rigidbody2D>();
		mainCam = Camera.main;

		pixel_per_unit = this.gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

        //get access to the animator of the player, i want to stop clays throwing anim here in this script
        myAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        crystalStickSound = gameObject.GetComponent<AudioSource>();

        myAnimator.SetBool("IsThrown", false);
    }

	// Update is called once per frame
	void Update()
	{
		// if crystal paused, don't update anything
		if (constants.g_crystal_paused)
			return;

		// apply forces and velocities (when not stuck)
		if (!stuck)
		{
			// apply gravity to "lob" crystal
			//my_vel.y -= constants.W_GRAVITY * Time.deltaTime;

			// apply velocity to position
			transform.position += new Vector3(my_vel.x, my_vel.y, 0) * Time.deltaTime;
			//rb.MovePosition(transform.position + new Vector3(my_vel.x, my_vel.y, 0) * Time.deltaTime);

			// return crystal to player if it goes off screen
			Vector2 viewPos = mainCam.WorldToViewportPoint(transform.position);
			if ((viewPos.x < 0) || (viewPos.x > 1) || (viewPos.y < 0) || (viewPos.y > 1))
			{
				Debug.Log("Crystal off screen");
				my_vel = Vector2.zero;
				stuck = true;
				stick_time_cntr = stick_time_limit + 1;
			}
		}

		if (stuck)
		{
			//this.gameObject.transform.position = crystalPos;

			// increase stuck counter
			stick_time_cntr += Time.deltaTime;
			// when time limit is ended gravtitate towards player
			if (stick_time_cntr > stick_time_limit)
			{
				transform.parent = null;
				Vector2 retVector = player.transform.position - transform.position;
				retVector.Normalize();
				my_vel = constants.RETURN_FORCE * retVector;
				//rb.MovePosition(transform.position + new Vector3(my_vel.x, my_vel.y, 0) * Time.deltaTime);
				transform.position += new Vector3(my_vel.x, my_vel.y, 0) * Time.deltaTime;
			}

            // apply velocity to position
            //transform.position += new Vector3(my_vel.x, my_vel.y, 0) * Time.deltaTime;


            //ANIMATION
            myAnimator.SetBool("IsThrowing", false);
            if(player.GetComponent<APlayerController>().isDead())
            {
                myAnimator.SetBool("IsThrown", false);
            }
            else
            {
                myAnimator.SetBool("IsThrown", true);
            }
        }

		// set tutorial bools
		// set true in collision check with "Unpurified" tag
		if (constants.crys_hit_unpurified && (!transform.parent || transform.parent.tag != "Purified" || transform.parent.tag != "PostPurified"))
			constants.crys_hit_unpurified = false;
	}

	// sets velocity of the crystal (player calls when launching)
	public void setVelocity(Vector2 vel)
	{
		my_vel = vel;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log("collided");
		// TODO **RICHARD** : do offset calculation to make the crystal not go into wall
		//this.gameObject.transform.position -= new Vector3(collision.collider.offset.x, collision.collider.offset.y);

		// send cystal back to player if the crystal hits hazard
		if (!stuck && collision.gameObject.tag == "Hazard")
		{
			Debug.Log("Hazard Hit by Crystal");
			my_vel = Vector2.zero;
			stuck = true;
			stick_time_cntr = stick_time_limit + 1;
		}

		// make crystal stick to wall
		if (!stuck && (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Floor" /*|| collision.gameObject.tag == "Purified" || collision.gameObject.tag == "PostPurified"*/))
		{
            crystalStickSound.Play();
			//this.gameObject.transform.position += new Vector3(collision.collider.offset.x, collision.collider.offset.y);
			Vector2 offset = constants.getCollisionoffset(this.gameObject.GetComponent<Collider2D>(), collision.collider);
			//this.transform.position += new Vector3(offset.x, offset.y);
			transform.position -= (new Vector3(offset.x, offset.y) / pixel_per_unit);

			// on a collision with wall, stop velocity so crystal "sticks"
			my_vel = Vector2.zero;

			//set crystal position to what it is upon collision so it "sticks" to the wall
			crystalPos = this.gameObject.transform.position;
			stuck = true;
		}


		//  ATSI:: Make Clay stick to objects that can be purified (enemies, crystals, etc)
		if (/*!stuck &&*/ (collision.gameObject.tag == "Unpurified" || collision.gameObject.tag == "Purified" || collision.gameObject.tag == "PostPurified"))
		{
            crystalStickSound.Play();
            //crystalPos = this.gameObject.transform.position;
            stuck = true;
			SetParent(collision.gameObject);
			collision.gameObject.tag = "Purified";
            constants.crys_hit_unpurified = true;
        }

		// set tutorial bool
		//if (collision.gameObject.tag == "Unpurified")
			//constants.crys_hit_unpurified = true;

		/*// send cystal back to player if the crystal hits hazard
		if (!stuck && collision.gameObject.tag == "Hazards") {
			my_vel = Vector2.zero;
			stuck = true;
			stick_time_cntr = stick_time_limit + 1;
		}*/
	}

	public void SetParent(GameObject newParent)
	{
		//Makes the GameObject "newParent" the parent of the GameObject "player".
		this.transform.parent = newParent.transform;
	}

	public void setPlayer(GameObject _player)
	{
		this.player = _player;
	}

	public bool isStuck() { return stuck; }

    //pls do not delete richard, i am using this in one of my tutorial scripts, thank
    public void setStuck(bool set)
    {
        stuck = set;
    }
}
