using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAvoidance : MonoBehaviour {

    public GameObject looter;
    public float punishmentValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void punishCollision()
    {
        looter.GetComponent<LooterAgent>().stateReward -= RewardSettings.collide_wall;
    }

    void OnCollisionEnter2D (Collision2D coll)
    {
        if (coll.collider.tag == "Wall")
            punishCollision();
    }

    void OnCollisionStay2D (Collision2D coll)
    {
        if (coll.collider.tag == "Wall")
            punishCollision();
    }
}
