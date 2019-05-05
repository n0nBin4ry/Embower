using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeHeartEnd : MonoBehaviour
{
    private IEnumerator coroutine;

    public ParticleSystem particleS;
    public Material blueMat = null;
    public GameObject player;

    Animator heartAnim;
    Animator bkgPurifyAnim;
    AudioSource purifiedSound;
    AudioSource purifyWholeTree;

    public GameObject textBox;
    public Text textDisplay;

    public GameObject shiftButton;
    public GameObject purifyBkg;
    public GameObject treeBackground;
    public GameObject flowers;

    bool playSound = true;

    public bool killable = false;
    bool useShift = false;

    // Create a list to hold all of the texts I want to be displayed in this scene
    List<string> texts;
    public string textOne = " ";
    public string textTwo = " ";
    public string textThree = " ";
    public string textFour = " ";
    public string textFive = " ";
    public bool boxesActive;
    

    int counter = 0;

    void Start()
    {
        constants.g_player_paused = false; //unpause player from the opening sequence
        coroutine = PlayAnimation();

        texts = new List<string>();

        // Get
        heartAnim = GetComponent<Animator>();
        purifiedSound = GetComponent<AudioSource>();
        bkgPurifyAnim = purifyBkg.GetComponent<Animator>();
        purifyWholeTree = flowers.GetComponent<AudioSource>();
        purifyBkg.SetActive(false);
        treeBackground.SetActive(false);
        flowers.SetActive(false);

        // Set
        texts.Add(textOne);
        texts.Add(textTwo);
        texts.Add(textThree);
        if (textFour != "")
            texts.Add(textFour);
        if (textFive != "")
            texts.Add(textFive);

        constants.g_crystal_paused = false;

        if(!boxesActive)
        {
            textBox.SetActive(false);
            shiftButton.SetActive(false);
            textDisplay.text = "";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!useShift) //not time to display stuff yet
        {
            if (gameObject.tag == "Purified" && playSound && constants.plyr_hit_purified)
            {
                heartAnim.SetBool("IsPurify", true);
                purifiedSound.Play();
                playSound = false;
            }

            if (!playSound && !purifiedSound.isPlaying)
            {
                if (particleS != null)
                {
                    //change its color to blue
                    particleS.GetComponent<ParticleSystemRenderer>().material = blueMat;

                    heartAnim.SetBool("IsPurified", true);
                    purifyBkg.SetActive(true);
                    bkgPurifyAnim.Play("bkgPurify");
                    StartCoroutine(PlayAnimation());
                }
                
                this.GetComponent<Collider2D>().enabled = false; //allow players to walk thru the crystal
                useShift = true;
                shiftButton.SetActive(true);
                textBox.SetActive(true);
                textDisplay.text = texts[counter];
                counter++;
                //blueCrystal.SetActive(true); //set the blue crystal to true because this current one will go bye bye but we still want the visual of the crystal

                //player needs to be able to walk thru the "pink" crystal, no collisions
                //Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), player.GetComponent<Collider2D>(), false);
            }
        }
        else //time to use shift
        {
            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)))
            {
                if (counter < texts.Count) //to aovid out of bounds errors
                {
                    textDisplay.text = texts[counter]; //set the text to be the first string in the array
                    counter++;
                }
                else
                {
                    textDisplay.text = " ";
                    textBox.SetActive(false);
                    shiftButton.SetActive(false);
                    foreach (Collider2D c in GetComponents<Collider2D>())
                    {
                        Physics2D.IgnoreCollision(c, player.GetComponent<Collider2D>(), false);
                    }
                }
            }
        }
        
    }

    // resets the values of enemy to "respawn"
    public void reset()
    {
        killable = false;
        tag = "Unpurified";
        this.gameObject.SetActive(true);
    }

    private IEnumerator PlayAnimation()
    {
        yield return new WaitForSeconds(0.2f);
        flowers.SetActive(true);
        purifyWholeTree.Play();
        yield return new WaitForSeconds(0.6f); // 1 second ?
        treeBackground.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        Destroy(purifyBkg);
        //display the tree background and all other visuals
    }

}
