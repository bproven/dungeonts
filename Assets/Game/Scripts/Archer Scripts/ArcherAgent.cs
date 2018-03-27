using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAgent : Agent
{
    [Header("Specific to ArchetTesting")]
    // PLAYER SETTINGS, used for default values on agent reset
    public static float RANGE = 0.6f, FIRERATE = 0.6f;
    public static int STR = 1;

    // Local settings to be adjusted by items
    public float myFireRate, myRange;   // Local fire rate, range
    public int myStrength;              // Local strength

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

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        // Keep the cooldown binary
        state.Add(((Time.time - lastShot) / myFireRate >= 1 ? 1 : 0));
        
        for (int i = 0; i < numRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Quaternion.AngleAxis((360f / numRays) * i, Vector3.forward) * transform.up, myRange);
            // Debug
            if (debug)
                if (hit)
                    Debug.DrawLine(gameObject.transform.position, hit.point, hit.collider.tag == "Enemy" ? Color.green : Color.blue);
                else
                    Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position + (Quaternion.AngleAxis((360f / numRays) * i, Vector3.forward) * transform.up).normalized * myRange));
            // Encode state data
            if (hit && hit.collider.tag == "Enemy")
            {
                state.Add(hit.distance / myRange);  // It's this far away
                state.Add(1.0f);                        // It's an enemy
                // Reward aiming at nearby enemies
                reward += (1.0f - hit.distance / myRange)
                    * (Mathf.Pow(0.5f - (float)i / numRays, 2))
                    / numRays;
            }
            else
            {
                state.Add(0.0f);    // Clean up non-enemy distances
                state.Add(0.0f);    // There's no enemy there
            }
        }
        return state;
    }

    private void punishMiss()
    {
        reward -= 5f;   // Don't miss
    }

    private void fireArrow()
    {
        if (Time.time - lastShot < myFireRate)
            return;
        else
            lastShot = Time.time;

		RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, myRange);
        // Debug
        if (debug)
            Debug.DrawLine(transform.position, hit ? (Vector3)hit.point : transform.position + transform.up * myRange);

        if (hit && hit.collider.tag == "Enemy")
        {
            reward += 15f;  // Reward kills
            hit.collider.GetComponent<AttackPlayer>().health -= myStrength; // FIXME: Make an enemy TakeDamage(int damage) function, we shouldn't be responsible for this
            if (looter.GetComponent<Agent>())
                looter.GetComponent<Agent>().reward += 2;   // Thanks for moving me into position

        }
        else
            punishMiss();
    }

    // to be implemented by the developer
    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.discrete)
        {
        }
        else if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            if (act[1] > 0)
                fireArrow();    // Attack
            gameObject.transform.Rotate(new Vector3(0, 0, (Mathf.Round(act[0] * 2) * turnSpeed)));  // Turn
        }

        // Reset the round if necessary
        if (Time.time - startTime > roundTime)
        {
            done = true;
            return;
        }
    }

    // to be implemented by the developer
    public override void AgentReset()
    {
        gameObject.transform.up = new Vector3(0, 1, 0);
        myFireRate = FIRERATE; myRange = RANGE; myStrength = STR; roundTime = LooterAgent.TIME;  // Reset default stats
        startTime = Time.time; lastShot = Time.time - myFireRate;   // Reset cooldowns

        turnSpeed = 360 / numRays;
    }
}
