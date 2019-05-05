using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSapAndEnemy : MonoBehaviour
{
    Animator enemyAnim;
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemyAnim = transform.GetChild(2).GetComponent<Animator>();

        enemyAnim.SetBool("IsHugged", true);
        enemyAnim.Play("FlyPurified"); // we want this enemy to appear blue at first

        //disactiveate all the children
        for (int a = 0; a < transform.childCount-1; a++)
        {
            transform.GetChild(a).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //once the player enters the room, pause the player and reactivate the sap and enemy stuff
        constants.g_player_paused = true;
        constants.g_crystal_paused = true;

        for (int a = 0; a < transform.childCount; a++)
        {
            transform.GetChild(a).gameObject.SetActive(true);
        }
    }
}
