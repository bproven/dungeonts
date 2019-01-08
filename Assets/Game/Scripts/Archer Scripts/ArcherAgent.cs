﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAgent : Agent
{
    [Header("Specific to ArcherTesting")]
    // PLAYER SETTINGS, used for default values on agent reset
    public static float RANGE = 0.45f, FIRERATE = 0.6f;
    public static float STR = 1;              // without weapons

    // Local settings to be adjusted by items
    public float myFireRate, myRange;   // Local fire rate, range
    public float myStrength;              // Local strength

    /// <summary>
    /// The Agen't strength as modified by Items
    /// </summary>
    public float Strength { get; set; }

    /// <summary>
    /// The Agent's range as modified by items
    /// </summary>
    public float Range { get; set; }

    public LooterAgent Looter
    {
        get
        {
            return looter.GetComponent<LooterAgent>();
        }
    }

    // Public variables to be set in scene
    public GameObject looter;
    public float turnSpeed;
    public GameObject arrow;

    // Output debug
    public bool debug;
    public float lastShot;
    private float startTime = 0;
    private float roundTime;
    
    public uint numRays;

    public int shotDelay;

    public float stateReward;

    public override void CollectObservations()
    {
        
        // Keep the cooldown binary
        AddVectorObs(((Time.time - lastShot) / myFireRate >= 1 ? 1 : 0));
        
        for (int i = 0; i < numRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Quaternion.AngleAxis((360f / numRays) * i, Vector3.forward) * transform.up, Range);
            // Debug
            if (debug)
                if (hit)
                    Debug.DrawLine(gameObject.transform.position, hit.point, hit.collider.tag == "Enemy" ? Color.green : Color.blue);
                else
                    Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position + (Quaternion.AngleAxis((360f / numRays) * i, Vector3.forward) * transform.up).normalized * Range));
            // Encode state data
            if (hit && hit.collider.tag == "Enemy")
            {
                AddVectorObs(hit.distance / Range);  // It's this far away
                AddVectorObs(1.0f);                        // It's an enemy
                // Reward aiming at nearby enemies
                stateReward += RewardSettings.aim * (1.0f - hit.distance / Range)
                    * (Mathf.Pow(0.5f - (float)i / numRays, 2))
                    / numRays;
            }
            else
            {
                AddVectorObs(0.0f);    // Clean up non-enemy distances
                AddVectorObs(0.0f);    // There's no enemy there
            }
        }
        
    }

    private void punishMiss()
    {
        stateReward -= RewardSettings.attack_miss;   // Don't miss
    }

    private void fireArrow()
    {
        if (Time.time - lastShot < myFireRate)
            return;
        else
            lastShot = Time.time;

		RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, Range);
        // Debug
        if (debug)
            Debug.DrawLine(transform.position, hit ? (Vector3)hit.point : transform.position + transform.up * Range);

        if (hit && hit.collider.tag == "Enemy")
        {
            stateReward += RewardSettings.attack_enemy;  // Reward kills
            hit.collider.GetComponent<AttackPlayer>().health -= Strength; // FIXME: Make an enemy TakeDamage(int damage) function, we shouldn't be responsible for this

			//setting variables for attack animatoins
			transform.parent.GetComponent<Animate>().hit = true;
			transform.parent.GetComponent<Animate>().enemy = hit.normal;

			if (looter.GetComponent<LooterAgent>())
                looter.GetComponent<LooterAgent>().stateReward += RewardSettings.attack_enemy;   // Thanks for moving me into position

        }
        else
            punishMiss();
    }

    // to be implemented by the developer
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (looter.GetComponent<LooterAgent>().Health <= 0)
            return;

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            int act = Mathf.FloorToInt(vectorAction[0]);
            switch (act)
            {
                case 0:
                    gameObject.transform.Rotate(new Vector3(0, 0, -turnSpeed));
                    break;
                case 1:
                    gameObject.transform.Rotate(new Vector3(0, 0, turnSpeed));
                    break;
                case 2:
                    fireArrow();
                    break;
                case 3:
                    gameObject.transform.Rotate(new Vector3(0, 0, 2 * -turnSpeed));
                    break;
                case 4:
                    gameObject.transform.Rotate(new Vector3(0, 0, 2 * turnSpeed));
                    break;
            }

        }
        else if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            if (vectorAction[1] > 0)
                fireArrow();    // Attack
            gameObject.transform.Rotate(new Vector3(0, 0, (Mathf.Round(vectorAction[0] * 2) * turnSpeed)));  // Turn
        }
        AddReward(stateReward);
        stateReward = 0;

        // Reset the round if necessary
        /*
        if (Time.time - startTime > roundTime)
        {
            done = true;
            return;
        }*/
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
        stateReward = 0;
        gameObject.transform.up = new Vector3(0, 1, 0);

        myFireRate = FIRERATE;
        myRange = RANGE;
        myStrength = STR;

        roundTime = LooterAgent.TIME;  // Reset default stats

        Strength = myStrength;
        Range = myRange;

        startTime = Time.time; lastShot = Time.time - myFireRate;   // Reset cooldowns

        turnSpeed = 360 / numRays;
    }

}
