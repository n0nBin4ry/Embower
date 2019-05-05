using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSequenceThree : MonoBehaviour
{
    public GameObject textBox;
    public Text textDisplay;

    public GameObject shiftButton;
    AudioSource textBeep;

    // Create a list to hold all of the texts I want to be displayed in this scene
    List<string> texts;

    //counter that is keeping track of what text we're using in the array
    int counter = 0;

    bool changeText = false;
    bool useShift = true;
    bool crystalCollision = false;

    public GameObject collider;

    void Start()
    { 
        texts = new List<string>();

        // Add all the dialouges
        texts.Add("Woah, whats that pink stuff? Try throwing your crystal at it.");
        texts.Add("Yikes, looks like that didn't work... I guess Gall's energy has creataed poisonous sap in the trees.");
        texts.Add("You'll have to find another way!");

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
            }
            else
            {
                //player went through all the texts, get rid of the box and text
                textBeep.Play();
                textDisplay.text = "";
                textBox.SetActive(false);
                Destroy(collider); //allow player to continue
                Destroy(gameObject); //destroy this so the text never appears again
            }
        }

        if(!useShift)
        {
            if(crystalCollision)
            {
                shiftButton.SetActive(true);
                useShift = true;
                textBeep.Play();
                counter++;
                textDisplay.text = texts[counter];
                crystalCollision = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Display the first dialouge
            textBeep.Play();
            textDisplay.text = texts[0]; //set the text to be the first string in the array
            changeText = true;

            useShift = false; //want the next text to display when crystal collides with a "hazzard" (sap)
            shiftButton.SetActive(false);

            //if textBox was set to false in previous sequence, set it to true here
            if (textBox.activeSelf == false)
            {
                textBox.SetActive(true);
            }
        }

        if(collision.tag == "crystal")
        {
            crystalCollision = true;
        }
    }
}
