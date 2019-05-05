using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCrystalCont : MonoBehaviour
{
    //IGNORE COLLSIONS, atsi code
    public GameObject[] hazards;
    public GameObject[] floors;
    public GameObject[] walls;

    protected Camera mainCam;

	Animator myAnimator;

	// "constants" (temp)
	protected static float GRAV = 9.8f; // gravity
	protected static float MIN_MOV = 0.001f; // if delta position is smaller than this then do nothing (removes jitters I hope)
	protected static float SHELL_PAD = 0.01f; // bit of padding around object when checking collisions
	protected static float GROUND_NORMAL_Y = 0.65f;

	// crystal's velocity
	protected Vector2 my_vel = Vector2.zero;
	// bool to check if the cyrstal is stuck
	protected bool stuck = false;
	// bool to check if the crystal is returning
	protected bool is_returning = false;
	// time limit for how long the crystal sticks
	float stick_time_limit = 2f;
	float stick_time_cntr = 0f;
	// reference to any enemy that the crystal is stuck to
	protected GameObject enemy = null;
	// reference to player
	protected GameObject player;

	// reference to rigid body
	protected Rigidbody2D rb;

	// new vars from raycasting
	// buffer to store all objects hit by collision raycast
	protected RaycastHit2D[] hit_buffer = new RaycastHit2D[16];
	protected List<RaycastHit2D> hit_list = new List<RaycastHit2D>();
	// filter for where to check for player's collisions
	protected ContactFilter2D contact_filter;
	protected bool on_ground = false;
	// current ground normal
	protected Vector2 ground_normal;

	float pixel_per_unit;

	AudioSource crystalStickSound;
    AudioSource crystalHazardSound;

	/*SCRATCH/REFERENCE*/
	Vector3 crystalPos;

	// Start is called before the first frame update
	void Start() {
		rb = this.gameObject.GetComponent<Rigidbody2D>();
		rb.freezeRotation = true;
		mainCam = Camera.main;

		// get access to the animator of the player, i want to stop clays throwing anim here in this script
		myAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        // set up sounds !
        var aSources = GetComponents<AudioSource>();
        crystalStickSound = aSources[0];
        crystalHazardSound = aSources[1];

        //SETTING UP ALL COLLISIONS TO BE IGNORED
        hazards = GameObject.FindGameObjectsWithTag("Hazard");
        walls = GameObject.FindGameObjectsWithTag("Wall");
        floors = GameObject.FindGameObjectsWithTag("Floor");

    }

	// normal updates like stick counters and returning
	private void Update()
	{
        // if crystal paused, don't update anything
        if (constants.g_crystal_paused)
        {
            stuck = true;
            return;

        }

        // crsytal get's taken if close enough to player

        // increase stick timer if stuck
        if (stuck) {
            /*foreach (GameObject hazard in hazards)
			{
				Physics2D.IgnoreCollision(hazard.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
			}*/
            stick_time_cntr += Time.deltaTime;
        }
		// return crystal if the stick-timer finishes
		if (stick_time_cntr > stick_time_limit)
			returnCrystal();

		// return crystal to player if it goes off screen
		Vector2 viewPos = mainCam.WorldToViewportPoint(transform.position);
		if (!is_returning && ((viewPos.x < 0) || (viewPos.x > 1) || (viewPos.y < 0) || (viewPos.y > 1)))
		{
			Debug.Log("Crystal off screen");
			my_vel = Vector2.zero;
			returnCrystal();
		}
	}

	// fixedupdate is called every frame where physics is applied
	void FixedUpdate()
	{
		// if crystal paused, don't update anything
		if (constants.g_crystal_paused)
        {
            stuck = true;
            return;
        }

        // get velocity to return to player
        if (is_returning)
		{
			transform.parent = null;
			Vector2 retVector = player.transform.position - transform.position;
            
            // if close enough, player collects crsytal
            if (retVector.magnitude < 1.5)
			{
				player.GetComponent<RaycastPlayerCont>().takeCrystal();
				return;
			}
			// use non-physics/rigidbody interpolation to move crystal since physics simulation is turned off when returning
			retVector.Normalize();
			my_vel = constants.RETURN_FORCE * retVector;
			Vector2 temp_pos = transform.position;
			temp_pos += retVector * constants.RETURN_FORCE * Time.deltaTime;
			transform.position = temp_pos;
		}

		// apply velocity to move crystal
		Vector2 delta_pos = my_vel * Time.deltaTime;
		move(delta_pos);

		/*// set tutorial bools
		// set true in collision check with "Unpurified" tag
		if (constants.crys_hit_unpurified && (!transform.parent || transform.parent.tag != "Purified" || transform.parent.tag != "PostPurified"))
			constants.crys_hit_unpurified = false;*/
	}

	void move(Vector2 delta_pos)
	{
		// list of closest objects hit in raycast
		List<RaycastHit2D> closest_hits = new List<RaycastHit2D>();

		float delta_pos_mag = delta_pos.magnitude;
		// do nothing if delta pos is too small
		//if (delta_pos_mag <= MIN_MOV)
			//return;

		// check collisions (through raycasts) with delta position and apply effects
		int collision_cnt = rb.Cast(delta_pos, contact_filter, hit_buffer, delta_pos_mag + SHELL_PAD);
		hit_list.Clear();
		for (int i = 0; i < collision_cnt; i++)
			hit_list.Add(hit_buffer[i]);

		// get the min distance from all hit objects and set that to new distance
		for (int i = 0; i < hit_list.Count; i++)
		{
			if (hit_list[i].transform.gameObject == player)
				continue;
			Vector2 curr_norm = hit_list[i].normal;
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

	// applies special collision events for the player based on raycast hit objects
	protected bool applySpecialCollisions(List<RaycastHit2D> close_hits)
	{
		bool _out = true;
		if (is_returning)
			return _out;

		for (int i = 0; i < close_hits.Count; i++)
		{
			//if (close_hits[i].transform.gameObject == player)
			//player.GetComponent<RaycastPlayerController>().takeCrystal();
			// return when you hit a hazard
			if (close_hits[i].transform.tag == "Hazard")
            {
                returnCrystal();
                crystalHazardSound.Play();
            }
			// stick to collision if not stuck
			else if (!transform.parent)
				stick(close_hits[i].transform.gameObject);
			// always stick to unpurified object
			else if (close_hits[i].transform.tag == "Unpurified")
                stick(close_hits[i].transform.gameObject);
              
		}

		return _out;
	}

	// stick to given object
	protected void stick(GameObject obj)
	{
        crystalStickSound.Play();
		stuck = true;
		transform.parent = obj.transform;
		// purify
		if (obj.tag == "Unpurified")
        {
            // setting tutorial bool!!
            constants.crys_hit_unpurified = true;

            obj.tag = "PrePurified";
        }
	}

	// sets velocity of the crystal (player calls when launching)
	public void setVelocity(Vector2 vel)
	{
		my_vel = vel;
	}

	// stick to given object
	private void stickTo(GameObject obj)
	{
		// stop all velocity
		my_vel = Vector2.zero;
		transform.parent = obj.transform;
		if (obj.tag == "Unpurified")
			obj.tag = "Purified";
	}

	// sets up crystal to be returned
	private void returnCrystal()
	{
        transform.parent = null;
		is_returning = true;
		GetComponent<Rigidbody2D>().simulated = false;
    }

	// set's the reference to the player
	public void setPlayer(GameObject _player)
	{
		this.player = _player;
	}

	public bool isStuck() { return stuck; }

	public bool isReturning() { return is_returning; }

	//pls do not delete richard, i am using this in one of my tutorial scripts, thank
	public void setStuck(bool set)
	{
		stuck = set;
	}
}
