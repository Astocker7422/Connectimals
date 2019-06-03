using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls input and behaviors of the player object
//Attached to the player object
public class Player : MonoBehaviour
{
    //Player attributes
    public float speed;

    //Indicates if loses all followers behind one that dies
    public bool breakChain;

    //Player components
    private Rigidbody rigid;

    //List of current followers
    private List<GameObject> followers;

    //Total number of followers in the level
    private int totalFollowers;

    //Text displaying number of followers collected
    private TMPro.TMP_Text countText;

    void Start()
    {
        //Initialize components
        rigid = GetComponent<Rigidbody>();

        //Initialize list of followers
        followers = new List<GameObject>();

        //Find total number of followers in the level
        totalFollowers = GameObject.Find("Followers").GetComponentsInChildren<Transform>().Length - 1;

        //Find and initialize text diplaying number of followers collected
        countText = GameObject.Find("Canvas").GetComponentInChildren<TMPro.TMP_Text>();
        SetCountText();
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
        Vector3 direction = Camera.main.transform.TransformDirection(new Vector3(horizontal, 0, vertical));
        rigid.velocity = new Vector3(direction.x, rigid.velocity.y, direction.z);

        Vector3 newRotDir = new Vector3(direction.x, 0, direction.z);

        if(newRotDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(newRotDir);
        }
    }

    //Access list of followers
    public List<GameObject> GetFollowers()
    {
        return followers;
    }

    //Update list of followers after losing the follower at the input index
    public void UpdateFollowers(int index)
    {
        if (breakChain)
        {
            //
            //REMOVE ALL FOLLOWERS AT AND AFTER INDEX
            //

            //For each index in the list of followers from index to the end,
            for (int listIndex = followers.Count - 1; listIndex >= index; listIndex--)
            {
                //Indicate the follower at that index has no leader
                followers[listIndex].GetComponent<Follower>().SetIsFollowing(false);
            }
            //Remove all followers from list from index to end of list
            followers.RemoveRange(index, followers.Count - index);
        }
        else
        {
            //
            //UPDATE LEADER OF EACH FOLLOWER
            //

            //Remove the follower at the input index
            followers.RemoveAt(index);

            //For each index in the list of followers from index + 1 to the end,
            for (int listIndex = followers.Count - 1; listIndex >= index; listIndex--)
            {
                //Access the follower object at that index
                Follower currFollower = followers[listIndex].GetComponent<Follower>();

                //Update its index to the current index
                currFollower.SetIndex(listIndex);

                //If the current index is the index of the removed follower,
                if (listIndex == index)
                {
                    //If this follower is now the first in the list
                    if(listIndex == 0)
                    {
                        //Update the follower's leader to be the player
                        currFollower.SetLeader(transform.gameObject);
                    }
                    else
                    {
                        //Update the follower's leader
                        currFollower.SetLeader(followers[listIndex - 1]);
                    }
                }
            }
        }
    }

    //Update the text diplaying the number of followers collected
    private void SetCountText()
    {
        countText.text = "Connectimals: " + followers.Count + "/" + totalFollowers;
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

                SetCountText();
            }
        }
    }
}
