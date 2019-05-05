using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : MonoBehaviour
{
    Vector3 mouse_pos;
    Vector3 object_pos = new Vector3(0, 0,0);
    public float angle;
 
    void Update()
    {
        mouse_pos = Input.mousePosition;

        //use this later to spawn where the arrow needs to be??
       // mouse_pos.z = 5.23f; //The distance between the camera and object

        object_pos = Camera.main.WorldToScreenPoint(this.transform.position);

        mouse_pos.x = mouse_pos.x - object_pos.x;
        mouse_pos.y = mouse_pos.y - object_pos.y;

        angle = Mathf.Atan2(mouse_pos.y - this.transform.position.y, mouse_pos.x - this.transform.position.x) * Mathf.Rad2Deg;
        this.transform.localEulerAngles = new Vector3(0, 0, angle+270);
    }
}
