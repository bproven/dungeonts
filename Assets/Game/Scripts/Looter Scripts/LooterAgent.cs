using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooterAgent : Agent
{
    // PLAYER SETTINGS, used for default values on agent reset
    public static int HP = 3, TIME = 60;    // Max health, level timer
    public static float DEX = 2;            // Movement speed

    // Local settings to be adjusted by items
    public float mySpeed;   // Local Speed Stat
    public int myHealth;    // Local Health Stat

    // Set in scene
    public GameObject shooter;      // GameObject that the ArcherAgent script is attached to
    public GameObject levelManager; // GameObject that the LevelSpawner script is attached to

    // Vision specific variables
    public float sightDistance;
    public uint numRays;

    // Output debug
    public bool debug;
    public float myReward, x, y;    // Reward, output velocity components

    // Local timers
    private float roundStart;
    private float lastDamage;

    /// <summary>
    /// Use this method to initialize your agent. This method is called when the agent is created. 
    /// Do not use Awake(), Start() or OnEnable().
    /// </summary>
    public override void InitializeAgent()
    {
        base.InitializeAgent();
    }

    /// <summary>
    /// Must return a list of floats corresponding to the state the agent is in. If the state space type is discrete, 
    /// return a list of length 1 containing the float equivalent of your state.
    /// </summary>
    /// <returns></returns>
    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        // New input, trying to add some kind of memory for confusing situations
        // Hey, past me, where did you want to go again?
        Vector2 dir = transform.parent.GetComponent<Rigidbody2D>().velocity;
        if (mySpeed != 0)
        {
            state.Add(dir.x / mySpeed);
            state.Add(dir.y / mySpeed);
        }
        else
        {
            state.Add(0.0f);
            state.Add(0.0f);
        }

        // What can I see?
        RaycastHit2D[] rays = new RaycastHit2D[numRays];
        // New loop
        for (int i = 0; i < rays.Length; i++)
        {
            rays[i] = Physics2D.Raycast(gameObject.transform.position, Quaternion.AngleAxis((360f / rays.Length) * i, Vector3.forward) * transform.up, sightDistance);
            // Debug
            if (debug)
            {
                Color collisionColor = Color.white;
                if (rays[i])
                {
                    if (rays[i].collider.tag == "Enemy") collisionColor = Color.red;
                    else if (rays[i].collider.tag == "Gold") collisionColor = Color.yellow;
                    else if (rays[i].collider.tag == "Wall") collisionColor = Color.blue;
                    else collisionColor = Color.magenta;
                    Debug.DrawLine(gameObject.transform.position, rays[i].point, collisionColor);
                }
                else
                    Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position + (Quaternion.AngleAxis((360f / rays.Length) * i, Vector3.forward) * transform.up).normalized * sightDistance), collisionColor);
            }
        }
        for (int i = 0; i < rays.Length; i++)
        {
            state.Add(rays[i].distance / sightDistance);
            // Add information about what was hit
            if (rays[i])
            {
                // Is it a Wall? Gold? Enemy?
                // 1.0f for yes, 0.0f for no
                state.Add(rays[i].collider.tag == "Enemy" ? 1.0f : 0.0f);
                state.Add(rays[i].collider.tag == "Wall" ? 1.0f : 0.0f);
                state.Add(rays[i].collider.tag == "Gold" ? 1.0f : 0.0f);
            }
            else
            {
                state.Add(0.0f);    // NOT Enemy
                state.Add(0.0f);    // NOT Wall
                state.Add(0.0f);    // NOT Gold
            }
        }
        // Could we attack if we needed to?
        float cd = (Time.time - shooter.GetComponent<ArcherAgent>().lastShot) / shooter.GetComponent<ArcherAgent>().myFireRate;
        state.Add(cd >= 1 ? 1 : 0);
        return state;
    }
    
    /// <summary>
    /// This function will be called every frame, you must define what your agent will do given the input actions. 
    /// You must also specify the rewards and whether or not the agent is done. To do so, modify the public fields of the agent reward and done.
    /// </summary>
    /// <param name="act"></param>
    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.discrete)
        {

        }
        else if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {
            // Turn-based controls
            //transform.Rotate(new Vector3(0, 0, act[0] * turnSpeed));
            //gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = transform.up * maxSpeed;
            // WASD movement
            gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Clamp(act[0], -1, 1), Mathf.Clamp(act[1], -1, 1)) * mySpeed;
        }
        // Proximity Rewards, make the earlier runs more distinct in score before the late game
        RaycastHit2D[] rays = new RaycastHit2D[numRays];
        for (int i = 0; i < rays.Length; i++)
            rays[i] = Physics2D.Raycast(gameObject.transform.position, Quaternion.AngleAxis((360f / rays.Length) * i, Vector3.forward) * transform.up, sightDistance);
        for (int i = 0; i < rays.Length; i++)
        {
            // Add information about what was hit
            if (rays[i])
            {
                // Reward greed
                if (rays[i].collider.tag == "Gold")
                    reward += 0.1f * (1 - (rays[i].distance / sightDistance)) / numRays;
                // Reward risk
                if (rays[i].collider.tag == "Enemy")
                    reward += 0.1f * (1 - (rays[i].distance / sightDistance)) / numRays;
                // Quit it with the wall hugging
                if (rays[i].collider.tag == "Wall")
                    reward -= 0.05f * (Mathf.Pow(1 - (rays[i].distance / sightDistance), 2)) / numRays;
            }
        }
        if (Time.time - roundStart > TIME)
        {
            Debug.Log("Looter: Out of time.");
            done = true;
        }

        // Debug
        myReward = CumulativeReward;
    }

    /// <summary>
    /// This function is called at start, when the Academy resets and when the agent is done (if Reset On Done is checked).
    /// </summary>
    public override void AgentReset()
    {
        roundStart = Time.time;
        gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.parent.GetComponent<Gold>().value = 0;
        transform.up = Vector2.up;
        levelManager.GetComponent<LevelSpawner>().resetLevel();
        lastDamage = Time.time - 0.5f;
        // PLAYER SETTINGS
        myHealth = HP;  mySpeed = DEX;

        transform.parent.GetComponent<SpriteRenderer>().color = Color.green;

        transform.parent.position = levelManager.transform.GetChild(0).position;
    }

    public void looterTakeDamage(int damage)
    {
        if (Time.time - lastDamage > 0.5f)
        {
            Debug.Log("Taking Damage!");
            myHealth -= damage;
            if (debug)
                transform.parent.GetComponent<SpriteRenderer>().color = Color.red;
            if (myHealth <= 0)
            {
                done = true;
                shooter.GetComponent<ArcherAgent>().done = true;
            }
            lastDamage = Time.time;
        }

    }

    /// <summary>
    /// If Reset On Done is not checked, this function will be called when the agent is done. 
    /// Reset() will only be called when the Academy resets.
    /// </summary>
    public override void AgentOnDone()
    {

    }

}