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
        gameObject.GetComponent<Rigidbody2D>().velocity = (player.transform.position - gameObject.transform.position).normalized *moveSpeed * Time.deltaTime;
	}

    // Kill the player who we collided with
    void killPlayer(Collision2D coll)
    {
        Debug.Log("Enemy: Player killed.");
        coll.gameObject.GetComponent<Agent>().done = true;
        coll.gameObject.GetComponent<Agent>().reward -= 1;
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
