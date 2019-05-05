using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BeginGameScript : MonoBehaviour
{
    public string newGameScene;

    AudioSource textBeep;

    bool playSound = false;

    private void Start()
    {
        textBeep = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
        {
            playSound = true;
            textBeep.Play();
        }

        if(playSound)
        {
            if(!textBeep.isPlaying)
            {
                SceneManager.LoadScene(newGameScene);
            }
        }
    }
}
