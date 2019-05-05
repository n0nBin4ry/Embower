using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    AudioSource intro;
    AudioSource layerOne;
    AudioSource layerTwo;

    bool playLayerOne = true;
    bool playLayerTwo = false;
    bool playLayerThree = false;

    bool notStarted = true;

    public float timeToWait;

    // Start is called before the first frame update
    void Start()
    {
        var aSources = GetComponents<AudioSource>();
        intro = aSources[0];
        layerOne = aSources[1];
        if(aSources.Length > 2)
        {
            layerTwo = aSources[2];
            layerTwo.volume = 0;
        }
        else { layerTwo = null; }

        intro.Play();

    }

    // Update is called once per frame
    void Update()
    {
        if (playLayerOne)
        {
            if (intro.time >= timeToWait)
            {
                //start playing the next clip
                layerOne.Play();
                if(layerTwo != null)
                    layerTwo.Play(); //we're just gonna have it play in the background and set its volume
                playLayerOne = false;
                playLayerTwo = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            layerTwo.volume = 1;
        }
    }
}
