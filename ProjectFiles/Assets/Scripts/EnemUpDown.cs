using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemUpDown : Actor_Base
{
	// bool to tell us which way we are moving
	public bool movingUp = true;
	// speed at which we move
	public float speed;
	// distance in which we move from origin
	public float distance = 2;
	// reference to master game controller
	public NewGameCont game_controller;
	// point at which it starts on the x-axis
	protected float origin_point;
	// position in which it starts, used for reseting
	protected Vector2 start_pos = Vector2.zero;
	// metric bool
	bool hugged = false;

	SpriteRenderer myRenderer;

	Animator enemyAnim;
	//AudioSource purifiedSound;

	bool dead = false;
	public bool killable = false;
	bool doMovement = true;
    bool play = true;

    AudioSource enemySound;

    protected override void Start()
	{
        enemySound = gameObject.GetComponent<AudioSource>();

        game_controller = FindObjectOfType<NewGameCont>();

		origin_point = transform.position.y;
		start_pos = transform.position;
		myRenderer = gameObject.GetComponent<SpriteRenderer>();
		enemyAnim = GetComponent<Animator>();

		//purifiedSound = GetComponent<AudioSource>();

		reset();

		// add self to actor-list
		game_controller.addActor(this.gameObject);
	}

	// Update is called once per frame
	protected override void Update()
	{
		/*if(gameObject.tag == "Purified" && !dead) //please make a bool for when player collides with player after throwing cyrstal
        {
           // enemyAnim.SetBool("IsHugged", true);
            // enemyAnim.SetBool("Purify", true);
            // ^^^^ these should work here if you get that bool going - atsi

        }*/
		if (gameObject.tag == "PostPurified" && !dead)
			die();

        if (gameObject.tag == "Purified" /*&& constants.plyr_hit_purified*/)
        {
            if (play)
            {
                enemySound.Play();
                enemyAnim.SetBool("Purify", true);
                enemyAnim.SetBool("IsHugged", true);
                play = false;
            }

            if (enemySound.time >= 1.2f)
            {
                enemyAnim.SetBool("IsHugged", false);
                constants.plyr_hit_purified = false;
            }
        }

        if (doMovement)
		{
			if (movingUp)
			{
				this.gameObject.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
			}
			else
			{
				this.gameObject.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
			}
            // change position if we are too far from origin
            if (Mathf.Abs(transform.position.y - origin_point) > distance)
            {
                float newY = movingUp ? origin_point + distance : origin_point - distance;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
                movingUp = !movingUp;
            }
        }
		// enemy only stops when it's dead, so fly away
		else {
			Vector2 viewPos = Camera.main.WorldToViewportPoint(start_pos);
			flyAway(viewPos.y > 0.5);
		}
	}

	// resets the values of enemy to "respawn"
	protected override void reset()
	{
		hugged = false;
		my_state = constants.ObjState.Active;
		GetComponent<Collider2D>().enabled = true;
        enemyAnim = GetComponent<Animator>(); //to get rid of some weird erros in the editor ??
        killable = false;
		dead = false;
		tag = "Unpurified";
		//this.gameObject.SetActive(true);
		doMovement = true;
		movingUp = true;
		// reset start position (only if vector is empty)
		if (start_pos != Vector2.zero)
			transform.position = start_pos;
        enemyAnim.SetBool("Purify", false);
        play = true;
    }

	// do death aniimation and anything else before changing state
	protected override void die()
	{
		// add metric data
		if (!hugged)
		{
			hugged = true;
			FindObjectOfType<NewGameCont>().addHug();
		}

		FindObjectOfType<NewGameCont>().addHug();

		// disable collider
		GetComponent<Collider2D>().enabled = false;
		// stop movement
		doMovement = false;
		// DEATH ANIMATION
		enemyAnim.SetBool("IsHugged", false);
	}

	private void OnEnable()
	{
		reset();
	}

	// enemy flies away when purified
	private void flyAway(bool up)
	{
		if (up)
			this.gameObject.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
		else
			this.gameObject.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
		// if enemy off-screen set it's object-state to dead
		Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position);
		if (viewPos.y > 1 || viewPos.y < 0)
		{
			my_state = constants.ObjState.Dead;
		}
	}
}