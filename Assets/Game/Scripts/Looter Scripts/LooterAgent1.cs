using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Game.Scripts.Pickups;

// New Looter Agent which also has control over attacks.
// Hoping the integration goes smoother than it did previously

public class LooterAgent1 : Agent
{
    public int roundTime;
    private float roundStart;
    public float maxSpeed;
    public GameObject enemy;

    public float sightDistance;

    //private Vector2 lootPos;
    private float lastLootCollection;

    public float rotation;
    public float cooldown1;
    public float shotDistance;
    public float shotDelay;
    public float range;

    private float lastShot;
    //private float lastKill;

    private void shuffleTarget(GameObject target)
    {
        target.transform.position = new Vector3(Random.Range(transform.parent.position.x - range, transform.parent.position.x + range), Random.Range(transform.parent.position.y - range, transform.parent.position.y + range), transform.parent.position.z);
    }

    private void fireArrow()
    {
        if (Time.time - lastShot < shotDelay)
        {
            AddReward(-0.5f);
            return;
        }
        else
            lastShot = Time.time;

        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, shotDistance);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + gameObject.transform.up * shotDistance);
        //GameObject projectile = GameObject.Instantiate(arrow, gameObject.transform.position, gameObject.transform.rotation, null);
        if (hit.collider != null)
        {
            Debug.DrawLine(gameObject.transform.position, hit.point);
            if (hit.collider.tag == "Enemy")
            {
                AddReward(25);
                shuffleTarget(hit.collider.gameObject);
                //lastKill = Time.time;

                // Feed a reward into the Movement agent for allowing us to kill it
                AddReward(25);
            }
            else
                AddReward(-0.5f);
        }
        else
        {
            AddReward(-0.5f);
        }
    }


    private Vector2 bestLootPos()
    {
        Vector2 bestLootPos = Vector2.zero;

        GameObject[] loot = GameObject.FindGameObjectsWithTag("Gold");
        // if there is no more loot left... you win? Reset.
        // FIXME: Handle the case where there is no gold on the floor.
        if (loot.Length == 0)
        {
            Debug.Log("Looter: Out of loot.");
            Done();
            return Vector2.zero;
        }

        float valPerDistance = 0;
        float bestVPD = 0;
        foreach(GameObject gold in loot)
        {
            if (gold.transform.position != gameObject.transform.position)
            {
                valPerDistance = gold.GetComponent<Gold>().value /  (gold.transform.position - gameObject.transform.position).magnitude;
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
        //lootPos = bestLootPos;
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
    public override void CollectObservations()
    {
        

        // Old implementation, too many manual variables
        /*
        // Current velocity, for force-based implementations
        AddVectorObs(gameObject.GetComponent<Rigidbody2D>().velocity.x);
        AddVectorObs(gameObject.GetComponent<Rigidbody2D>().velocity.y);

        // Where are the enemies that we need to avoid on the way to sick loot
        Vector2 diff = enemy.transform.position - gameObject.transform.position;
        // How far is this enemy?
        AddVectorObs(diff.magnitude / 6);
        // In what direction is this enemy?
        diff.Normalize();
        AddVectorObs(diff.x);
        AddVectorObs(diff.y);
        // Combine diffs into one angle
        //AddVectorObs(Vector2.SignedAngle(Vector2.up, diff) / 180);

        // Where is the sick loot
        Vector2 bestLoot = bestLootPos();
        Vector2 lootDiff = new Vector3(bestLoot.x, bestLoot.y) - gameObject.transform.position;
        // How far is this enemy?
        AddVectorObs(lootDiff.magnitude / 6);
        // In what direction is this enemy?
        lootDiff.Normalize();
        AddVectorObs(lootDiff.x);
        AddVectorObs(lootDiff.y);
        // Combine diffs into one angle
        //AddVectorObs(Vector2.SignedAngle(Vector2.up, lootDiff) / 180);
        */

        // New implementation, raycasts in 8 directions with information on what they hit
        RaycastHit2D[] rays = new RaycastHit2D[16];
        int counter = 0;
        for (float i = -1; i <= 1; i += 0.5f)
        {
			for (float j = -1; j <= 1; j += 0.5f)
            {
                if ((j != 1 && j != -1) && (i < 1 && i > -1))
                    continue;
                rays[counter] = Physics2D.Raycast(gameObject.transform.position, new Vector2(i, j), sightDistance);
                counter++;
            }
        }
        counter = 0;
        for (float i = -1; i <= 1; i += 0.5f)
        {
			for (float j = -1; j <= 1; j += 0.5f)
            {
                if ((j != 1 && j != -1) && (i < 1 && i > -1))
                    continue;
                // Add the raycast information into the state
                // Add the distance
                AddVectorObs(rays[counter].distance / sightDistance);
                // Add information about what was hit
                if (rays[counter])
                {
                    if (rays[counter].collider.tag == "Gold")
                        AddVectorObs(1.0f);
                    else
                        AddVectorObs(0.0f);

                    if (rays[counter].collider.tag == "Enemy")
                        AddVectorObs(1.0f);
                    else
                        AddVectorObs(0.0f);

                    if (rays[counter].collider.tag == "Wall")
                        AddVectorObs(1.0f);
                    else
                        AddVectorObs(0.0f);
                }
                else
                {
                    // No gold
                    AddVectorObs(0.0f);
                    // No enemy
                    AddVectorObs(0.0f);
                    // No wall
                    AddVectorObs(0.0f);
                }
                counter++;
            }
        }
        // We should probably know what our cooldowns are when moving around the enemy
        cooldown1 = (transform.GetChild(0).GetComponent<ArcherAgent>().shotDelay - (Time.time - transform.GetChild(0).GetComponent<ArcherAgent>().lastShot)) / transform.GetChild(0).GetComponent<ArcherAgent>().shotDelay;
        AddVectorObs(cooldown1);

        
    }

    /// <summary>
    /// This function will be called every frame, you must define what your agent will do given the input actions. 
    /// You must also specify the rewards and whether or not the agent is done. To do so, modify the public fields of the agent reward and done.
    /// </summary>
    /// <param name="vectorAction"></param>
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {

        }
        else if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
        {
            // Give the AI more indirect control, apply force to the player to move them
            // gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(act[0], act[1]));

            // Give the AI more direct control, directly affect the velocity of the player. Allows for more precise movement
            gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.ClampMagnitude(new Vector2(vectorAction[0], vectorAction[1]), maxSpeed);

            // Give the AI a constantly moving player, but let them rotate the direction they move each frame
            //Vector3 move = Quaternion.Euler(0, 0, act[0] * 180) * Vector2.up * maxSpeed;
            //gameObject.GetComponent<Rigidbody2D>().velocity = move;
            //rotation = act[0] * 180;
        }

        // Just in case it somehow breaks physics while it's training
        if ((Mathf.Abs(gameObject.transform.position.x - transform.parent.position.x) > 2) ||
            (Mathf.Abs(gameObject.transform.position.y - transform.parent.position.y) > 2))
        {
            Done();
            Debug.Log("Looter: Out of bounds.");
            AddReward(-1000);
        }
        else if (Time.time - roundStart > roundTime)
        {
            Debug.Log("Looter: Out of time.");
            Done();
        }
    }

    /// <summary>
    /// This function is called at start, when the Academy resets and when the agent is done (if Reset On Done is checked).
    /// </summary>
    public override void AgentReset()
    {
        roundStart = Time.time;
        gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        gameObject.transform.position = transform.parent.position;

        GameObject[] loot = GameObject.FindGameObjectsWithTag("Gold");
        foreach(GameObject gold in loot)
        {
            if (gold.transform.parent == transform.parent)
                gold.GetComponent<Gold>().Randomize();
            shuffleEnemy();
        }
    }

    private void shuffleEnemy()
    {
        enemy.transform.position = new Vector3(Random.Range(transform.parent.position.x - 2, transform.parent.position.x + 2), Random.Range(transform.parent.position.y - 2, transform.parent.position.y + 2), transform.parent.position.z);
    }

    /// <summary>
    /// If Reset On Done is not checked, this function will be called when the agent is done. 
    /// Reset() will only be called when the Academy resets.
    /// </summary>
    public override void AgentOnDone()
    {
    }

}