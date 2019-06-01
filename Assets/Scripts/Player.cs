using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls input and behaviors of the player object
//Attached to the player object
public class Player : MonoBehaviour
{
    //Player attributes
    public float speed;

    private Rigidbody rigid;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Take the movement input and apply movement speed
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = Input.GetAxis("Vertical") * speed;

        //Apply movement variables
        Movement(horizontal, vertical);
    }

    //Applies movement to the player using the RigidBody
    void Movement(float horizontal, float vertical)
    {
        rigid.velocity = new Vector3(horizontal, 0.0f, vertical) * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Follower"))
        {
            other.GetComponent<Follower>().SetIsFollowing(true);
        }
    }
}
