/* dead
 * using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject player;
	protected Vector3 respawnPos;

    bool playerRespawned = false;

    Animator playerAnimator;

	GameObject[] enemies;

    // Start is called before the first frame update
    void Start()
	{
		if (!player)
			Destroy(gameObject);
		// keep object persistant
		//DontDestroyOnLoad(this.gameObject);
		// set default player spawn point
		respawnPos = player.transform.position;
		// set player animator
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
		// get list of all enemies (unpurified)
		enemies = GameObject.FindGameObjectsWithTag("Unpurified");
	}

	// Update is called once per frame
	void Update()
	{
		//if (!player)
			//Destroy(gameObject); //this is where that nasty bug is taking place??? when the player is destroyed the game controller goes along with it...
								 //but why is the player being destroyed?? 
								// - no, this is when there is a copy of the game controller without a reference to the player - but no longer needed since I redid respawning

        if (player.GetComponent<APlayerController>().isDead() || Input.GetKeyDown(KeyCode.Tab))
		{
			reset_level();
		}

        //to reload the whole game
        if (Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        if (playerRespawned && player.transform.position == respawnPos) //if player was respawned the player was moved to the respawn pos (which happens at the end of anim)
        {
            playerAnimator.SetBool("IsDead", false);
            player.GetComponent<APlayerController>().setDead(false);
            playerRespawned = false; //make sure not to check again even if player goes back to that respawn pos, cuz they arent actually dead
            constants.g_player_paused = false; //unpause the player so that they can move around and stuff again
        }

		// disable killable enemies
		for (int i = 0; i < enemies.Length; i++) {
			// TODO: **RICHARD** do this in a cleaner way when you dont feel like youre dying
			EnemUpDown temp_updown = enemies[i].GetComponent<EnemUpDown>();
			EnemyMovLeftRight temp_leftright = enemies[i].GetComponent<EnemyMovLeftRight>();
			if (temp_updown && temp_updown.killable)
				enemies[i].SetActive(false);
			if (temp_leftright && temp_leftright.killable)
				enemies[i].SetActive(false);
		}
		
	}

	public void setRespawn(Vector2 Pos) { respawnPos = Pos; }

	protected void reset_level() {
		//player.transform.position = respawnPos; //set the player to their respawn pos
		player.GetComponent<APlayerController>().respawn(respawnPos);
		playerRespawned = true; // TO ATSI: please add a note to any new things you add

		// re-activate enemies
		for (int i = 0; i < enemies.Length; i++)
		{
			// TODO: **RICHARD** do this in a cleaner way when you dont feel like youre dying
			EnemUpDown temp_updown = enemies[i].GetComponent<EnemUpDown>();
			EnemyMovLeftRight temp_leftright = enemies[i].GetComponent<EnemyMovLeftRight>();
			if (temp_updown)
			{
				temp_updown.reset();
			}
			if (temp_leftright)
			{
				temp_leftright.reset();
			}
			enemies[i].SetActive(true);
		}
	}
}
*/