using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour {

    public float myRespawnRange = 1.0f;    // Controls the variance in where this object spawns during reset

    public GameObject player;
    public float moveSpeed;
    private float maxHealth;
    public float health;
    public float strength = 1;

    private float HealthBar_Length;
	
	//private float prevHealth;
	
	// Use this for initialization
	void Start () {
        maxHealth = health;
        HealthBar_Length = transform.GetChild(0).lossyScale.x;
		//prevHealth = health;
	}
	
	// Update is called once per frame
	void Update () {
        // Keep it simple
        if (player.transform.GetChild(0).GetComponent<Agent>().done)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            return;
        }
        Vector2 newVelocity = (player.transform.position - gameObject.transform.position);
        if (newVelocity.sqrMagnitude > 6.25f)
            newVelocity = Vector2.zero;
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, newVelocity);
            if (hit.collider.tag != "Player")
                newVelocity = Vector2.zero;
            else
            {
                newVelocity.Normalize();
                newVelocity.Scale(new Vector2(moveSpeed, moveSpeed));
            }
        }
        gameObject.GetComponent<Rigidbody2D>().velocity = newVelocity;
        transform.GetChild(0).localScale = new Vector3((health / maxHealth) * HealthBar_Length, transform.GetChild(0).localScale.y, transform.GetChild(0).localScale.z);
		/*
		if (prevHealth != health) {
			player.GetComponent<Animate>().hit = true;
			player.GetComponent<Animate>().enemy = transform.position - player.transform.position;
		}
		prevHealth = health;
		*/
        if (health <= 0)
            die();
    }

    void randomizePosition()
    {
        // Spawn Variance
        Vector2 rand = new Vector2(Random.Range(-myRespawnRange, myRespawnRange), Random.Range(-myRespawnRange, myRespawnRange));
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, GetComponent<CircleCollider2D>().radius, rand, rand.magnitude);
        if (hit)
            transform.position = hit.centroid;
        else
            transform.position += new Vector3(rand.x, rand.y);
    }

    public void reset()
    {
        health = maxHealth;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        randomizePosition();
    }

    void die()
    {
        gameObject.SetActive(false);
        ResultsWindow.Score_EnemyKilled();
    }

    // Kill the player who we collided with
    void killPlayer(Collision2D coll)
    {
        for (int i = 0; i < coll.transform.childCount; i++)
        {
            // Punish the looter brains involved in this disaster
            if (coll.transform.GetChild(i).GetComponent<LooterAgent>())
                coll.transform.GetChild(i).GetComponent<LooterAgent>().stateReward -= RewardSettings.collide_enemy;
            // Apply damage
            if (coll.transform.GetChild(i).GetComponent<LooterAgent>())
                coll.transform.GetChild(i).GetComponent<LooterAgent>().looterTakeDamage(strength);
        }
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
