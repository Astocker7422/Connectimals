using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controls input and behaviors of the player object
//Attached to the player object
public class Player : MonoBehaviour
{
    //Player attributes
    public float speed;
    public float jumpSpeed;
    public float groundPoundSpeed;

    //Indicates if the player is touching the ground
    private bool isGrounded;

    //Indicates if the player is currently double jumping
    private bool isDoubleJumping;

    //Indicates if the double jump was activated during the previous frame
    private bool doubleJumpPrevFrame;

    //Value tracking how long the player has been gliding
    private float glideTimer;

    //Value representing how long the player is allowed to glide
    private float glideTime;

    //Indicates if the player is currently attacking in the air
    private bool isJumpAttacking;

    //Indicates if the player is ground pounding
    private bool isGroundPounding;

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

    private Animator animator;

    //Normal global gravity
    private float globalGravity = -9.81f;

    //Scale to apply to gravity
    private float gravityScale = 1.0f;

    void Start()
    {
        Time.timeScale = 1;

        jumpSpeed = 2;

        groundPoundSpeed = 5;

        isGrounded = true;

        isDoubleJumping = false;

        doubleJumpPrevFrame = false;

        glideTimer = 0;

        glideTime = 3;

        //Initialize components
        rigid = transform.GetComponent<Rigidbody>();

        animator = GetComponentInChildren<Animator>();

        rigid.useGravity = false;

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
        //If the jump key is pressed,
        if (Input.GetButtonDown("Jump"))
        {
            //If the player is touching the ground,
            if (isGrounded)
            {
                //Apply jump to player
                Jump();
            }
            //If the player is not touching the ground,
            else
            {
                //If the player has not already activated the double jump,
                if (!isDoubleJumping)
                {
                    //Apply the double jump to the player
                    DoubleJump();

                    //Indicate the double jump was activated during this frame
                    doubleJumpPrevFrame = true;
                }
            }
        }

        //If the jump key is held down,
        if (Input.GetButton("Jump"))
        {
            //If the double jump was activated this frame or the previous frame,
            if (doubleJumpPrevFrame)
            {
                //If the glide timer has not reached its limit,
                if (glideTimer < glideTime)
                {
                    //Reduce the gravity scale
                    gravityScale = 0.33f;
                }
                //If the glide timer has reached its limit,
                else
                {
                    //Return the gravity scale to normal
                    gravityScale = 1;
                }
            }
        }
        //If the jump key is not held down,
        else
        {
            //Indicate the double jump was not activated on the previous frame
            doubleJumpPrevFrame = false;

            //Return the gravity scale to normal
            gravityScale = 1;
        }

        //Take the movement input and apply movement speed
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = Input.GetAxis("Vertical") * speed;

        if(horizontal != 0 || vertical != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        //If the ground pound has not been activated,
        if (!isGroundPounding)
        {
            //Apply movement variables
            Movement(horizontal, vertical);
        }

        //If the attack button is pressed,
        if (Input.GetButtonDown("Fire1"))
        {
            if(!isGrounded)
            {
                StartCoroutine(GroundPound());
            }
        }
    }

    void FixedUpdate()
    {
        //If is not ground pounding,
        if (!isGroundPounding)
        {
            //Apply gravity
            rigid.AddForce(Vector3.up * globalGravity * gravityScale, ForceMode.Acceleration);
        }
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

    //Applies jumping behavior to the player object using the RigidBody
    private void Jump()
    {
        //If the ground pound has not been activated,
        if (!isGroundPounding)
        {
            //Apply a jump and play the jump audio
            rigid.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }
    }

    //Applies slight upward velocity to the player if they have already jumped once
    private void DoubleJump()
    {
        //If the ground pound has not been activated,
        if (!isGroundPounding)
        {
            //Indicate the player has activated double jump
            isDoubleJumping = true;

            //Move the Rigidbody upward and maintain movement in X
            rigid.AddForce(Vector3.up * jumpSpeed / 2, ForceMode.Impulse);
        }
    }

    //Waits in air, then falls quickly
    private IEnumerator GroundPound()
    {
        isGroundPounding = true;

        rigid.velocity = Vector3.zero;

        yield return new WaitForSeconds(1);

        rigid.AddForce(Vector3.down * groundPoundSpeed, ForceMode.Impulse);
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
        //If the other object is the ground,
        else if (coll.transform.CompareTag("Ground"))
        {
            //Return gravity scale to normal
            //if player landed from glide before glide function finished
            gravityScale = 1;

            //Indicate the player is touching the ground
            isGrounded = true;

            //Indicate the player has not activated the double jump
            isDoubleJumping = false;

            //Indicate the double jump was not activated on the previous frame
            doubleJumpPrevFrame = false;

            isGroundPounding = false;

            //Reset glide timer
            glideTimer = 0;
        }
    }

    void OnCollisionExit(Collision coll)
    {
        //If the other object is the ground,
        if (coll.transform.CompareTag("Ground"))
        {
            //Indicate the player is not touching the ground
            isGrounded = false;
        }
    }
}
