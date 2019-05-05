using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerController : MonoBehaviour
{
	// variables

	// pointer to crystal prefab
	public GameObject crystal;

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

	// the initial position of the launch vector
	protected Vector2 launch_0;
	// the final position of the launch vector
	protected Vector2 launch_1;
	// the offset of crystal (compared to player) when launched
	protected float launch_offset = 1; // arbitrary for now
									   // the force coefficent to mulitply by launch_vector magnitude
	protected float force_mult = 10;
	// max force applied to crystal
	protected float max_mag = 2; // arbitrary for now
								 // force at which we gravitate to crystal
	protected float grav_force = 50; // arbitrary for now

	// methods

	// Start is called before the first frame update
	void Start() { }

	// Update is called once per frame
	void Update()
	{

		// Process-Input phase
		processInput();

		// Move-Phase
		// acceleration on velocity
		if (!stuck && in_air)
		{
			my_vel.y -= constants.W_GRAVITY * Time.deltaTime;
			//Debug.Log("applying grav");
		}

		// decellerate horizontal velocity
		if (my_vel.x > 0)
			my_vel.x = Mathf.Clamp(my_vel.x - (constants.PLAYER_DCELL * Time.deltaTime), 0, my_vel.x);
		else if (my_vel.x < 0)
			my_vel.x = Mathf.Clamp(my_vel.x + (constants.PLAYER_DCELL * Time.deltaTime), my_vel.x, 0);

		// velocity on position
		this.gameObject.transform.position += new Vector3(my_vel.x, my_vel.y) * Time.deltaTime;
	}

	// collision checks
	void OnCollisionEnter2D(Collision2D other)
	{
		//Debug.Log("Collision");
		// pick up crystal when you collide (just for now at least)
		if (other.gameObject.tag == "crystal" && Input.GetMouseButton(1))
		{
			launched = false;
			GameObject.Destroy(other.gameObject);
			//Debug.Log("Crystal touched\n");
		}

		// stand on floor
		if (other.gameObject.tag == "Floor")
		{
			Debug.Log("Floor touched");
			my_vel.y = 0;
			in_air = false;
			Vector2 offset = constants.getCollisionoffset(this.gameObject.GetComponent<Collider2D>(), other.collider);
			transform.position -= new Vector3(offset.x, offset.y);
		}

		// stick to wall when not on ground
		if (other.gameObject.tag == "Wall" && in_air)
		{
			stuck = true;
			my_vel = Vector2.zero;
			Vector2 offset = constants.getCollisionoffset(this.gameObject.GetComponent<Collider2D>(), other.collider);
			transform.position -= new Vector3(offset.x, offset.y);
		}
	}

	private void processInput()
	{
		// debug
		/*if (Input.GetMouseButtonDown(0)) {
			Debug.Log("Player pos: " + Camera.main.WorldToScreenPoint(gameObject.transform.position));
			Debug.Log("Mouse pos: " + Camera.main.WorldToScreenPoint(Input.mousePosition));
			Debug.Log(Input.mousePosition);
			Debug.Log("launch vect: " + (Input.mousePosition - Camera.main.WorldToScreenPoint(gameObject.transform.position)));
		}*/

		// when the left button pressed and the crystal's not thrown
		if (Input.GetMouseButtonDown(0) && !launched)
		{
			launch_0 = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			launch_1 = Input.mousePosition;
			launched = true;
			// get vector made from letting go
			Vector3 launch_vector = launch_1 - launch_0;
			// get magnitude and limit it
			float force_mag = launch_vector.magnitude;
			if (force_mag > max_mag)
				force_mag = max_mag;
			// normalize the force vector for future calculations
			launch_vector.Normalize();
			//Debug.Log(launch_vector);
			// create a crystal at an offset from players
			GameObject crystal_temp = Instantiate(crystal, transform.position +
						(launch_vector * launch_offset), transform.rotation);
			crystal_temp.tag = "crystal";
			// apply force to launch crystal
			crystal_temp.GetComponent<CystalScript>().setVelocity(force_mag * force_mult * launch_vector);
		}

		// when right held down and there is a crystal thrown already
		else if (Input.GetMouseButton(1) && launched)
		{
			// gravitate towards crystal
			GameObject crystal_temp = GameObject.FindWithTag("crystal");
			Vector2 grav_vector = crystal_temp.transform.position - transform.position;
			grav_vector.Normalize();
			my_vel = grav_vector * grav_force * Time.deltaTime;
			in_air = true;
			// NOTE: trying Force force-type here. but we should experiment
			//GetComponent<Rigidbody2D>().AddForce(grav_vector * grav_force, ForceMode2D.Force);
		}
	}

	public bool isLaunching()
	{
		return launching;
	}
}
