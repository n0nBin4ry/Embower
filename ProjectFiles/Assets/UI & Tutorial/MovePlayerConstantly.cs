using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MovePlayerConstantly : MonoBehaviour
{
    Animator myAnimator;

    bool walkForward = true;
    bool changeCamera = false;
    bool changeCamPos = false;
    bool foundCrystal = false;
    bool getRidOfBox = false;
    bool fadeImage = false;
    bool fadeImageIn = true;
    bool fadeMusic = false;

    public Image blackImage;
    float fadeRate = 1;
    public float speed;
    float difference = 0.1f;

    public Camera mainCam;

    public GameObject uiBox;

    public Text uiText;

    List<string> texts;

    List<string> crystalTexts;
    public bool createCrystalTexts;

    int counter = 0;

    public string textOne = "";
    public string textTwo = "";
    public string textThree = "";
    public string textFour = "";

    public string sceneToLoad = "";

    public GameObject tutorialUi;
    public GameObject particleE;
    public GameObject colliderr;

    AudioSource findingCrystal;
    AudioSource bkgMusic;

    void Start()
    {
        var aSources = GetComponents<AudioSource>();
        bkgMusic = aSources[0];
        if (aSources.Length >= 2)
        {
            findingCrystal = aSources[1];
        }
        else { findingCrystal = null; }

        if (createCrystalTexts)
        {
            crystalTexts = new List<string>();
            crystalTexts.Add("Hey. . .");
            crystalTexts.Add("I can't help but notice that crystal over there! What is it? It's so shiny.");
            crystalTexts.Add("Let's pick it up!");
            crystalTexts.Add("Woah! It seems to be sticking to you. Maybe we can use it somehow. . .?");
        }

        texts = new List<string>();

        texts.Add(textOne);
        texts.Add(textTwo);
        texts.Add(textThree);
        if(textFour != "")
        {
            texts.Add(textFour);
        }

        uiBox.SetActive(false);
        uiText.text = " ";

        myAnimator = gameObject.GetComponent<Animator>();
        constants.isThrown = true; //the player does not have the crystal yet!!!

        mainCam.orthographicSize = 4.24f;

        //set alpha of black image to 0
        Color blackImg = blackImage.color;
        blackImg.a = 0;
        blackImage.color = blackImg;
    }

    // Update is called once per frame
    void Update()
    {
        constants.g_crystal_paused = true;
        if (walkForward)
        {
            //no longer move player forward automatically
        }
        else //shift sequence
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) //both shifts now haha
            {
                // show the text for when player finds crystal!!!
                if(foundCrystal)
                {
                    if (getRidOfBox)
                    {
                        counter = 0; //reset the counter!!!!
                        walkForward = true;
                        uiBox.SetActive(false);
                        uiText.text = " ";
                        constants.g_player_paused = false;
                        foundCrystal = false;

                        //bring back the movement ui
                        if (tutorialUi != null)
                        {
                            tutorialUi.SetActive(true);
                        }
                    }
                    else
                    {
                        if (counter < crystalTexts.Count - 1) //to aovid out of bounds errors
                        {
                            uiText.text = crystalTexts[counter];
                            counter++;
                        }
                        else //player reached rest of texts
                        {
                            walkForward = true;
                            uiBox.SetActive(false);
                            uiText.text = " ";
                            constants.g_player_paused = false;

                            //bring back the movement ui
                            if (tutorialUi != null)
                            {
                                tutorialUi.SetActive(true);
                            }
                        }

                    }
                   
                }
                else
                {
                    changeCamera = true;
                    if (changeCamera)
                    {
                        changeCamera = false; //will never run this if statement again
                        mainCam.orthographicSize = 20.6806f;
                        changeCamPos = true;
                        constants.pause_cam = true; //pause cam so updates here will work
                    }

                    if (counter < texts.Count) //to aovid out of bounds errors
                    {
                        uiText.text = texts[counter];
                        counter++;
                    }
                    else //the end of the texts was reached, move camera back and make player go forward
                    {
                        walkForward = true;
                        mainCam.orthographicSize = 4.24f;
                        changeCamPos = false;
                        uiBox.SetActive(false);
                        uiText.text = " ";
                        constants.pause_cam = false; //unpause cam so it goes back to normal

                        //unpause the player!!
                        constants.g_player_paused = false;

                        //bring back the movement ui
                        if (tutorialUi != null)
                        {
                            tutorialUi.SetActive(true);
                        }
                    }
                }
            }
        }

        if (fadeMusic)
        {
            if (bkgMusic.volume <= 1)
                bkgMusic.volume = Mathf.Lerp(bkgMusic.volume, 0f, 4f * Time.deltaTime);
        }

        if (changeCamPos)
        {
            //change the pos of the camera
            Vector3 pos = new Vector3(30.9f, 16.6f, -10f);
            mainCam.transform.position = pos;
        }

        if (fadeImage)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            walkForward = false;
            uiBox.SetActive(true);
            uiText.text = texts[0];
            counter++;

            //pause the player!!!!
            myAnimator.SetBool("isIdle", true);
            constants.g_player_paused = true;
            
            collision.tag = "Shredder"; //just to make sure this will never triger again

            //set the tutorial ui to be false so people cant see it
            if (tutorialUi != null)
            {
                tutorialUi.SetActive(false);
            }
        }

        if(collision.tag == "Respawn")
        {
            // MAKE IMAGE FADE TO BLACK BEFORE LOADING NEXT SCENE
            fadeImage = true;
            //pause the player
            constants.g_player_paused = true;
            //make the music start to fade!!!
            fadeMusic = true;
        }

        if(collision.tag == "Text") //untagged???
        {
            walkForward = false;
            foundCrystal = true; //to display the crystal texts!
            uiBox.SetActive(true);
            uiText.text = crystalTexts[0];
            counter++;
        

            //pause the player!!!!
            constants.g_player_paused = true;
            collision.tag = "Shredder"; //just to make sure this will never triger again

            //set the tutorial ui to be false so people cant see it
            if (tutorialUi != null)
            {
                tutorialUi.SetActive(false);
            }
        }
        if(collision.tag == "crystal")
        {
            // Text stuff
            walkForward = false;
            uiBox.SetActive(true);
            getRidOfBox = true;
            uiText.text = crystalTexts[counter];
            counter++;


            //pause the player!!!!
            constants.g_player_paused = true;

            //set the tutorial ui to be false so people cant see it
            if (tutorialUi != null)
            {
                tutorialUi.SetActive(false);
            }


            Destroy(collision.gameObject); //destroy the place holder crystal there
            Destroy(particleE); // no more partcle effects yeet
            Destroy(colliderr); //allow player to move forward

            //play finding crystal sound here!
            findingCrystal.Play();

            constants.isThrown = false; //bring the crystal animation to clay back
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