using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialSequenceOne : MonoBehaviour
{
    public GameObject textBox;
    public Text textDisplay;

    Animator myAnimator;

    // Create a list to hold all of the texts I want to be displayed in this scene
    List<string> texts;

    //counter that is keeping track of what text we're using in the array
    int counter = 0;

    bool useShift = true;
    bool crystalCollided = false;
    bool playerCollided = false;

    public GameObject crystal;
    public GameObject player;
    public GameObject shiftButton;

    AudioSource textBeep;

    public GameObject collider;

    void Start()
    {
        texts = new List<string>();

        // Add all the dialouges
        texts.Add("Hm. . . What should we do with that crystal we found? Here, I think I have an idea!");                 //0
        texts.Add("Move your mouse to move me (the bird) around the screen. You will use me to aim your throw!");                                       //1
        texts.Add("Next, when I'm in the right position, left click on your mouse to throw the crystal. Try throwing it at the ceiling.");     //2
        texts.Add("Awesome! Lets wait for it to come back to you. As you can see, this takes a second.");                                      //3
        texts.Add("Let's throw it at the ceiling again.");                                                                                      //4
        texts.Add("Now that its stuck, activate the magnetic field of your stomach crystal by holding right click. Then you'll gravitate towards the thrown crystal.");      //5
        texts.Add("Amazing! Once you've made contact with the thrown crystal, you will stick to the ceiling until you get tired.");         //6
        texts.Add("During this time, you can wait to fall, jump to let go, or throw your crystal again!");                                                     //7
        texts.Add("Try throwing your crystal to the ground while you're on the ceiling. When you feel ready, let's go forward!");                              //8

        textDisplay.text = texts[0]; //set the text to be the first string in the array

        //set sound to play when text is changed
        textBeep = GetComponent<AudioSource>();
        myAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();

        constants.g_crystal_paused = false;

        textBox.SetActive(true);
        shiftButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && useShift)
        {
            shiftButton.SetActive(true);
            if (counter < texts.Count - 1) //to aovid out of bounds errors
            {
                textBeep.Play();
                counter++;
                textDisplay.text = texts[counter]; //set the text to be the first string in the array

                if (counter == 2) //the 3rd text, it should wait until the player throws the crystal at the ceiling
                {
                    useShift = false;
                }

                if(counter == 4)
                {
                    useShift = false;
                }
            }
            else
            {
                //player went through all the texts, get rid of the box and text
                textBeep.Play();
                textDisplay.text = "";
                textBox.SetActive(false);
                Destroy(collider); //destroy the collider so player can now move forward
                Destroy(gameObject); //destroy this gameobject so the text never prompts again
            }
        }

        if (!useShift)
        {
            shiftButton.SetActive(false);

            if (counter == 2)
            {
                if(crystalCollided)
                {
                    useShift = true;
                    shiftButton.SetActive(true);
                    textBeep.Play();
                    counter++;
                    textDisplay.text = texts[counter];
                    crystalCollided = false;
                }
                
            }

            if(counter == 4)
            {
                if(crystalCollided)
                {
                    textBeep.Play();
                    counter++;
                    textDisplay.text = texts[counter];
                    crystalCollided = false;
                }
            }

            if(counter == 5)
            {
                constants.g_crystal_paused = true;

                myAnimator.SetBool("IsThrowing", false);
                myAnimator.SetBool("IsThrown", true);

                //if player collides with the CEILING, move on... //ceiling because cyrstal collides with player when it comes back, so we'll do a ceiling check
                if (playerCollided)
                {
                    constants.g_crystal_paused = false;
                    //crystal.GetComponent<CystalScript>().setStuck(false); //make the crystal go back to the player!! bug fix
                    textBeep.Play();
                    counter++;
                    textDisplay.text = texts[counter];

                    //SPECIAL CASE, need to fix player animation here....
                    myAnimator.SetBool("IsThrown", false);
                }
            }

            if(counter == 6)
            {
                useShift = true;
                shiftButton.SetActive(true);
                textBeep.Play();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "crystal" && (counter == 2 || counter == 4))
        {
            crystalCollided = true;
        }
        if(collision.tag == "Player" && counter == 5)
        {
            playerCollided = true;
            //SPECIAL CASE
            myAnimator.SetBool("IsGravitating", false); // no longer gravitating
        }
    }
}
