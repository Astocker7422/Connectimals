using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Controls behaviors of the follower objects
//Attached to the follower objects
public class CubeFollower : MonoBehaviour
{
    //Follower attributes
    public float speed;
    public float distance;

    //The Player
    public CubePlayer player;

    public bool isDead;

    //Follower components
    private Rigidbody rigid;
    private NavMeshAgent agent;

    //The GameObject this Follower object should follow
    public GameObject leader;

    //Indicates this follower has a leader
    private GameObject connectionIndicator;
    private Renderer indicatorRenderer;

    //Indicates whether this Follower object is following another object
    private bool isFollowing;

    //This follower's index in the player's list of followers
    public int index;

    //Indicates if the follower is in the player's list of followers
    private bool inList;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize components
        rigid = transform.GetComponent<Rigidbody>();
        agent = transform.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<CubePlayer>();

        //Find the connection indicator object and deactivate it
        connectionIndicator = transform.FindDeepChild("Connection Indicator").gameObject;
        indicatorRenderer = connectionIndicator.GetComponent<Renderer>();
        connectionIndicator.SetActive(false);

        //Indicate this Follower object is not following another object
        isFollowing = false;

        //Initialize a value for index
        index = 0;

        inList = false;

        isDead = false;
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

    public bool GetIsFollowing()
    {
        return isFollowing;
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
                //Set the leader to the player
                leader = player.gameObject;
            }

            //Update the index
            index = listSize;

            //Indicate the follower is in the player's list
            inList = true;

            //Activate the connection indicator object
            connectionIndicator.SetActive(true);

            //Change indicator to connected color
            indicatorRenderer.material.color = Color.green;
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
        //Set new leader
        leader = newLeader;

        //Stop movement
        isFollowing = false;

        //Change indicator to disconnected color
        indicatorRenderer.material.color = Color.yellow;
    }

    public void SetIndicatorColor(Color newColor)
    {
        indicatorRenderer.material.color = newColor;
    }

    //Moves this Follower object toward its leader
    private void MoveToLeader()
    {
        agent.SetDestination(leader.transform.position);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.CompareTag("Obstacle"))
        {
            agent.ResetPath();

            inList = false;

            //Change indicator to dead color
            indicatorRenderer.material.color = Color.red;

            //Indicate this follower has no leader
            isFollowing = false;

            isDead = true;

            //Update the player's list of followers
            player.UpdateFollowers(index);
        }
    }
}
