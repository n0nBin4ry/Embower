using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera mainCam;
    public GameObject player;

    protected Vector3 cameraPos;
    protected float cameraW;
    protected float cameraH;

	// METRICS-DEPENDENT VARS
	int curr_floor = 1;
	int curr_room = 1;

	void Start()
    {
        //mainCam = Camera.main;   
        if (mainCam)
        {
            cameraPos = mainCam.transform.position;
            Debug.Log("Cam pos: " + cameraPos);
            cameraH = mainCam.orthographicSize;
            Debug.Log("Cam height: " + cameraH);
            cameraW = cameraH * mainCam.aspect;
            Debug.Log("Cam width: " + cameraW);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!constants.pause_cam) //pls do not take out this if statement!!! - atsi
        {
            Vector2 viewPos = mainCam.WorldToViewportPoint(player.transform.position);
            // default camera distance for z
            cameraPos.z = -10;
            if (viewPos.x < 0)
            {
                cameraPos.x -= 2 * cameraW;
                this.gameObject.GetComponent<NewGameCont>().setRespawn(player.transform.position);
				// change room for metrics data
				curr_room--;
				FindObjectOfType<NewGameCont>().setRoom(curr_floor, curr_room);
            }
            else if (viewPos.x > 1)
            {
                cameraPos.x += 2 * cameraW;
                this.gameObject.GetComponent<NewGameCont>().setRespawn(player.transform.position);
				// change room for metrics data
				curr_room++;
				FindObjectOfType<NewGameCont>().setRoom(curr_floor, curr_room);
			}

            if (viewPos.y < 0)
            {
                cameraPos.y -= 2 * cameraH;
                this.gameObject.GetComponent<NewGameCont>().setRespawn(player.transform.position);
				// change room for metrics data
				curr_floor--;
				FindObjectOfType<NewGameCont>().setRoom(curr_floor, curr_room);
			}
            else if (viewPos.y > 1)
            {
                cameraPos.y += 2 * cameraH;
                this.gameObject.GetComponent<NewGameCont>().setRespawn(player.transform.position);
				// change room for metrics data
				curr_floor++;
				FindObjectOfType<NewGameCont>().setRoom(curr_floor, curr_room);
			}

            mainCam.transform.position = cameraPos;
        }
        


    }

    //public void setCrystal(GameObject crys) { crystal = crys; }
}
