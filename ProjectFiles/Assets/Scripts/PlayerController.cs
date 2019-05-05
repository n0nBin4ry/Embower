using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour {
  
    public float startTime;
    public float distance;
    public Vector2 playerPos = new Vector2(0, 0);
    bool setTime = true;

    MouseScript mouse;

    void Start() {
    mouse = GameObject.Find("ArrowOrginPoint").GetComponent<MouseScript>();
        mouse.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
		// when the left button pressed and the crystal's not thrown
		if (Input.GetMouseButtonDown(0) && !launched)
		{
            mouse.gameObject.SetActive(true);
        }
		// when left button released and crystal not already thrown
		else if (Input.GetMouseButtonUp(0) && !launched)
        {
            mouse.gameObject.SetActive(false);
            launched = true;
			// get vector made from letting go, use the mouse position with the position of the player to get a vector between the two
            //dont want to make a vector from when the player first presses the button to letting it go- thats in the wrong direction !
			Vector3 launch_vector = Input.mousePosition - this.transform.position;
			// get magnitude and limit it
			float force_mag = launch_vector.magnitude;
		if (force_mag > max_mag)
			force_mag = max_mag;
			// normalize the force vector for future calculations
			launch_vector.Normalize();
			// create a crystal at an offset from players
			GameObject crystal_temp = Instantiate(crystal, this.transform.position +
						(launch_vector * launch_offset), transform.rotation); //<-- fix to rotation of mouse so player can throw it different directions
         
			// apply force to launch crystal
			crystal_temp.GetComponent<Rigidbody2D>().AddForce(launch_vector *
						max_mag * force_mult, ForceMode2D.Force);
            
        }
		// when right held down and there is a crystal thrown already
		else if (Input.GetMouseButton(1) && launched) {
            // gravitate towards crystal
			GameObject crystal_temp = GameObject.FindWithTag("crystal");

            //Vector2 grav_vector = this.transform.position - crystal_temp.transform.position;
            //grav_vector.Normalize();
            // NOTE: trying Force force-type here. but we should experiment
            // Distance moved = time * speed.

            if(setTime)
            {
                startTime = Time.time;
                setTime = false;
            }

            float distCovered = (Time.time - startTime) *10;

            // Fraction of journey completed = current distance divided by total distance.
            float fracJourney = distCovered / distance;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(this.transform.position, crystal_temp.transform.position, fracJourney);
        }
	}

	// collision checks
	void OnCollisionEnter2D(Collision2D other)
    {
		//Debug.Log("Crystal touched?\n");
		// pick up crystal when you collide (just for now at least)
		if (other.gameObject.tag == "crystal" && Input.GetMouseButton(1)) {
            launched = false;
			GameObject.Destroy(other.gameObject);
            startTime = 0;
            setTime = true;
            
			//Debug.Log("Crystal touched\n");
		}
	}

	// pointer to crystal prefab
	public GameObject crystal;

	// the crystal has been thrown
	protected bool launched = false;
	// the player is preparing to launch; player will not be able to move when launching
	protected bool launching = false;
	// the initial position of the launch vector
	protected Vector2 launch_0;
	// the final position of the launch vector
	protected Vector2 launch_1;
	// the offset of crystal (compared to player) when launched
	protected float launch_offset = 1; // arbitrary for now
	// the force coefficent to mulitply by launch_vector magnitude
	protected float force_mult = 100;
	// max force applied to crystal
	protected float max_mag = 10; // arbitrary for now
	// force at which we gravitate to crystal
	protected float grav_force = 50; // arbitrary for now
}
