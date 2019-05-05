using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPlayerCont : Actor_Base
{
    // VARIABLES
    // ATSI TRYING SOMETHING HERE....
    private IEnumerator coroutine;
    bool waiting = false;
    bool playedSound = false;

    // "constants" (temp)
    protected static float GRAV = 9.8f; // gravity
	protected static float MIN_MOV = 0.001f; // if delta position is smaller than this then do nothing (removes jitters I hope)
	protected static float SHELL_PAD = 0.01f; // bit of padding around object when checking collisions
	protected static float GROUND_NORMAL_Y = 0.65f;
	protected static float HANG_LIMIT = 2.0f;
	

	// pointer to crystal prefab
	public GameObject crystal;
	protected GameObject crystal_inst = null;

	// reference to game controller
	public NewGameCont game_controller;

	// accelerations
	//protected Vector2 my_accl = Vector2.zero;
	// velocities
	//protected Vector2 my_vel = Vector2.zero;

	// object state for object controller
	//protected constants.ObjState my_state = constants.ObjState.Active;

	// the crystal has been thrown
	protected bool launched = false;
	// the player is preparing to launch; player will not be able to move when launching
	protected bool launching = false;
	// bool to tell us if player is stuck
	protected bool stuck = false;
	// to tell us if player has jumped yet
	protected bool jumped = false;
	// tell us if player is hanging from wall
	protected bool is_hanging = false;
	protected float hang_timer = 0.0f;

	// pixels per unit of current sprite
	float pixel_per_unit;

	/*// time limit for how long a player sticks
	float stick_time_limit = 2f;
	float stick_time_cntr = 0f;*/

	// the initial position of the launch vector
	protected Vector2 launch_0;
	// the final position of the launch vector
	protected Vector2 launch_1;

	// the offset of crystal (compared to player) when launched
	protected float launch_offset = 1; // arbitrary for now TODO **RICHARD** : make the offset more dynamic

	// new vars from raycasting
	// reference to player's rigidbody
	protected Rigidbody2D rb;
	// the 2d collider
	protected Collider2D coll;
	// vector for player's current velocity
	protected Vector2 my_vel = Vector2.zero;
	// buffer to store all objects hit by collision raycast
	protected RaycastHit2D[] hit_buffer = new RaycastHit2D[16];
	protected List<RaycastHit2D> hit_list = new List<RaycastHit2D>();
	// filter for where to check for player's collisions
	protected ContactFilter2D contact_filter;
	protected bool on_ground = false;
	// current ground normal
	protected Vector2 ground_normal;
	// if flying through air from gravitating
	protected bool flying = false;
	// jump speed
	protected float jump_speed = 5f;
	protected float walk_speed = constants.HORZ_SPD_LIM;
	// bool for telling us that the crystal was grabbed recently
	protected bool crystal_aquired = false;

	// input bools
	bool walk_left_held = false;
	bool walk_right_held = false;
	bool jump_pressed = false;
	bool jump_released = false;
	bool launch_pressed = false;
	bool launch_held = false;
	bool launch_released = false;
	bool grav_held = false;

	//atsis not so great code, LOL
	AudioSource clayDead;
    AudioSource throwCrystalSound;
    AudioSource gravitationSound;
    bool playSound = true;
    bool play = true;
    bool checkAgain = false;
    // animation components
    SpriteRenderer myRenderer;
    Animator myAnimator;



    // METHODS

    // Start is called before the first frame update
    protected override void Start() {

        //ATSI
        coroutine = WaitAndDie(2f, new Vector2 (0,0));

		// add self to the actor-list
		game_controller.addActor(this.gameObject);

		rb = GetComponent<Rigidbody2D>();
		rb.freezeRotation = true;
		coll = GetComponent<Collider2D>();
		// components
		myAnimator = gameObject.GetComponent<Animator>();
		myRenderer = gameObject.GetComponent<SpriteRenderer>();
		// might not be needed anymore
		pixel_per_unit = myRenderer.sprite.pixelsPerUnit;
        // set up sounds !
        var aSources = GetComponents<AudioSource>();
        clayDead = aSources[0];
        throwCrystalSound = aSources[1];
        gravitationSound = aSources[2];
    }

	// fixed update for all physics-related updates
	private void FixedUpdate() // // LOOK HERE FOR THE DIE-TWICE BUG
	{

        if (Input.GetKeyDown(KeyCode.Q)) //reset crystal in case of bug
        {
            takeCrystal();
        }
        if (my_state == constants.ObjState.Paused || constants.g_player_paused)
        {
            //for tutorial and such
            myAnimator.SetBool("isIdle", true); // set his state to idle while he's paused
            return;
        }

		// Move-Phase: Accel/Force -> Velocity -> Position
		// y-axis
		//apply gravity if not stuck or hugging
		if (!stuck && !(grav_held && launched) && coll.enabled)
			my_vel.y += constants.W_GRAVITY * Time.deltaTime;
		// move() will set to true if actually on ground
		on_ground = false;
		// apply new velocity to delta position
		Vector2 delta_pos = my_vel * Time.deltaTime;
		move(delta_pos, true);

		// x-axis
		// apply new velocity to delta position
		delta_pos = my_vel * Time.deltaTime;
		move(delta_pos, false);
	}

	// Update is called once per frame
	protected override void Update() {
		// only allow hanging to wall over a short amount of time
		if (is_hanging) {
			hang_timer += Time.deltaTime;
			if (hang_timer > HANG_LIMIT)
				unStick();
		}
		// proccess input
		processInput();
	}

	void move(Vector2 delta_pos, bool y_mov)
	{
		// list of closest objects hit in raycast
		List<RaycastHit2D> closest_hits = new List<RaycastHit2D>();

		float delta_pos_mag = delta_pos.magnitude;
		// do nothing if delta pos is too small
		//if (delta_pos_mag <= MIN_MOV)
		//return;
		if (!coll.enabled)
			return;

		// check collisions (through raycasts) with delta position and apply effects
		int collision_cnt = rb.Cast(delta_pos, contact_filter, hit_buffer, delta_pos_mag + SHELL_PAD);
		hit_list.Clear();
		for (int i = 0; i < collision_cnt; i++)
			hit_list.Add(hit_buffer[i]);

		// get the min distance from all hit objects and set that to new distance
		for (int i = 0; i < hit_list.Count; i++)
		{
			// special case: hit crystal, so don't stop but take crystal
			if (hit_list[i].transform.gameObject == crystal_inst)
			{
				if (crystal_inst.GetComponent<RaycastCrystalCont>().isStuck())
					takeCrystal();
				continue;
			}

			// check if player lands on top of something
			Vector2 curr_norm = hit_list[i].normal;
			if (curr_norm.y > GROUND_NORMAL_Y)
			{
				//on_ground = true;
				land();
				// set new ground normal on y movement
				if (y_mov)
				{
					ground_normal = curr_norm;
					curr_norm.x = 0;
				}
			}

			// adjust velocity to prevent going into other collisions, based on projection scalar
			float projection = Vector2.Dot(my_vel, curr_norm);
			if (projection < 0)
				my_vel = my_vel - (projection * curr_norm);
			// change delta position 
			// also store the nearest objects collided with
			float modified_delta = hit_list[i].distance - SHELL_PAD; // account for shell padding so that player doesn't enter collided object

			// if there is an object even closer then clear the close-collision list and add new object
			if (modified_delta < delta_pos_mag)
			{
				delta_pos_mag = modified_delta;
				closest_hits.Clear();
				closest_hits.Add(hit_list[i]);
			}
			// of if equal, just add object to close-collision hit
			else if (modified_delta == delta_pos_mag)
				closest_hits.Add(hit_list[i]);
		}

		// apply extra collision effects if needed; on returning false, don't change position
		if (!applySpecialCollisions(closest_hits))
			return;

		// set new position
		rb.MovePosition(rb.position + delta_pos.normalized * delta_pos_mag);

		// stop momentum on sticking
		if (stuck)
			my_vel = Vector2.zero;
	}

	private void processInput()
	{
		// set input bools
		walk_right_held = Input.GetKey(KeyCode.D);
		walk_left_held = Input.GetKey(KeyCode.A);
		jump_pressed = Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space);
		jump_released = Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space);
		launch_pressed = Input.GetMouseButtonDown(0);
		launch_held = Input.GetMouseButton(0);
		launch_released = Input.GetMouseButtonUp(0);
		grav_held = Input.GetMouseButton(1);

		// MOVEMENT
		// walk right
		if (walk_right_held && !flying && !launching && !stuck)
		{
            if(on_ground)
			    myAnimator.SetBool("isWalking", true);
			myAnimator.SetBool("isIdle", false);
            myRenderer.flipX = false;
			my_vel.x = walk_speed;
            // move slightly slower in air
            if (!on_ground)
				my_vel.x *= constants.JUMP_MOV_CO;
        }
		// walk left
		else if (walk_left_held && !flying && !launching && !stuck)
		{
            if (on_ground)
                myAnimator.SetBool("isWalking", true);
            myAnimator.SetBool("isIdle", false);
            myRenderer.flipX = true;
			my_vel.x = -walk_speed;
			// move slightly slower in air
			if (!on_ground)
				my_vel.x *= constants.JUMP_MOV_CO;

        }
		// idle
		else
		{
			myAnimator.SetBool("isWalking", false);
            // when on floor, there's no deccel/accel or sliding
            if (on_ground && !stuck)
            {
                myAnimator.SetBool("isIdle", true);
                myAnimator.SetBool("IsJumping", false);
                my_vel = Vector2.zero;
            }
			// when in air without flying, no x-vel
			if (!grav_held && !flying)
				my_vel.x = 0;
			// but there is accel/deccel in the air when flying and not gravitating
			if (!grav_held && flying && my_vel.x > 0)
				my_vel.x = Mathf.Clamp(my_vel.x - (constants.PLAYER_DCELL * Time.deltaTime), 0, my_vel.x);
			else if (!grav_held && flying && my_vel.x < 0)
				my_vel.x = Mathf.Clamp(my_vel.x + (constants.PLAYER_DCELL * Time.deltaTime), my_vel.x, 0);
		}

		// JUMPING
		// initiate jump on press
		if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && !jumped && !grav_held)
		{
			jumped = true;
			// unstick if stuck
            if(stuck)
			    unStick();
            else
                myAnimator.SetBool("IsJumping", true);
            on_ground = false;
			// jumping cancels flying/grav momentum
			if (flying)
			{
				flying = false;
				my_vel.x = 0;
			}
			//my_vel.y = jump_speed;
			my_vel.y = constants.JUMP_SPD;
			//ANIMATION
			myAnimator.SetBool("isWalking", false);
            myAnimator.SetBool("isIdle", false);
		}
		// adjust jump speed on release; if released before climax then jump reduced
		if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W)) && !jumped && !grav_held)
		{
			if (my_vel.y > 0)
				my_vel.y *= 0.5f;
			// fall animation
			//todo (OR MAYBE JUST SET ANIMATION IN REGULAR UPDATEIF NOT ON GROUND AND Y-VEL < 0)
		}

		// LAUNCHING AND GRAVITATING
		// when the left click pressed and the crystal's not thrown
		if (launch_pressed && !launched && !constants.g_crystal_paused)
		{
			// set the initial point for the launch
			launch_0 = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			launching = true;

			// ANIMATION
			myAnimator.SetBool("IsThrowing", true);
            throwCrystalSound.Play();
		}

		// when aiming, draw arrow for player to see
		else if (launch_held && launching)
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

            myAnimator.SetBool("IsThrowing", true);
        }

		// when left left released and crystal not already thrown
		else if (launch_released && !launched && !constants.g_crystal_paused)
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
			crystal_inst = Instantiate(crystal, transform.position +
						(launch_vector * launch_offset), transform.rotation);
			crystal_inst.tag = "crystal";
			crystal_inst.GetComponent<RaycastCrystalCont>().setPlayer(this.gameObject);
			// apply force to launch crystal
			crystal_inst.GetComponent<RaycastCrystalCont>().setVelocity(force_mag * constants.L_FORCE_MULT * launch_vector);

			//ANIMATION
			myAnimator.SetBool("IsHugging", false); //CASE FOR WHEN PLAYER THROWS CRYSTAL FROM ENEMY?
            // FINE TUNE
            myAnimator.SetBool("IsThrown", true);
            myAnimator.SetBool("IsThrowing", true);

        }

		// when right click held down and there is a crystal thrown already: gravitate towards crystal
		if (grav_held && launched && crystal_inst && crystal_inst.GetComponent<RaycastCrystalCont>().isStuck() && !crystal_inst.GetComponent<RaycastCrystalCont>().isReturning())
		{
			//ANIMATION
			myAnimator.SetBool("IsGravitating", true);
            myAnimator.SetBool("isIdle", false);
            myAnimator.SetBool("IsHugging", false);
			myAnimator.SetBool("IsSticking", false);

            //play sound
            if(playSound)
            {
                playSound = false;
                gravitationSound.Play();
            }
            

            // unstick player if stuck
            unStick();

			// player starts flying
			flying = true;

			// vector from player to crystal
			Vector2 grav_vector = crystal_inst.transform.position - transform.position;
			grav_vector.Normalize();
			my_vel += grav_vector * constants.CRYSTAL_FORCE * Time.deltaTime;
			//in_air = true;
			on_ground = false;
			// unparent from any purified; temp needed to not destroy player if parent destroyed
			Transform temp = transform.parent;
			transform.parent = null;
            // if attached to a purified object, set ot Post-puriofied
            if (temp && temp.tag == "Purified")
            {
                temp.tag = "PostPurified";
            }
		}
	}

	// applies special collision events for the player based on raycast hit objects
	protected bool applySpecialCollisions(List<RaycastHit2D> close_hits) // LOOK HERE FOR THE DIE-TWICE BUG
	{
		bool _out = true;

        for (int i = 0; i < close_hits.Count; i++)
        {
			// die when hit an enemy or hazard
			if (close_hits[i].transform.tag == "Hazard" || close_hits[i].transform.tag == "Unpurified") {
				die();
			}

			// always stick to a purified object if it has the crystal stuck to it
			else if (close_hits[i].transform.tag == "Purified" || close_hits[i].transform.tag == "PrePurified")
			{
				// check if crystal attatched to hit obj or if crystal was recently aquired
				if ((crystal_aquired || (crystal_inst && crystal_inst.transform.parent == close_hits[i].transform)) && !stuck)
					stick(close_hits[i].transform.gameObject);
				// else die
				else
					die();
				constants.plyr_hit_purified = true;
			}

			// hang to collision if not landing on top, if it has the crystal stuck to it
			else if (!transform.parent && !on_ground && close_hits[i].normal.y <= GROUND_NORMAL_Y) {
				// check if crystal attatched to hit obj or if crystal was recently aquired
				if ((crystal_aquired || (crystal_inst && crystal_inst.transform.parent == close_hits[i].transform)) && !stuck)
					hang(close_hits[i].transform.gameObject);
				// else stop movement
				else
					stop();
			}
        }
		return _out;
	}

	// launches crystal from player at given velocity
	private void launchCrystal(Vector2 launch_vel)
	{

	}

	// removes crystal from scene
	public void takeCrystal()
	{
		if (!crystal_inst)
			return;
		GameObject.Destroy(crystal_inst);
        myAnimator.SetBool("IsGravitating", false);
        myAnimator.SetBool("IsThrown", false);
        crystal_inst = null;
		launched = false;
		crystal_aquired = true;
	}

	// stick to other
	private void stick(GameObject other)
	{
        //transform.parent = other.transform;

        playSound = true; //reset this variable so gravitating sound plays next time playe gravitates

		// if purifying an enemy/object
        if (other.transform.tag == "PrePurified"/*"Purified"*/ || other.transform.tag == "Unpurified") {
			other.transform.tag = "Purified";
			// change animation
			myAnimator.SetBool("IsHugging", true);
            myAnimator.SetBool("isIdle", false);
            myAnimator.SetBool("IsJumping", false);
			// disable collision box for other object (so that the player can throw crystal through hugged object)
			other.GetComponent<BoxCollider2D>().enabled = false; // LOOK HERE FOR invincibility bug
        }

		transform.parent = other.transform;
		//coll.enabled = true; // just to make sure that the collider stays on

		stuck = true;
		flying = false;
		jumped = false;
		crystal_aquired = false;
		//my_vel = Vector2.zero;
		// HUG ANIMATION
	}

	// unstick to whatever we are stuck to; also unhangs
	private void unStick()
	{
        myAnimator.SetBool("IsHugging", false);

        if (!transform.parent)
			return;
		if (transform.parent.tag == "Purified")
        {
            transform.parent.tag = "PostPurified";
            myAnimator.SetBool("IsGravitating", true);
        }
        else
        {
            myAnimator.SetBool("IsSticking", false);
            myAnimator.SetBool("IsGravitating", true);
        }
		transform.parent = null;
		stuck = false;
		is_hanging = false;
		hang_timer = 0.0f;
	}

	private void hang(GameObject other) {
		stick(other);
        myAnimator.SetBool("IsSticking", true);
        myAnimator.SetBool("isIdle", false);
        is_hanging = true;
		hang_timer = 0.0f;
	}

	// land on ground
	private void land()
	{
        myAnimator.SetBool("IsGravitating", false);
        myAnimator.SetBool("IsHugging", false);
        myAnimator.SetBool("IsSticking", false);
        myAnimator.SetBool("IsThrowing", false);
        myAnimator.SetBool("IsJumping", false); //basically just reset all the animation bools

        playSound = true; //reset this variable so gravitating sound plays next time playe gravitates

        clayDead.volume = 1;
        playedSound = false;

        on_ground = true;
		flying = false;
		jumped = false;
		crystal_aquired = false;
	}

	// kills plays animations and anything else before changing state
	protected override void die()
	{
		// kill parent if enemy
		if (transform.parent && (transform.parent.tag == "Purified" || transform.parent.tag == "PrePurified"))
			transform.parent.tag = "PostPurified";
		// unparent
		transform.parent = null;
		// disable plaer collider to prevent activating more collisions
		coll.enabled = false;

        //ATSI STUFF

		// DOUBLE-DYING PROB: after coreoutine, the state is set to dead again; OR maybe it has to do with the hazards? since when killed by enemies, it's only 1 death

        //bool for waiting, so that this is only called once
        if(!waiting) //set to dead, which in the game controller, now calls the respawn function, which will then call my wait corotuine...
            my_state = constants.ObjState.Dead;

        //SEE BELOW
    }

	public void respawn(Vector2 spawn_point)
	{
		if (crystal_inst)
			GameObject.Destroy(crystal_inst);

        StartCoroutine(WaitAndDie(2f, spawn_point)); //CALL MY WAIT COROUTINE, using this to get animations and sound just right, 
        //it is the only way : (

		crystal_inst = null;
		my_vel = Vector2.zero;
		launched = false;
		launching = false;
		stuck = false;
		on_ground = false;
		grav_held = false;
		//my_state = constants.ObjState.Active;
		jumped = false;
		checkAgain = false;
        constants.g_player_paused = false; //unpause player and continue to check for bools!
        //having a little bool here, code still checks for collisions at last minute and sometimes registers a second collision
        //between enemies and player : (

    }

    private IEnumerator WaitAndDie(float waitTime, Vector2 respawnPos)
    {
        constants.g_player_paused = true; //stop checking for collisions!!

        myAnimator.SetBool("IsDead", true);
        if(!playedSound)
            clayDead.Play();//play death sound
        playedSound = true;

        waiting = true; //since the call is happening within update, this bool will keep it from getting called more than once
        yield return new WaitForSeconds(clayDead.clip.length);
        waiting = false;

        // set state to death when sound is done playing
        myAnimator.SetBool("IsDead", false);
        transform.position = respawnPos;
		// turn on collider again
		coll.enabled = true;
		// set self to active
		my_state = constants.ObjState.Active;
        clayDead.volume = 0;
	}
	
	// stop moving and clear any movement related vars
	public void stop() {
		flying = false; 
		stuck = false;
		on_ground = false;
		my_vel.x = 0;
		crystal_aquired = false;
	}
}

