using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovLeftRight : Actor_Base
{
	// bool needed for metric data
	bool hugged = false;

	// bool to tell us which way we are moving
	public bool movingRight = true;
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

	SpriteRenderer myRenderer;

	Animator enemyAnim;
	//AudioSource purifiedSound;

	bool dead = false;
	public bool killable = false;
    public bool doMovement = true;
    bool play = true;

    AudioSource enemySound;

    protected override void Start()
	{
        enemySound = gameObject.GetComponent<AudioSource>();

        game_controller = FindObjectOfType<NewGameCont>();

		origin_point = transform.position.x;
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
        //DONT TAKE THIS OUT!!! atsi (special case for sap enemy)
       enemySound = GetComponent<AudioSource>();

		if (gameObject.tag == "PostPurified" && !dead)
        {
            die();
            play = true;
        }

        if(gameObject.tag == "Purified" /*&& constants.plyr_hit_purified*/)
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

		//f(gameObject.tag == "Purified" && enemyAnim.
		/*if (dead && !purifiedSound.isPlaying) {
            enemyAnim.SetBool("IsHugged", false);

            // MAKE ENEMY MOVE AWAY HERE, will need to depend on if enemies are ground enemies or flying enemies
                //ground enemies need to just move left/right and leave
                    //flying enemies can just fly away top left / top right -atsi


            /*Destroy(gameObject);
			this.gameObject.SetActive(false);
			killable = true;
        }*/
		if (doMovement)
		{
			if (movingRight)
			{
				this.gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
				myRenderer.flipX = false;
			}
			else
			{
				this.gameObject.transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
				myRenderer.flipX = true;
			}
			// change position if we are too far from origin
			if (Mathf.Abs(transform.position.x - origin_point) > distance)
            {
                float newX = movingRight ? origin_point + distance : origin_point - distance;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                movingRight = !movingRight;
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
		my_state = constants.ObjState.Active;
		GetComponent<Collider2D>().enabled = true;
        enemyAnim = GetComponent<Animator>(); //to get rid of some weird erros in the editor ??
		killable = false;
		dead = false;
		hugged = false;
		tag = "Unpurified";
		//this.gameObject.SetActive(true); redundant because function is called when object is enabled, aka set active
		doMovement = true;
		movingRight = true;
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
		if (!hugged) {
			hugged = true;
			FindObjectOfType<NewGameCont>().addHug();
		}

		constants.enemy_dies = true;

        // disable collider
		GetComponent<Collider2D>().enabled = false;
		// stop movement
		doMovement = false;
        //play enemies dying animation before getting destroyed 
        //purifiedSound.Play();

        // DEATH ANIMATION
        enemyAnim.SetBool("IsHugged", false);
	}

	private void OnEnable()
	{
		reset();
	}

	// enemy flies away when purified
	private void flyAway(bool up) {
		if (up)
			this.gameObject.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
		else
			this.gameObject.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
		// if enemy off-screen set it's object-state to dead
		Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position);
		if (viewPos.y > 1 || viewPos.y < 0) {
			my_state = constants.ObjState.Dead;
		}
	}
}