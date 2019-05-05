using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedMouseScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; //dont want to see the default cursor
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 temp = Input.mousePosition;
       temp.z = 20f;
        transform.position = Camera.main.ScreenToWorldPoint(temp);
    }
}
