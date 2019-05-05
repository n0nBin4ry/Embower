using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sapEnemyScript : MonoBehaviour
{
    Animator enemyAnim;

    AudioSource playerDeathSound;
    EnemyMovLeftRight enemyMove;
    public BoxCollider2D colliderr;
    EnemyMovLeftRight enemyScript;

    bool checkPurify = false;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyAnim = GetComponent<Animator>();
        enemyAnim.SetBool("IsHugged", true);
        enemyAnim.Play("FlyPurified"); // we want this enemy to appear blue at first

        playerDeathSound = GetComponent<AudioSource>();

        enemyScript = this.GetComponent<EnemyMovLeftRight>();
    }

    // Update is called once per frame
    void Update()
    {
        if(checkPurify)
        {
            if(!playerDeathSound.isPlaying)
            {
                enemyAnim.Play("FlyEnemy");
                constants.g_player_paused = false;
                constants.g_crystal_paused = false;
                Destroy(colliderr);
                checkPurify = false;
                Destroy(playerDeathSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Hazard" && !checkPurify) //it collided with one of the saps
        {
            enemyAnim.Play("FlyPurify");
            checkPurify = true;
            playerDeathSound.Play();
            this.GetComponent<Collider2D>().isTrigger = false; //make it a regular collider again
            //delete its rigid body
            Destroy(this.GetComponent<Rigidbody2D>());

            //need to pause the enemy!!! RICHARd help???
            //enemyMove.doMovement = false;
        }
    }
}
