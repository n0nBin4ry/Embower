using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sapTutorial : MonoBehaviour
{

    public GameObject textBox;
    public Text textDisplay;
    
    public GameObject shiftButton;
    AudioSource textBeep;

    bool useShift = false;
    int counter = 0;

    List<string> texts;

    public string textOne;
    public string textTwo;
    public string textThree;

    public bool hideBox;

    void Start()
    {
        texts = new List<string>();
        textBeep = GetComponent<AudioSource>();

        if(hideBox)
        {
            textBox.SetActive(false);
            textDisplay.text = " ";
            shiftButton.SetActive(false);
        }

        texts.Add(textOne);
        if(textTwo != "")
        {
            texts.Add(textTwo);
        }
        if(textThree != "")
        {
            texts.Add(textThree);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useShift && (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
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
                shiftButton.SetActive(false);

                // let the players be active again
                constants.g_player_paused = false;
                constants.g_crystal_paused = false;

                Destroy(gameObject);

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            constants.g_player_paused = true; //so that the players cant do anything until shift is pressed!!!
            constants.g_crystal_paused = true;

            textBox.SetActive(true);
            textDisplay.text = texts[counter];
            shiftButton.SetActive(true);
            useShift = true;
            textBeep.Play();
        }
        
    }
}
