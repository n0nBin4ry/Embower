using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour
{
    public ParticleSystem particles;

    public string newGameScene;

    public GameObject button1;
    public GameObject button2;
    public GameObject button3;

    public GameObject shiftButton;

    public GameObject creditsText;

    bool checkForShift = false;

    AudioSource textBeep;

    // Start is called before the first frame update
    void Start()
    {
        //start particle system ahead of its initial start time
        particles.Simulate(10);
        particles.Play();

        shiftButton.SetActive(false);

        textBeep = GetComponent<AudioSource>();

        creditsText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(checkForShift)
        {
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
            {
                textBeep.Play();
                button1.SetActive(true);
                button2.SetActive(true);
                button3.SetActive(true);
                checkForShift = false;
                creditsText.SetActive(false);
                shiftButton.SetActive(false);

                checkForShift = false;
            }
        }
    }

    public void startGame()
    {
        SceneManager.LoadScene(newGameScene);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void credits()
    {
        creditsText.SetActive(true);
        button1.SetActive(false);
        button2.SetActive(false);
        button3.SetActive(false);
        
        shiftButton.SetActive(true);
        checkForShift = true;
    }
}
