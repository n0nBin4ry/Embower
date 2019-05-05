using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float backgroundSize;
    public float paralaxSpeed;
    public GameObject player; //background will move with player, so need player position

    private Transform[] layers;
    private float viewZone = 2;
    private int leftIndex;
    private int rightIndex;
    private float lastPlayerX;

    public static bool setY = false;

    void Start()
    {
        lastPlayerX = player.transform.position.x;
        layers = new Transform[transform.childCount]; //will get the child count
        //each background parent object rn has 3 children under it that represent the background
        for(int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }

        leftIndex = 0;
        rightIndex = layers.Length - 1;

        setY = false;
    }

    void ScrollLeft()
    {
        int lastRight = rightIndex;
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backgroundSize);

        if (setY)
        {
            Vector3 pos = layers[rightIndex].position;
            pos.y = 17f + 3.48f; //the y pos that puts it behind the second part of the level
            layers[rightIndex].position = pos;
        }

        leftIndex = rightIndex;
        rightIndex--;
        if(rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }
    }

    void ScrollRight()
    {
        int lastLeft = leftIndex;
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x + backgroundSize);

        if (setY)
        {
            Vector3 pos = layers[leftIndex].position;
            pos.y = 17f +3.48f; //the y pos that puts it behind the second part of the level
            layers[leftIndex].position = pos;
        }

        rightIndex = leftIndex;
        leftIndex++;
        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    }


    // Update is called once per frame
    void Update()
    {
        float deltaX = player.transform.position.x - lastPlayerX;
        transform.position += Vector3.right * (deltaX * paralaxSpeed);
        lastPlayerX = player.transform.position.x;
        if (player.transform.position.x < (layers[leftIndex].transform.position.x + viewZone) + 5f)
        {
            ScrollLeft();
        }

        if (player.transform.position.x > (layers[rightIndex].transform.position.x - viewZone) - 5f) //minus the 5f to make the system draw the layers before hand so the player never sees it not drawn
        {
            ScrollRight();
        }
    }
}

