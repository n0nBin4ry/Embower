using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSequenceTwo : MonoBehaviour
{
    public GameObject textBox;
    public Text textDisplay;

    public GameObject shiftButton;

    AudioSource textBeep;

    // Create a list to hold all of the texts I want to be displayed in this scene
    List<string> texts;

    bool changeText = false;

    void Start()
    {
        texts = new List<string>();
        textBeep = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && changeText)
        {
            textBeep.Play();
            // the box and text need to be hidden so that the player can see the spikes
            textDisplay.text = ""; //display no text
            textBox.SetActive(false); //hide the dialouge box
            shiftButton.SetActive(false);

            constants.g_player_paused = false; //unpause player
            constants.g_crystal_paused = false;

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            constants.g_player_paused = true; //so that the players cant do anything until shift is pressed!!!
            constants.g_crystal_paused = true;

            // Add all the dialouges
            texts.Add("Clay, can you avoid these spikes? \n I believe in you!");

            textDisplay.text = texts[0]; //set the text to be the first string in the array
            textBeep.Play();

            changeText = true;
            //set the shift button to be seen
            shiftButton.SetActive(true);
        }

        if (textBox.activeSelf == false)
        {
            textBox.SetActive(true);
        }
    }
}
