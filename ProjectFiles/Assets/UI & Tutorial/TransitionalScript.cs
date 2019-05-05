using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionalScript : MonoBehaviour
{
    List<string> texts;
    public string textOne = " ";
    public string textTwo = " ";
    public string textThree = " ";
    public string textFour = " ";
    public string textFive = " ";

    public Text textChange;

    int counter = 0;

    public GameObject shiftButton;
    public string sceneToLoad;
    public bool loadScene;

    AudioSource textBeep;

    void Start()
    {
        textBeep = GetComponent<AudioSource>();

        texts = new List<string>();

        // Set
        texts.Add(textOne);
        texts.Add(textTwo);
        texts.Add(textThree);
        if (textFour != " ")
            texts.Add(textFour);
        if (textFive != " ")
            texts.Add(textFive);

        textChange.text = texts[counter];
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            if (counter < texts.Count - 1) //to aovid out of bounds errors
            {
                counter++;
                textChange.text = texts[counter]; //set the text to be the first string in the array
                textBeep.Play();
            }
            else
            {
                if(loadScene)
                {
                    textChange.text = " ";
                    shiftButton.SetActive(false);

                    SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
                }
                else
                {
                    shiftButton.SetActive(false);
                }

                //figure out how to play a loading animation?????
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //go back to title screen
        {
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
