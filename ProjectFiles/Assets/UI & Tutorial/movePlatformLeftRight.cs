using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movePlatformLeftRight : MonoBehaviour
{
    // bool to tell us which way we are moving
    public bool movingRight = true;
    // speed at which we move
    public float speed;
    // distance in which we move from origin
    public float distance = 2;


    bool doMovement = true;

    protected float origin_point;

    // Start is called before the first frame update
    void Start()
    {
        origin_point = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (doMovement)
        {
            if (movingRight)
            {
                this.gameObject.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            }
            else
            {
                this.gameObject.transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
            }
            // change position if we are too far from origin
            if (Mathf.Abs(transform.position.x - origin_point) > distance)
            {
                float newX = movingRight ? origin_point + distance : origin_point - distance;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
                movingRight = !movingRight;
            }
        }
    }
}
