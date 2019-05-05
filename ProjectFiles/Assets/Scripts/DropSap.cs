using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSap : MonoBehaviour
{
    public int numOfDrops;

    public GameObject sapDrop;

    int counter = 0;

    public float waitTime;
    private float timer = 0.0f;

    Vector2 startPosOfDrop;

    // Start is called before the first frame update
    void Start()
    {
        startPosOfDrop = sapDrop.gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (timer > waitTime)
        {
            if (counter < numOfDrops)
            {
                Instantiate(sapDrop, startPosOfDrop, transform.rotation);
                counter++;
            }

            // Remove the recorded 2 seconds.
            timer = timer - waitTime;
        }
    }
}
