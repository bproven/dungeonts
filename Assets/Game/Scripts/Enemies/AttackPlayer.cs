﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour {

    public GameObject player;
    public float moveSpeed;
    private int maxHealth;
    public int health;
    public int strength = 1;
	// Use this for initialization
	void Start () {
        maxHealth = health;
	}
	
	// Update is called once per frame
	void Update () {
        // Keep it simple
        Vector2 newVelocity = (player.transform.position - gameObject.transform.position);
        if (newVelocity.magnitude > 2.5f)
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
        if (health <= 0)
            die();
        if (health == 1)
            GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void reset()
    {
        health = maxHealth;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        // Spawn Variance
        Vector2 rand = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, GetComponent<CircleCollider2D>().radius, rand, rand.magnitude);
        if (hit)
            transform.position = hit.centroid;
        else
            transform.position += new Vector3(rand.x, rand.y);
    }

    void die()
    {
        gameObject.SetActive(false);
    }

    // Kill the player who we collided with
    void killPlayer(Collision2D coll)
    {
        for (int i = 0; i < coll.transform.childCount; i++)
        {
            // Punish all brains involved in this disaster
            if (coll.transform.GetChild(i).GetComponent<Agent>())
                coll.transform.GetChild(i).GetComponent<Agent>().reward -= 5f;
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
