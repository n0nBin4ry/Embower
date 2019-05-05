using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NarrativeOpening : MonoBehaviour
{
    List<string> texts;
    List<Sprite> images;

    public Text textChange;

    int counter = 0;
    int imageCounter = 0;

    public GameObject shiftButton;
    public string sceneToLoad;

    AudioSource textBeep;

    //load the images...
    public Image imageTochange;

    public Sprite imageOne;
    public Sprite imageTwo;
    public Sprite imageThree;
    public Sprite imageFour;
    public Sprite imageFive;
    public Sprite imageSix;
    public Sprite imageSeven;
    public Sprite imageNine;

    // Start is called before the first frame update
    void Start()
    {
        textBeep = GetComponent<AudioSource>();

        // ADD TEXTS
        texts = new List<string>();

        texts.Add("This is the world Ustrianata. Here, the trees emit from their roots and branches a special regenerative energy.");
        texts.Add("It shines a bright blue, and is hard to miss by anyone walking by.");
        texts.Add("There are three trees that create the energy and supply it to all other trees. They are known as the GIVING TREES.");
        texts.Add("All in this world know of the trees. THE SATRI have learned how to harvest their energy to be used in" +
            " foods, drinks, and clothes for its special healing abilities.");
        texts.Add("The Satri have never experienced famine or war, for the trees supply them all they need. All was well for them, until one day. . .");
        texts.Add("A Satri named GALL ADELGID took the energy from the trees and corrupted it. He created an energy that was made to kill.");
        texts.Add("Knowing of the Giving Trees, Gall took the corrupted energy and forced it into the heart of each one, and they began to emit it. . .");
        texts.Add("The new energy began to corrupt all nearby animals, and the Satri soon found that something was wrong with the trees. The world was in chaos.");
        texts.Add("But at the moment when all hope seemed lost. . .");
        texts.Add("A mysterious boy named CLAY appeared before the Satri, and showed them his special power. . .");
        texts.Add("Clay was born with a crystal on his stomach. It had always shined the same bright blue color of the tree's energy.");
        texts.Add("He never knew of its significance, until a corrupted animal tried to attack him. It touched his crystal, became purified, and ran away!");
        texts.Add("Clay then knew. . . He was born with the regenerative energy of the trees inside of him, and it created the crystal on his stomach." +
            " Anything he hugs will be purified.");
        texts.Add("Being the only one with this power, Clay set off on an adventure with his bird companion, Aimee, to save the Giving Trees.");
        texts.Add("If Clay hugs all three of their hearts, they will be purified and all shall be well again.");
        texts.Add("So, Clay set out on his adventure- to Tree Sequoia, the first Giving Tree.");


        textChange.text = texts[imageCounter];

        // ADD IMAGES
        images = new List<Sprite>();

        images.Add(imageOne);
        images.Add(imageTwo);
        images.Add(imageThree);
        images.Add(imageFour);
        images.Add(imageFive);
        images.Add(imageSix);
        images.Add(imageSeven);
        images.Add(imageNine);

        imageTochange.sprite = images[counter];
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

                if(counter == 2)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }
                if(counter ==3)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }
                if(counter == 5)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }
                if(counter == 7)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }

                if(counter == 10)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }

                if(counter == 11)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }
                if(counter == 13)
                {
                    imageCounter++;
                    imageTochange.sprite = images[imageCounter];
                }
            }
            else
            {
                textChange.text = " ";
                shiftButton.SetActive(false);

                SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);

                //figure out how to play a loading animation?????
            }
        }
    }
}
