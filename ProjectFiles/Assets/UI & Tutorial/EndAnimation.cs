using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndAnimation : MonoBehaviour
{
   // public GameObject animatedBackG;
   // public GameObject treeHeart;
   // public GameObject player;
    public string sceneToLoad;

    //FOR IMAGE FADING
    public Image blackImage;
    bool fadeImageIn = false;
    bool fadeImage = false;
    float fadeRate = 1;
    public float speed;
    float difference = 0.1f;
    bool fadeMusic = false;

    AudioSource bkgMusic;
    AudioSource layerTwo;

    void Start()
    {
        //set alpha of black image to 100, then 0
        Color blackImg = blackImage.color;
        blackImg.a = 1;
        blackImage.color = blackImg;
        fadeImageIn = true;

        var aSources = GameObject.Find("MusicController").GetComponents<AudioSource>();
        bkgMusic = aSources[1];
        if (aSources.Length >= 2)
            layerTwo = aSources[2];
        else
            layerTwo = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeImage)
        {
            if (fadeRate >= 1)
            {
                //load the end scene
                constants.g_player_paused = false; //unpause it before the next scene???
                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
            }
            else
                fadeIn();
        }
        

        if(fadeImageIn)
        {
            if (fadeRate <= 0)
            {
                fadeImageIn = false;
            }
            else
                fadeOut();
        }

        if (fadeMusic)
        {
            if (bkgMusic.volume <= 1)
                bkgMusic.volume = Mathf.Lerp(bkgMusic.volume, 0f, 4f * Time.deltaTime);
            if(layerTwo != null)
                layerTwo.volume = Mathf.Lerp(bkgMusic.volume, 0f, 4f * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) //when player triggers the door
    {
        //pause player and make image fade
        fadeImage = true;
        constants.g_player_paused = true;
        //begin to lower the volume of the music!!
        fadeMusic = true;
    }

    void fadeIn() //used for fading in the image
    {
        Color currColor = blackImage.color;

        fadeRate += difference * Time.deltaTime * speed;

        currColor.a = fadeRate;
        blackImage.color = currColor;
    }

    void fadeOut()
    {
        Color currColor = blackImage.color;

        fadeRate -= difference * Time.deltaTime * speed;

        currColor.a = fadeRate;
        blackImage.color = currColor;
    }
}
