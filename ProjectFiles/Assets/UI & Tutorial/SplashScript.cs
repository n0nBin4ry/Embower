using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScript : MonoBehaviour
{

    float fadeRate;
    public float speed;

    public Image uscLogo;
    public Image berkleeLogo;

    float difference = 0.1f;

    Image currentImage;

    bool setRate = true;
    bool faOut = false;

    public string newGameScene;

    bool loadNextScene = false;
    // Start is called before the first frame update
    void Start()
    {
        Color berkleeColor = berkleeLogo.color;
        berkleeColor.a = 0;
        berkleeLogo.color = berkleeColor;

        currentImage = uscLogo;

        fadeRate = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeRate >= 1)
        {
            faOut = true;
        }
        else
        {
            if(!faOut)
                fadeIn();
        }

        if(faOut)
        {
            fadeOut();
        }
    }

    void fadeIn()
    {
        Color currColor = currentImage.color;

        fadeRate += difference * Time.deltaTime * speed;

        currColor.a = fadeRate;
        currentImage.color = currColor;
    }

    void fadeOut()
    {
        Color currColor = currentImage.color;

        fadeRate -= difference * Time.deltaTime * speed;

        currColor.a = fadeRate;
        currentImage.color = currColor;

        if(currColor.a <= 0)
        {
            currentImage = berkleeLogo;
            fadeRate = 0;
            faOut = false;

            if(loadNextScene)
            {
                SceneManager.LoadScene(newGameScene);
            }
            loadNextScene = true;
        }
    }
}
