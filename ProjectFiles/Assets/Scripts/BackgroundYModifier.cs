using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundYModifier : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
       if (player.transform.position.y > 7.5f) //7.5 is the y pos of the player where he is past the current level
        {
            Vector3 pos = transform.position;
            pos.y = 17f; //the y pos that puts it behind the second part of the level
            transform.position = pos;
            Background.setY = true;
        }
    }
}
