using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class NewGameCont : MonoBehaviour
{
	public GameObject player;
	protected Vector3 respawnPos;

	bool playerRespawned = false;

	Animator playerAnimator;

	// list of player and all other actors (besides crystal)
	List<GameObject> actors = new List<GameObject>();

	// METRICS-DEPENDENT VARS
	// metrics file writer
	MetricsFileWriter death_writer = new MetricsFileWriter();
	// filename of where to write the death data
	public string death_file_name = "";
	// current "floor"
	int curr_floor = 1;
	// current "room"
	int curr_room = 1;
	// keep track of hugs
	HugWriter hug_writer = new HugWriter();
	// filename of where to write the hug data
	public string hug_file_name = "";

	// Start is called before the first frame update
	void Start()
	{
		// give the file-writer the name of the scene as the file-name
		death_writer.filename = death_file_name;
		hug_writer.filename = hug_file_name;

		// cap framerate
		Application.targetFrameRate = 60;

		if (!player)
			Destroy(gameObject);
		// set default player spawn point
		respawnPos = player.transform.position;
		// set player animator
		playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update()
	{
        // check list of objects and see if any need to be destroyed
        for (int i = 0; i < actors.Count; i++)
		{
			if (actors[i].GetComponent<Actor_Base>().getState() == constants.ObjState.Dead)
			{
				// if player dies, reset level
				if (actors[i] == player) {
					// count death for metrics
					death_writer.addDeath(curr_floor, curr_room);
					// respawn player
					reset_level();
				}
				// else, diable dead actor
				else
					actors[i].SetActive(false);
			}
		}


		if (Input.GetKeyDown(KeyCode.Tab))
		{
			reset_level();
		}

		//to reload the whole game
		if (Input.GetKeyDown(KeyCode.R))
		{
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);
		}

        if(Input.GetKeyDown(KeyCode.Q))
        {
            playerAnimator.SetBool("IsDead", false);
            
        }

        if(Input.GetKeyDown(KeyCode.Escape)) //go back to title screen
        {
            SceneManager.LoadScene("TitleScreen");
        }
	}

	// set's current room for metric data
	public void setRoom(int floor, int room) {
		curr_room = room;
		curr_floor = floor;
	}

	public void addHug() {
		hug_writer.addHug();
		hug_writer.writeHug();
	}

	public void addActor(GameObject actor) { actors.Add(actor); }

	public void setRespawn(Vector2 Pos) { respawnPos = Pos; }

	protected void reset_level()
	{
		player.GetComponent<RaycastPlayerCont>().respawn(respawnPos);

		// re-activate actors
		for (int i = 0; i < actors.Count; i++)
			actors[i].SetActive(true);
	}
}
