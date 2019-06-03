using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls camera that follows the player
//Attached to the main camera
public class CameraFollow : MonoBehaviour
{
    //The player the camera is following
    public GameObject player;
    private Player playerScript;

    //Distance from the player
    private float distance;

    //Camera movement speed
    private float speed;

    //Camera angles
    private float y;
    private float x;

    //Joystick deadzone
    private float joystickDeadZone;
    
    //The previous transform values of the camera
    private Vector3 lastPosition;
    private Quaternion lastRotation;

    void Start ()
    {
        playerScript = player.GetComponent<Player>();

        //Initialize distance from player and movement speed
        distance = 7.21f;
        speed = 50.0f;

        //Initialize angles
        Vector3 angles = transform.eulerAngles;
        y = angles.y;
        x = angles.x;

        joystickDeadZone = 0.02f;

        //Initialize transform values
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
	
	void LateUpdate ()
    {
        if (!playerScript.GetHasLost())
        {
            //Obtain the mouse/joystick input and scale
            if (Input.GetAxis("Right Joystick X") > joystickDeadZone || Input.GetAxis("Right Joystick X") < -joystickDeadZone)
            {
                y += Input.GetAxis("Right Joystick X") * speed;
            }
            else
            {
                y += Input.GetAxis("Mouse X") * speed * distance * 0.02f;
            }

            if (Input.GetAxis("Right Joystick Y") > joystickDeadZone || Input.GetAxis("Right Joystick Y") < -joystickDeadZone)
            {
                x += -Input.GetAxis("Right Joystick Y") * speed;
            }
            else
            {
                x += Input.GetAxis("Mouse Y") * speed * distance * 0.02f;
            }

            //Obtain the rotation of the camera and restrict the movement
            Quaternion rotation = Quaternion.Euler(Mathf.Clamp(-x, 0, 65), y, 0);

            //Calculate the position of the camera based on the rotation and distance
            Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + player.transform.position;

            //Apply transformation
            transform.rotation = rotation;
            transform.position = position;

            //Cast ray to check for objects
            RaycastHit hit = new RaycastHit();
            Ray wallRay = new Ray(player.transform.position, transform.position - player.transform.position);

            //If the ray hit an object,
            if (Physics.Raycast(wallRay, out hit, distance))
            {
                //If the object was not a Follower, 
                if (!hit.collider.gameObject.CompareTag("Follower"))
                {
                    //Move the camera to the point the ray hit
                    transform.position = hit.point;
                }
            }

            //Update previous transform values
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }
}
