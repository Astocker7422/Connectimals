using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    //Follower attributes
    public float speed;
    public float distance;

    //The Player
    public Player player;

    //Follower components
    private Rigidbody rigid;

    //The GameObject this Follower object should follow
    private GameObject leader;

    //Indicates whether this Follower object is following another object
    private bool isFollowing;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize components
        rigid = transform.GetComponent<Rigidbody>();

        //Indicate this Follower object is not following another object
        isFollowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        //If this Follower is following another object,
        if (isFollowing)
        {
            //Move this Follower object toward its leader
            MoveToLeader();
        }
    }

    //Indicate this Follower is or is not following another object
    public void SetIsFollowing(bool newFollowing)
    {
        //If this Follower is going to be following another object,
        if (newFollowing)
        {
            //Access the Player's list of followers
            List<GameObject> followerList = player.GetFollowers();

            //Access the size of the list
            int listSize = followerList.Count;

            //If the list is not empty,
            if (listSize > 0)
            {
                //Set the leader to the last follower in the list
                leader = followerList[listSize - 1];
            }
            //If the list is empty,
            else
            {
                leader = player.gameObject;
            }
        }

        //Set the indicator based on the input
        isFollowing = newFollowing;
    }

    //Moves this Follower object toward its leader
    private void MoveToLeader()
    {
        //Look at target
        Vector3 v3 = leader.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(v3);

        //If the leader is out of attack range,
        if (Vector3.Distance(transform.position, leader.transform.position) > distance)
        {
            //Move towards target
            rigid.velocity = new Vector3(transform.forward.x * speed, rigid.velocity.y, transform.forward.z * speed);
            Debug.Log(rigid.velocity);
        }
    }
}
