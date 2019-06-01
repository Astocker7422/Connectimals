using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    public float distance;

    //The player
    public Player player;

    private Rigidbody rigid;

    private bool isFollowing;

    // Start is called before the first frame update
    void Start()
    {
        rigid = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isFollowing)
        {
            MoveToPlayer();
        }
    }

    public void SetIsFollowing(bool newFollowing)
    {
        isFollowing = newFollowing;
    }

    public void MoveToPlayer()
    {
        //Look at target
        Vector3 v3 = player.transform.position - transform.position;
        v3.y = 0.0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(v3), turnSpeed * Time.deltaTime);

        //If the enemy is out of attack range,
        if (Vector3.Distance(transform.position, player.transform.position) > distance)
        {
            //Move towards target
            rigid.velocity = new Vector3(transform.forward.x * speed, rigid.velocity.y, transform.forward.z * speed);
        }
    }
}
