using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAgent : Agent
{
    [Header("Specific to ArchetTesting")]
    // PLAYER SETTINGS, used for default values on agent reset
    public static float RANGE = 0.35f, FIRERATE = 0.5f;
    public static int STR = 1;

    // Local settings to be adjusted by items
    public float myFireRate, myRange;   // Local fire rate, range
    public int myStrength;              // Local strength

    // Public variables to be set in scene
    public GameObject looter;
    public float turnSpeed;

    // Output debug
    public bool debug;
    public float lastShot;
    private float startTime = 0;
    private float roundTime;
    
    public float x, y, fire;
    public uint numRays;

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        float cd = (Time.time - lastShot) / myFireRate;
        // Keep the cooldown normalized
        state.Add((cd > 1 ? 1 : 0));

        // New implementation, raycasts in 8 directions with information on what they hit
        RaycastHit2D[] rays = new RaycastHit2D[numRays];
        // New loop
        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = Physics2D.Raycast(gameObject.transform.position, Quaternion.AngleAxis((360f / rays.Length) * i, Vector3.forward) * transform.up, myRange);
            // Debug
            if (debug)
                if (rays[i])
                    Debug.DrawLine(gameObject.transform.position, rays[i].point, rays[i].collider.tag == "Enemy" ? Color.green : Color.blue);
                else
                    Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position + (Quaternion.AngleAxis((360f / rays.Length) * i, Vector3.forward) * transform.up).normalized * myRange));
        }
        for (int i = 0; i < rays.Length; i++)
        {
            // Pretend any data about walls/coins/etc isn't there. Just don't want to see enemies through walls, anything else is irrelevant
            if (rays[i])
            {
                if (rays[i].collider.tag == "Enemy")
                    state.Add(rays[i].distance / myRange);
                else
                    state.Add(0.0f);
            }
            else
                state.Add(0.0f);
            
            // Add information about what was hit
            if (rays[i])
            {
                // We only need information about whether it's an enemy or not, really
                if (rays[i].collider.tag == "Enemy")
                {
                    state.Add(1.0f);
                    // Nudge it towards aiming correctly
                    if (i == 0)
                        reward += 0.01f;
                }
                else
                    state.Add(0.0f);
            }
            else
                state.Add(0.0f);
        }
        return state;
    }

    private void punishMiss()
    {
        reward -= 2f;
    }

    private void fireArrow()
    {
        if (Time.time - lastShot < myRange)
            return;
        else
            lastShot = Time.time;

		RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, myRange);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Enemy")
            {
                reward += 5;
                hit.collider.GetComponent<AttackPlayer>().health -= myStrength;
                if (looter.GetComponent<Agent>())
                    looter.GetComponent<Agent>().reward += 2;
            }
            else
                punishMiss();
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
            gameObject.transform.Rotate(new Vector3(0, 0, act[0] * turnSpeed));
            // Debug
            x = act[0]; fire = act[1];
            // Fire
            if (act[1] > 0)
            {
                // Just leave it be, I need it to aim before it can shoot
                fireArrow();
            }
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
        myFireRate = FIRERATE;
        myRange = RANGE;
        myStrength = STR;

        roundTime = LooterAgent.TIME;
        startTime = Time.time;
        lastShot = Time.time - myFireRate;

    }
}
