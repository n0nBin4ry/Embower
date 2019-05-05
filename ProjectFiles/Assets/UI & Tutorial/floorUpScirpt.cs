using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorUpScirpt : MonoBehaviour
{
    public GameObject pinkCrystal = null;
    Vector3 floorPos;
    Vector3 startPos;

    Animator enemyAnim;
    AudioSource purifiedSound;

    void Start()
    {
        // Get
        enemyAnim = GetComponent<Animator>();
        purifiedSound = GetComponent<AudioSource>();

        // Set
        floorPos = transform.position;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if ((pinkCrystal.tag == "Purified" && constants.plyr_hit_purified) || pinkCrystal.tag == "PostPurified") //if player hugged the crystal
        {
                if (floorPos.y < -0.5f)
                {
                    floorPos.y += 0.08f;
                    transform.position = floorPos;
                }
            
        }

        //in the case that the player dies we need to reset the level
        if(pinkCrystal.tag == "Unpurified")
        {
            transform.position = startPos;
            floorPos = startPos;
        }

    }
}
