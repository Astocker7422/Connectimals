using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Controls behaviors of the follower objects
//Attached to the follower objects
public class Follower : MonoBehaviour
{
    //Follower attributes
    public float speed;
    public float distance;

    //The Player
    public Player player;

    //Follower components
    private Rigidbody rigid;
    private NavMeshAgent agent;

    //The GameObject this Follower object should follow
    private GameObject leader;

    //Indicates this follower has a leader
    private GameObject connectionIndicator;

    //Indicates whether this Follower object is following another object
    private bool isFollowing;

    //This follower's index in the player's list of followers
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize components
        rigid = transform.GetComponent<Rigidbody>();
        agent = transform.GetComponent<NavMeshAgent>();

        //Find the connection indicator object and deactivate it
        connectionIndicator = transform.FindDeepChild("Connection Indicator").gameObject;
        connectionIndicator.SetActive(false);

        //Indicate this Follower object is not following another object
        isFollowing = false;

        //Initialize a value for index
        index = 0;
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

                //Update this follower's index
                index = listSize - 1;
            }
            //If the list is empty,
            else
            {
                //Set the leader to the player
                leader = player.gameObject;

                //Update this follower's index
                index = 0;
            }

            //Activate the connection indicator object
            connectionIndicator.SetActive(true);
        }

        //Set the indicator based on the input
        isFollowing = newFollowing;
    }

    //Set the index of this follower in the player's list of followers
    public void SetIndex(int newIndex)
    {
        index = newIndex;
    }

    //Set the object this follower should follow
    public void SetLeader(GameObject newLeader)
    {
        leader = newLeader;
    }

    //Moves this Follower object toward its leader
    private void MoveToLeader()
    {
        agent.SetDestination(leader.transform.position);
    }

    void OnCollisionEnter(Collision coll)
    {
        if(coll.transform.CompareTag("Obstacle"))
        {
            agent.ResetPath();

            //Indicate this follower has no leader
            isFollowing = false;

            //Deactivate the connection indicator object
            connectionIndicator.SetActive(false);

            //Update the player's list of followers
            player.UpdateFollowers(index);
        }
    }
}
