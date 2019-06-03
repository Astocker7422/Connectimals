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
    public List<GameObject> followers;

    //Total number of followers in the level
    private int totalFollowers;

    //Number of followers lost
    private int lostFollowers;

    //Text displaying number of followers collected
    private TMPro.TMP_Text countText;

    //Menu that appears when the player loses
    private GameObject loseMenu;

    //Indicates if the player has lost
    private bool hasLost;

    //Follow script of the main camera
    private CameraFollow cameraScript;

    void Start()
    {
        Time.timeScale = 1;

        //Initialize components
        rigid = GetComponent<Rigidbody>();

        //Initialize list of followers
        followers = new List<GameObject>();

        //Find total number of followers in the level
        totalFollowers = GameObject.Find("Followers").GetComponentsInChildren<Transform>().Length - 1;

        //Initialize number of followers lost
        lostFollowers = 0;

        //The main canvas of the scene
        Transform canvas = GameObject.Find("Canvas").transform;

        //Find and initialize text diplaying number of followers collected
        countText = canvas.Find("Count Text").GetComponentInChildren<TMPro.TMP_Text>();
        SetCountText();

        //Find the losing menu and deactivate it
        loseMenu = canvas.Find("Lose Menu").gameObject;
        loseMenu.SetActive(false);

        //Indicate the player has not lost
        hasLost = false;

        cameraScript = Camera.main.GetComponent<CameraFollow>();
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

    //Returns bool indicating if the player has lost
    public bool GetHasLost()
    {
        return hasLost;
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

            //Update number of followers lost
            lostFollowers += (followers.Count - index);

            //Update count display
            SetCountText();
        }
        else
        {
            //
            //UPDATE LEADER OF EACH FOLLOWER
            //

            //For each index in the list of followers from index + 1 to the end,
            for (int listIndex = followers.Count - 1; listIndex >= index; listIndex--)
            {
                //Access the follower object at that index
                Follower currFollower = followers[listIndex].GetComponent<Follower>();

                if(listIndex != index)
                {
                    currFollower.SetIndicatorColor(Color.yellow);

                    currFollower.SetIsFollowing(false);

                    cameraScript.IncreaseDistance(false);
                }
            }

            //Remove all followers from list from index to end of list
            followers.RemoveRange(index, followers.Count - index);

            cameraScript.IncreaseDistance(false);

            //Update number of followers lost
            lostFollowers += 1;

            //Update count display
            SetCountText();
        }
    }

    //Update the text diplaying the number of followers collected
    private void SetCountText()
    {
        countText.text = "Total: " + totalFollowers 
                         + "\nConnected: " + followers.Count
                         + "\nLost: " + lostFollowers;
    }

    void OnTriggerEnter(Collider other)
    {
        //If the other object is a Follower,
        if(other.CompareTag("Follower"))
        {
            Follower newFollower = other.GetComponent<Follower>();

            if (!newFollower.isDead && !other.name.Contains("Indicator"))
            {
                //If the list of followers does not contain the other object
                if (!followers.Contains(other.gameObject))
                {
                    //Indicate the other object is following another object
                    newFollower.SetIsFollowing(true);

                    //Add the other object to the list of followers
                    followers.Add(other.gameObject);

                    cameraScript.IncreaseDistance(true);

                    //Update the count text
                    SetCountText();
                }
                //If the list of followers does contain the other object,
                else
                {
                    //If the other object is not following another object
                    if (!newFollower.GetIsFollowing())
                    {
                        //Indicate it should follow another object
                        other.GetComponent<Follower>().SetIsFollowing(true);

                        //For each index from the index of the other object to the end of the follower list,
                        for (int index = followers.IndexOf(other.gameObject); index < followers.Count; index++)
                        {
                            //Access the follower at that index
                            Follower currFollower = followers[index].GetComponent<Follower>();

                            //Change the indicator color to the connected color
                            currFollower.SetIndicatorColor(Color.green);
                        }
                    }
                }
            }
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.transform.CompareTag("Obstacle"))
        {
            hasLost = true;

            Time.timeScale = 0;

            loseMenu.SetActive(true);
        }
    }
}
