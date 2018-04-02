using System.Collections;
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

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        // Keep the cooldown binary
        state.Add(((Time.time - lastShot) / myFireRate >= 1 ? 1 : 0));
        
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
                state.Add(hit.distance / Range);  // It's this far away
                state.Add(1.0f);                        // It's an enemy
                // Reward aiming at nearby enemies
                reward += (1.0f - hit.distance / Range)
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

		RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, Range);
        // Debug
        if (debug)
            Debug.DrawLine(transform.position, hit ? (Vector3)hit.point : transform.position + transform.up * Range);

        if (hit && hit.collider.tag == "Enemy")
        {
            reward += 15f;  // Reward kills
            hit.collider.GetComponent<AttackPlayer>().health -= Strength; // FIXME: Make an enemy TakeDamage(int damage) function, we shouldn't be responsible for this
            if (looter.GetComponent<Agent>())
                looter.GetComponent<Agent>().reward += 15f;   // Thanks for moving me into position

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
