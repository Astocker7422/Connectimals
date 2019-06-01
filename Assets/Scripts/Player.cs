using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls input and behaviors of the player object
//Attached to the player object
public class Player : MonoBehaviour
{
    //Player attributes
    public float speed;

    //Player components
    private Rigidbody rigid;

    //List of current followers
    private List<GameObject> followers;

    void Start()
    {
        //Initialize components
        rigid = GetComponent<Rigidbody>();

        //Initialize list of followers
        followers = new List<GameObject>();
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
    private void Movement(float horizontal, float vertical)
    {
        rigid.velocity = new Vector3(horizontal, 0.0f, vertical) * speed;
    }

    //Access list of followers
    public List<GameObject> GetFollowers()
    {
        return followers;
    }

    void OnTriggerEnter(Collider other)
    {
        //If the other object is a Follower,
        if(other.CompareTag("Follower"))
        {
            //If the list of followers does not contain the other object
            if(!followers.Contains(other.gameObject))
            {
                //Indicate the other object is following another object
                other.GetComponent<Follower>().SetIsFollowing(true);

                //Add the other object to the list of followers
                followers.Add(other.gameObject);
            }
        }
    }
}
