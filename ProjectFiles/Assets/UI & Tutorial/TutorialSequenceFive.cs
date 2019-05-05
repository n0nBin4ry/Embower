using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSequenceFive : MonoBehaviour
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

    public GameObject crystal;
    AudioSource textBeep;

    public GameObject button;

    void Start()
    {

        texts = new List<string>();

        // Add all the dialouges
        texts.Add("See that crystal? It's corrupted. Purify it and see what happens.");
        texts.Add("Woo! Perfect Job. You can purify any crystals to see whats hiding beneath them.");
        texts.Add(". . .");
        texts.Add("Clay, I'm tired now. Can I stop talking? I'm sure you can figure out what to do next.");
        texts.Add("Good luck! We are counting on you, clay!");

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
                Destroy(gameObject); //so text never shows again
            }
        }

        if(!useShift)
        {
            if(button.activeSelf) //if player got rid of the crystal
            {
                shiftButton.SetActive(true);
                crystal.SetActive(false);
                textBeep.Play();
                counter++;
                textDisplay.text = texts[counter];
                useShift = true;
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
            useShift = false;
            shiftButton.SetActive(false);

            //if textBox was set to false in previous sequence, set it to true here
            if (textBox.activeSelf == false)
            {
                textBox.SetActive(true);
            }
        }
    }
}
