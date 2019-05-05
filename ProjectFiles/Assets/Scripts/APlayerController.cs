using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APlayerController : MonoBehaviour
{
    // VARIABLES

    // pointer to crystal prefab
    public GameObject crystal;
	protected GameObject crystal_inst = null;

    // animation components
    SpriteRenderer myRenderer;
    Animator myAnimator;

    // accelerations
    protected Vector2 my_accl = Vector2.zero;
    // velocities
    protected Vector2 my_vel = Vector2.zero;

    // the crystal has been thrown
    protected bool launched = false;
    // the player is preparing to launch; player will not be able to move when launching
    protected bool launching = false;
    // bool to tell us if player is stuck
    protected bool stuck = false;
    // bool to tell us if player is in air
    protected bool in_air = true;
    // bool set true for when gravitating
    protected bool gravitating = false;
    // bool to check if dead
    protected bool dead = false;
	// to tell us if player has jumped yet
	protected bool jumped = false;

	// pixels per unit of current sprite
	float pixel_per_unit;

    // time limit for how long a player sticks
    float stick_time_limit = 2f;
    float stick_time_cntr = 0f;

    // the initial position of the launch vector
    protected Vector2 launch_0;
    // the final position of the launch vector
    protected Vector2 launch_1;

    // the offset of crystal (compared to player) when launched
    protected float launch_offset = 1; // arbitrary for now TODO **RICHARD** : make the offset more dynamic

    //atsis not so great code, LOL
    AudioSource clayDead;
    bool checkAgain = false;
    

    // METHODS
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = gameObject.GetComponent<Animator>();
        myRenderer = gameObject.GetComponent<SpriteRenderer>();

		pixel_per_unit = myRenderer.sprite.pixelsPerUnit;

        clayDead = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

		// if player is paused, return so nothing changes
		if (!constants.g_player_paused)
        {
            // Input-Phase
            processInput();

            // Move-Phase: Accel/Force -> Velocity -> Position

            //my_vel += accel * Time.deltaTime;
            // apply gravity when in air and not stuck
            if (!stuck && in_air && !gravitating)
            {
                my_vel.y -= constants.W_GRAVITY * Time.deltaTime;
                //ANIMATION
                myAnimator.SetBool("IsHugging", false);
                myAnimator.SetBool("IsSticking", false);
            }

            // decellerate horizontal velocity MOVED TO INPUT WHEN IDLE
            /*if (my_vel.x > 0)
                my_vel.x = Mathf.Clamp(my_vel.x - (constants.PLAYER_DCELL * Time.deltaTime), 0, my_vel.x);
            else if (my_vel.x < 0)
                my_vel.x = Mathf.Clamp(my_vel.x + (constants.PLAYER_DCELL * Time.deltaTime), my_vel.x, 0);*/

            // apply velocity to position
            this.gameObject.transform.position += new Vector3(my_vel.x, my_vel.y) * Time.deltaTime;

            // incriment stick counter if stuck
            if (stuck)
            {
                stick_time_cntr += Time.deltaTime;
                if ((!transform.parent || !(transform.parent.tag != "Purified" || transform.parent.tag != "PostPurified")) // don't unstick if enemy 
					&& (stick_time_cntr > stick_time_limit))
                {
                    stuck = false;
                    stick_time_cntr = 0;
                    // unparent from any purified; temp needed to not destroy player if parent destroyed
                    Transform temp = transform.parent;
                    transform.parent = null;
                    // if attached to a purified object, set ot Post-puriofied
                    if (temp && temp.tag == "Purified")
                    {
                        temp.tag = "PostPurified";
                    }
                }

                // set tutorial bools
                if (transform.parent && (transform.parent.tag == "Purified" || transform.parent.tag == "PostPurified"))
                    constants.plyr_hit_purified = true;
                else
                    constants.plyr_hit_purified = false;
                if (!crystal)
                    constants.crys_hit_unpurified = false;


            }
        }

        
        //ADDED THIS SO THAT THE SOUND AND ANIMATION GETS THE CHANCE TO FINISH 
        // it needs to be checked even when the player is paused, so i put it outside of the if statement
        if (checkAgain)
        {
            if (!clayDead.isPlaying)
            {
                dead = true;
                checkAgain = false;
            }
        }
    }

    // collision checks
    void OnCollisionEnter2D(Collision2D other)
    {

        // pick up crystal when you collide
        // TODO **RICHARD** : create a better fix for crystal being thrown and not destroyed instantly
		// note to self: we check if the crystal is stuck because it can only be picked up when stuck
        if (other.gameObject.tag == "crystal" && other.gameObject.GetComponent<CystalScript>().isStuck())
        {
            launched = false;
			//GameObject.Destroy(other.gameObject);
			GameObject.Destroy(crystal_inst);
			crystal_inst = null;


			//ANIMATION
			myAnimator.SetBool("IsThrown", false); // got crystal back
        }

        // stand on floor
        if (other.gameObject.tag == "Floor")
        {
            // 0 out verticle velocity
            //my_vel.y = 0;
            my_vel = Vector2.zero;
            // player not in air
            in_air = false;
			// reset jump
			jumped = false;
            // apply collison offset
            Vector2 offset = constants.getCollisionoffset(this.gameObject.GetComponent<Collider2D>(), other.collider);
			transform.position -= (new Vector3(offset.x, offset.y) / pixel_per_unit);
			//transform.position += new Vector3(offset.x, offset.y);
			gravitating = false;

            //ANIMATION
            myAnimator.SetBool("IsGravitating", false);
            myAnimator.SetBool("IsJumping", false);
        }

        // stick to wall when not on ground
        if (other.gameObject.tag == "Wall" && in_air)
        {
            // player stuck
            stuck = true;
			// reset jump
			jumped = false;
            // stop all velocities
            my_vel = Vector2.zero;
            // apply collision offset
            Vector2 offset = constants.getCollisionoffset(this.gameObject.GetComponent<Collider2D>(), other.collider);
            transform.position -= (new Vector3(offset.x, offset.y) / pixel_per_unit);
			gravitating = false;

            //ANIMATION
            myAnimator.SetBool("IsGravitating", false); // no longer gravitating
            myAnimator.SetBool("IsSticking", true);
            myAnimator.SetBool("isIdle", false);
        }

        if (other.gameObject.tag == "Purified" || other.gameObject.tag == "PostPurified")
        {
            //ANIMATION
            myAnimator.SetBool("IsHugging", true);

            // player stuck
            stuck = true;
			// reset jump
			jumped = false;
            // stop all velocities
            my_vel = Vector2.zero;
            // apply collision offset
            Vector2 offset = constants.getCollisionoffset(this.gameObject.GetComponent<Collider2D>(), other.collider);
            transform.position -= new Vector3(offset.x, offset.y);
            // set the collided object as parent
            transform.parent = other.gameObject.transform;
            // destroy crystal if you there is one
            GameObject temp = GameObject.FindGameObjectWithTag("crystal");
            if (!temp)
                GameObject.Destroy(temp);
			// reset launch variables
			launched = false;
        }

        if (other.gameObject.tag == "Hazard" || other.gameObject.tag == "Unpurified")
        {
            // ANIMATION
            myAnimator.SetBool("IsThrown", false); //set up for the next animations
            myAnimator.SetBool("IsDead", true);
            
            clayDead.Play();
            checkAgain = true;
            constants.g_player_paused = true; //pause player so that the player cant throw the crystal and stuff again
			my_vel = Vector2.zero;
        }
    }

    // process player input
    private void processInput()
    {

        // MOVEMENT
        // right
        if (Input.GetKey(KeyCode.D) && !gravitating && !launching && !stuck)
        {
            myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isIdle", false);
            myRenderer.flipX = false;
            // player was moving left, reset speed before moving right
            if (my_vel.x < 0)
                my_vel.x = 0;
            my_vel.x = constants.PLAYER_SPD_LIM;
			if (in_air)
				my_vel.x /= 2;
			// * Time.deltaTime;
                                                //my_vel.x = Mathf.Clamp(my_vel.x + (constants.PLAYER_MSPD * Time.deltaTime), my_vel.x, constants.PLAYER_SPD_LIM);
                                                //my_vel.x += constants.PLAYER_MSPD * Time.deltaTime;
                                                //this.gameObject.transform.position += new Vector3(0.15f, 0, 0);
        }
        // left
        else if (Input.GetKey(KeyCode.A) && !gravitating && !launching && !stuck)
        {
            myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isIdle", false);
            myRenderer.flipX = true;
            // if player was moving right, reset speed before moving left
            if (my_vel.x > 0)
                my_vel.x = 0;
            my_vel.x = -constants.PLAYER_SPD_LIM;
			if (in_air)
				my_vel.x /= 2;
			// * Time.deltaTime;
                                                 //my_vel.x = Mathf.Clamp(my_vel.x - (constants.PLAYER_MSPD * Time.deltaTime), -constants.PLAYER_SPD_LIM, my_vel.x);
                                                 //my_vel.x += -constants.PLAYER_MSPD * Time.deltaTime;
                                                 //this.gameObject.transform.position += new Vector3(-0.15f, 0, 0);
        }
        // idle
        else if (!gravitating && !in_air)
        {
            myAnimator.SetBool("isIdle", true);
            myAnimator.SetBool("isWalking", false);
            // when on floor, there's no deccel/accel
            if (!in_air)
                my_vel = Vector2.zero;
            // but there is accel/deccel in the air
            if (my_vel.x > 0)
                my_vel.x = Mathf.Clamp(my_vel.x - (constants.PLAYER_DCELL * Time.deltaTime), 0, my_vel.x);
            else if (my_vel.x < 0)
                my_vel.x = Mathf.Clamp(my_vel.x + (constants.PLAYER_DCELL * Time.deltaTime), my_vel.x, 0);
        }

		// JUMPING
		// TODO: **RICHARD** make a varied jump on button-press time
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && !jumped && !gravitating) {
			jumped = true;
			in_air = true;
			my_vel.y = constants.JUMP_FORCE * Time.timeScale; //multiplied it by 1.2 to make it go a little higher - atsi - don't chage this please; change the constants. I made the constants so that you can tweak stuff there, not in my code - richard
            //ANIMATION
            myAnimator.SetBool("IsJumping", true);
            myAnimator.SetBool("isWalking", false);
        }

        // LAUNCHING AND GRAVITATING
        // when the left click pressed and the crystal's not thrown
        if (Input.GetMouseButtonDown(0) && !launched)
        {
            // set the initial point for the launch
            launch_0 = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            launching = true;

            // ANIMATION
            myAnimator.SetBool("IsThrowing", true);
        }

        // when aiming, draw arrow for player to see
        else if (Input.GetMouseButton(0) && launching)
        {
            // get current vector from player position to mouse
            launch_1 = Input.mousePosition;
            // current launch vector
            Vector3 launch_vector = launch_1 - launch_0;
            // get magnitude of the launch_vector; capped at limit
            float force_mag = launch_vector.magnitude;
            if (force_mag > constants.L_MAX_MAG)
                force_mag = constants.L_MAX_MAG;
            // normalize launch vector now that we have magnitude
            launch_vector.Normalize();
            // TODO**RICHARD** draw arrow rotated and in scale of force: 
        }

        // when left left released and crystal not already thrown
        else if (Input.GetMouseButtonUp(0) && !launched)
        {
            // final position of launch vector
            launch_1 = Input.mousePosition;
            launching = false;
            launched = true;
            // get vector made from letting go
            Vector3 launch_vector = launch_1 - launch_0;
            // get magnitude and limit it
            float force_mag = launch_vector.magnitude;
            if (force_mag > constants.L_MAX_MAG)
                force_mag = constants.L_MAX_MAG;
            launch_vector.Normalize();
            Debug.Log(launch_vector);
            // create a crystal at an offset from players
            /*GameObject crystal_temp = Instantiate(crystal, transform.position +
                        (launch_vector * launch_offset), transform.rotation);*/
			crystal_inst = Instantiate(crystal, transform.position +
						(launch_vector * launch_offset), transform.rotation);
			//crystal_temp.tag = "crystal";
			crystal_inst.tag = "crystal";
			//crystal_temp.GetComponent<CystalScript>().setPlayer(this.gameObject);
			crystal_inst.GetComponent<CystalScript>().setPlayer(this.gameObject);
			// apply force to launch crystal
			//crystal_temp.GetComponent<CystalScript>().setVelocity(force_mag * constants.L_FORCE_MULT * launch_vector);
			crystal_inst.GetComponent<CystalScript>().setVelocity(force_mag * constants.L_FORCE_MULT * launch_vector);

			//ANIMATION
			myAnimator.SetBool("IsHugging", false); //CASE FOR WHEN PLAYER THROWS CRYSTAL FROM ENEMY?
            
        }

        // when right click held down and there is a crystal thrown already: gravitate towards crystal
        if (Input.GetMouseButton(1) && launched && crystal_inst && crystal_inst.GetComponent<CystalScript>().isStuck())
        {
            //ANIMATION
            myAnimator.SetBool("IsGravitating", true);
            myAnimator.SetBool("IsSticking", false);

            //GameObject crystal_temp = GameObject.FindWithTag("crystal");
            // vector from player to crystal
            Vector2 grav_vector = crystal_inst.transform.position - transform.position;
            grav_vector.Normalize();
            my_vel += grav_vector * constants.CRYSTAL_FORCE * Time.deltaTime;
            //my_accl = grav_vector * constants.CRYSTAL_FORCE * Time.deltaTime;
            in_air = true;
            gravitating = true;
            // unstick
            stuck = false;
            stick_time_cntr = 0;
            // unparent from any purified; temp needed to not destroy player if parent destroyed
            Transform temp = transform.parent;
            transform.parent = null;
            // if attached to a purified object, set ot Post-puriofied
            if (temp && temp.tag == "Purified")
                temp.tag = "PostPurified";
        }
		if (Input.GetMouseButtonUp(1) /*&& launched*/)
			gravitating = false;
    }

    // returns if player is dead
    public bool isDead()
    {
        
        return dead;
    }

    public void setDead(bool set)
    {
        dead = set;
    }

	public void respawn(Vector2 spawn_point)
	{
		if (crystal_inst)
			GameObject.Destroy(crystal_inst);
		crystal_inst = null;
		my_accl = Vector2.zero;
		my_vel = Vector2.zero;
		launched = false;
		launching = false;
		stuck = false;
		in_air = true;
		gravitating = false;
		dead = false;
		jumped = false;
		checkAgain = false;
		transform.position = spawn_point;
	}
}
