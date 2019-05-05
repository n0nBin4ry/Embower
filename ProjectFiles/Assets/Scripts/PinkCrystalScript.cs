using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkCrystalScript : Actor_Base
{
    public GameObject button = null;

    Animator crystalAnim;
    AudioSource purifiedSound;

    bool dead = false;

    public bool killable = false;

    bool check = true;

    public bool runText = false;

    bool playSound = true;

    bool runAnim = true;

    // Create a list to hold all of the texts I want to be displayed in this scene
    List<string> texts;

    override protected void Start()
    {
        crystalAnim = GetComponent<Animator>();

        purifiedSound = GetComponent<AudioSource>();

		// add self to game controller's actor list
		NewGameCont game_cont = FindObjectOfType<NewGameCont>();
		game_cont.addActor(this.gameObject);
    }

    // Update is called once per frame
    override protected void Update()
    {
        if (button != null && check) //moved this here because if i disabeled the button too quickly, the particle system that
            //comes from the script would not be told to stop
        {
            button.SetActive(false); //hide the button that is hiding behind the pink crystal
            check = false;
        }

        if (gameObject.tag == "PostPurified" && !dead)
        {
            die(); // kill the crystal
        }

        if(gameObject.tag == "Purified" && constants.plyr_hit_purified && playSound)
        {
            crystalAnim.SetBool("IsPurify", true);
            purifiedSound.Play();
            playSound = false;
        }

        if (!playSound && !purifiedSound.isPlaying && runAnim)
        {
            crystalAnim.SetBool("IsPurified", true); //set the crystal to its purified animation
            runAnim = false;

        }
    }

    // resets the values of enemy to "respawn"
    protected override void reset()
    {
		my_state = constants.ObjState.Active;
		killable = false;
        dead = false;
        tag = "Unpurified";
        crystalAnim = GetComponent<Animator>(); //to get rid of some weird erros in the editor ??
        crystalAnim.SetBool("IsPurify", false);
        crystalAnim.SetBool("IsPurified", false);
        runAnim = true;
        playSound = true;
        //button.SetActive(false);
        /*BoxCollider2D[] bc = GetComponents<BoxCollider2D>();
		for (int i = 0; i < bc.Length; i++)
			bc[i].enabled = true;*/
        //this.gameObject.SetActive(true);
    }

	protected void OnEnable()
	{
		reset();
	}

	protected override void die() {
		/*BoxCollider2D[] bc = GetComponents<BoxCollider2D>();
		for (int i = 0; i < bc.Length; i++)
			bc[i].enabled = false;*/

		if (button != null)
		{
			button.SetActive(true);
		}
        dead = true;
		my_state = constants.ObjState.Dead;
	}
}
