using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSequenceFour : MonoBehaviour
{
    public GameObject textBox;
    public Text textDisplay;

    public GameObject previousSequence;
    public GameObject shiftButton;

    // Create a list to hold all of the texts I want to be displayed in this scene
    List<string> texts;

    //counter that is keeping track of what text we're using in the array
    int counter = 0;

    bool changeText = false;
    bool useShift = true;
    bool crystalHitEnem = false;

    public GameObject enemy;
    public GameObject collider;

    AudioSource textBeep;

    void Start()
    {

        texts = new List<string>();

        // Add all the dialouges
        texts.Add("Phew, good job Clay! You're doing well ---");
        texts.Add("What's that? A corrupted animal? EEK!\nQuick, throw your crystal at it!");
        texts.Add("Now hug it, that's the only way you can purify it with the crystal on your stomach!");
        texts.Add("Phew, he's purified now! Gravitate elsewhere or jump to let him go.");
        texts.Add("Huff... huff... Wow, you made that look so easy Clay! I bet you're ready for anything.");

        textBeep = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && changeText && useShift)
        {
            if (counter < texts.Count - 1) //to aovid out of bounds errors
            {
                textBeep.Play();
                counter++;
                textDisplay.text = texts[counter]; //set the text to be the first string in the array

                if(counter == 1)
                {
                    useShift = false;
                    shiftButton.SetActive(false);
                }
            }
            else
            {
                //player went through all the texts, get rid of the box and text
                textBeep.Play();
                textDisplay.text = "";
                textBox.SetActive(false);

                Destroy(collider); //allow player to now walk forward
                Destroy(gameObject); //stop text from ever showing again
            }
        }

        if(!useShift)
        {
            if(counter == 1)
            {
                //when crystal collides with "unpurified" (ENEMY)
                if (constants.crys_hit_unpurified)
                {
                    textBeep.Play();
                    counter++;
                    textDisplay.text = texts[counter];
                }
            }

            if(counter == 2)
            {
                //when player collides with the crystal (WHEN ITS ON THE ENEMY)
                if(constants.plyr_hit_purified)
                {
                    textBeep.Play();
                    counter++;
                    textDisplay.text = texts[counter];
                }
            }

            if(counter == 3)
            {
                if(constants.enemy_dies) //player killed enemy
                    //WILL NEED TO BE CHANGED TO MATCH WHEN ENEMY JUST FLIES OFF SCREEN
                {
                    textBeep.Play();
                    counter++;
                    textDisplay.text = texts[counter];
                    useShift = true;
                    shiftButton.SetActive(true);
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // destroy the string sequence before this one, since we don't need it anymore
            Destroy(previousSequence);

            // Display the first dialouge
            textDisplay.text = texts[0]; //set the text to be the first string in the array
            changeText = true;
            shiftButton.SetActive(true);

            //if textBox was set to false in previous sequence, set it to true here
            if (textBox.activeSelf == false)
            {
                textBox.SetActive(true);
            }
        }
    } 
}
