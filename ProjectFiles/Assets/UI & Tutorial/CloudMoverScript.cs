using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMoverScript : MonoBehaviour
{
    public float speed;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Shredder") //respawn the cloud
        {
            Vector2 pos;
            pos = gameObject.transform.position;
            pos.x = 75f;
            gameObject.transform.position = pos;
        }
    }
}
