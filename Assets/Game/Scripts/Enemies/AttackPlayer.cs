using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour {

    public GameObject player;
    public float moveSpeed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        // Keep it simple
        Vector2 newVelocity = (player.transform.position - gameObject.transform.position);
        newVelocity.Normalize();
        newVelocity.Scale(new Vector2(moveSpeed, moveSpeed));
        gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
    }

    // Kill the player who we collided with
    void killPlayer(Collision2D coll)
    {
        Debug.Log("Enemy: Player killed.");
        // Punish all brains involved in this disaster
        for (int i = 0; i < coll.transform.childCount; i++)
            if (coll.transform.GetChild(i).GetComponent<Agent>())
                coll.transform.GetChild(i).GetComponent<Agent>().done = true;

        // Randomize my position so I can't spawn camp
        transform.position = new Vector3(Random.Range(transform.parent.position.x - 2, transform.parent.position.x + 2), Random.Range(transform.parent.position.y - 2, transform.parent.position.y + 2), transform.parent.position.z);
    }

    void OnCollisionEnter2D (Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            killPlayer(coll);
        }
    }

    void OnCollisionStay2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            killPlayer(coll);
        }
    }
}
