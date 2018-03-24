using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootAndShootAgent: Agent
{
    [Header("Specific to ArchetTesting")]
    public GameObject target;
    public GameObject home;
    public GameObject arrow;
    public int range;
    public float turnSpeed;
	public float moveForce;

    public float shotDelay;
    private float lastShot;
    public float roundStart = 0;
    public float roundTime;

    public float x, y, fire;
    
    public GameObject enemy;
    Vector2 tmpLootPos;
    public float Angle;
    public Text UITimer;

    private Vector2 bestLootPos()
    {
        Vector2 bestLootPos = Vector2.zero;

        GameObject[] loot = GameObject.FindGameObjectsWithTag("Gold");
        // if there is no more loot left... you win? Reset.
        // FIXME: Handle the case where there is no gold on the floor.
        if (loot.Length == 0)
        {
            done = true;
            return Vector2.zero;
        }

        float valPerDistance = 0;
        float bestVPD = 0;
        foreach (GameObject gold in loot)
        {
            if (gold.transform.position != gameObject.transform.position)
            {
                valPerDistance = gold.GetComponent<Gold>().value / (gold.transform.position - gameObject.transform.position).magnitude;
            }
            else
            {
                // We should really never have to be here. If we are, just ignore that loot, it will be in our inventory by next frame.
                continue;
            }
            if (valPerDistance >= bestVPD)
            {
                bestLootPos = gold.transform.position;
                bestVPD = valPerDistance;
            }
        }
        Debug.DrawLine(gameObject.transform.position, bestLootPos);
        tmpLootPos = bestLootPos;
        return bestLootPos;
    }


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
        
        // Gold States
        state.Add(gameObject.GetComponent<Rigidbody2D>().velocity.x);
        state.Add(gameObject.GetComponent<Rigidbody2D>().velocity.y);

        Vector2 bestLoot = bestLootPos();
        state.Add(bestLoot.x - gameObject.transform.position.x);
        state.Add(bestLoot.y - gameObject.transform.position.y);

        // Combat States
        state.Add(Vector2.Angle(gameObject.transform.up, target.transform.position - gameObject.transform.position));
        state.Add(Time.time - lastShot);

        // Enemy States
        state.Add(enemy.transform.position.x - gameObject.transform.position.x);
        state.Add(enemy.transform.position.y - gameObject.transform.position.y);

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
			gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(act[0] * moveForce, act[1] * moveForce));

            gameObject.transform.Rotate(new Vector3(0, 0, act[2]));
            x = act[0]; y = act[1]; fire = act[3];
            if (act[3] > 0)
            {
                fireArrow();
            }
        }

        if ((Mathf.Abs(gameObject.transform.position.x - home.transform.position.x) > 2) ||
            (Mathf.Abs(gameObject.transform.position.y - home.transform.position.y) > 2))
        {
            done = true;
            reward -= 1;
        }
        else if (Time.time - roundStart > roundTime)
            done = true;

        // reward the player for aiming correctly toward the target
        reward += (180 - Mathf.Abs(Vector2.Angle(gameObject.transform.up, target.transform.position - gameObject.transform.position))) / 180;
        Angle = Mathf.Abs(Vector2.Angle(gameObject.transform.up, target.transform.position - gameObject.transform.position));
        
        // punish the player for being far away from their desired loot
        reward -= (new Vector3(tmpLootPos.x, tmpLootPos.y) - gameObject.transform.position).magnitude / 5.65f;

        if (GetComponent<Rigidbody2D>().velocity.sqrMagnitude > 25)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(GetComponent<Rigidbody2D>().velocity, 5.0f);
        }

        UITimer.text = "Time Remaining: " + (roundTime -(Time.time - roundStart));
    }

    /// <summary>
    /// This function is called at start, when the Academy resets and when the agent is done (if Reset On Done is checked).
    /// </summary>
    public override void AgentReset()
    {
        roundStart = Time.time;
        gameObject.GetComponent<Gold>().value = 0;
        gameObject.transform.position = home.transform.position;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        gameObject.transform.up = new Vector3(0, 1, 0);
        shuffleTarget();
        lastShot = Time.time;

        GameObject[] loot = GameObject.FindGameObjectsWithTag("Gold");
        foreach (GameObject gold in loot)
        {
            if (gold.transform.parent.gameObject == home)
            {
                gold.GetComponent<Gold>().randomizeGold();
            }
        }
    }

    /// <summary>
    /// If Reset On Done is not checked, this function will be called when the agent is done. 
    /// Reset() will only be called when the Academy resets.
    /// </summary>
    public override void AgentOnDone()
    {

    }

    private void shuffleTarget()
    {
        target.transform.position = new Vector3(Random.Range(home.transform.position.x - range, home.transform.position.x + range), Random.Range(home.transform.position.y - range, home.transform.position.y + range), home.transform.position.z);
    }

    private void fireArrow()
    {
        if (Time.time - lastShot < shotDelay)
        {
            reward -= 0.1f;
            return;
        }
        else
            lastShot = Time.time;

        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up * 30);
        //GameObject projectile = GameObject.Instantiate(arrow, gameObject.transform.position, gameObject.transform.rotation, null);
        if (hit.collider != null)
        {
            Debug.DrawLine(gameObject.transform.position, hit.point);
            if (hit.collider.gameObject == target)
            {
                reward += 5;
                //roundStart += 5;
                shuffleTarget();
            }
            else
                reward -= 0.3f;
        }
        else
        {
            reward -= 0.3f;
        }
    }
}
