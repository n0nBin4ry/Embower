using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PromptGall : MonoBehaviour
{
    public GameObject textBox;
    public GameObject shiftButton;

    public Image blackImage;
    bool fadeImageIn = false;
    bool fadeImage = false;
    float fadeRate = 1;
    public float speed;
    float difference = 0.1f;

    //public GameObject gall;

    public Text textDisplay;
    List<string> texts;

    bool showText = false;

    int counter = 0;

    AudioSource textBeep;

    // Start is called before the first frame update
    void Start()
    {
        // STOP ALL THE OTHER BACKGROUND MUSIC PLAYING

        texts = new List<string>();

        //textDisplay.text = "";
        textBox.SetActive(false);

        texts.Add(". . . Huh? You hugged it, right?");
        texts.Add("Why didn't your hug work? It's always worked!");
        texts.Add(". . .");
        texts.Add("That would be because of me."); //CHANGE THE TEXT COLOR HERE!!!
        //texts.Add(" ");

        textBeep = GetComponent<AudioSource>();
        //gall.SetActive(false); // hide him for now

        constants.g_crystal_paused = false;

        //set alpha of black image to 100, then 0
        Color blackImg = blackImage.color;
        blackImg.a = 1;
        blackImage.color = blackImg;
        fadeImageIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (tag == "PostPurified" && !showText) //clay tried to purify it
        {
            textBox.SetActive(true);
            shiftButton.SetActive(true);
            showText = true;
            textDisplay.text = texts[counter];
        }

        if (showText)
        {
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
            {
                if (counter < texts.Count - 1) //to aovid out of bounds errors
                {
                    textBeep.Play();
                    counter++;
                    if (counter == 3)
                    {
                        textDisplay.color = Color.magenta;
                    }
                    textDisplay.text = texts[counter]; //set the text to be the first string in the array

                }
                else
                {
                    //player went through all the texts, get rid of the box and text
                    textBeep.Play();
                    textDisplay.text = "";
                    textBox.SetActive(false);

                    // FADE THE IMAGE TO BLACK SLOWLY
                    fadeImage = true;

                    //pause player and crystal so that player cant do anything anymore lol
                    constants.g_player_paused = true;
                    constants.g_crystal_paused = true;
                }
            }
        }

        if (fadeImage)
        {
            if (fadeRate >= 1)
            {
                //load the end scene
                constants.g_player_paused = false; //unpause it before the next scene???
                SceneManager.LoadScene("EndScene");
            }
            else
                fadeIn();
        }


        if (fadeImageIn)
        {
            if (fadeRate <= 0)
            {
                fadeImageIn = false;
            }
            else
                fadeOut();
        }
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
