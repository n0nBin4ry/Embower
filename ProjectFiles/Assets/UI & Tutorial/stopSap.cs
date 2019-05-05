using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stopSap : MonoBehaviour
{

    public GameObject particleSaps;
    public GameObject pinkCrystal;

    public GameObject sapOne;
    public GameObject sapTwo;
    public GameObject sapThree;

    // Start is called before the first frame update
    void Start()
    {
        particleSaps.SetActive(false);

        gameObject.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!pinkCrystal.activeSelf || pinkCrystal.gameObject.tag == "Purified")
        {
            particleSaps.SetActive(true);

            sapOne.SetActive(false);
            sapTwo.SetActive(false);
            sapThree.SetActive(false);

            gameObject.GetComponent<Renderer>().enabled = true;
        }

        if(pinkCrystal.gameObject.tag == "Unpurified")
        {
            //crystal got reset cuz player died
            particleSaps.SetActive(false);

            sapOne.SetActive(true);
            sapTwo.SetActive(true);
            sapThree.SetActive(true);

            gameObject.GetComponent<Renderer>().enabled = false;
        }
    }
}
