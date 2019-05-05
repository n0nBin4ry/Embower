using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sapController : MonoBehaviour
{
    public float speed;

    Vector2 startPosOfDrop;
    // Start is called before the first frame update
    void Start()
    {
        startPosOfDrop = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if it collides with the shredder, delete the instance
        if (collision.tag == "Shredder")
        {
            transform.position = startPosOfDrop;
        }
    }
}
