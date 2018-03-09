using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAgent : Agent
{
    [Header("Specific to ArchetTesting")]
    public GameObject target;
    public Transform home;
    public GameObject arrow;
    public int range;
    public float turnSpeed;
	public float shotDistance;

    public float shotDelay;
    public float lastShot;
    private float lastKill;
    private float startTime = 0;
    public float roundTime;

    public float currentAngle;

    public float x, y, fire;

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        //state.Add(gameObject.transform.rotation.z);
        state.Add(Vector2.SignedAngle(gameObject.transform.up, target.transform.position - gameObject.transform.position) / 180);
        state.Add((Time.time - lastShot) / shotDelay);
        return state;
    }

    private void shuffleTarget()
    {
        target.transform.position = new Vector3(Random.Range(home.position.x - range, home.position.x + range), Random.Range(home.position.y - range, home.position.y + range), home.position.z);
    }

    private void punishMiss()
    {
        reward -= 0.5f;
        transform.parent.GetComponent<LooterAgent>().reward -= 0.5f;
    }

    private void fireArrow()
    {
        if (Time.time - lastShot < shotDelay)
        {
            reward -= 0.5f;
            return;
        }
        else
            lastShot = Time.time;

		RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, gameObject.transform.up, shotDistance);
		Debug.DrawLine (gameObject.transform.position, gameObject.transform.position + gameObject.transform.up * shotDistance);
        //GameObject projectile = GameObject.Instantiate(arrow, gameObject.transform.position, gameObject.transform.rotation, null);
        if (hit.collider != null)
        {
            Debug.DrawLine(gameObject.transform.position, hit.point);
            if (hit.collider.gameObject == target)
            {
                reward += 25;
                shuffleTarget();
                lastKill = Time.time;

                // Feed a reward into the Movement agent for allowing us to kill it
                gameObject.transform.parent.GetComponent<LooterAgent>().reward += 25;
            }
            else
                punishMiss();
        }
        else
        {
            punishMiss();
        }
    }

    // to be implemented by the developer
    public override void AgentStep(float[] act)
    {
        if (brain.brainParameters.actionSpaceType == StateType.discrete)
        {
            switch ((int)act[0])
            {
                case 1:
                    gameObject.transform.Rotate(new Vector3(0, 0, 1 * turnSpeed * Time.deltaTime));
                    break;
                case 2:
                    gameObject.transform.Rotate(new Vector3(0, 0, -1 * turnSpeed * Time.deltaTime));
                    break;
                case 3:
                    {
                        fireArrow();
                    }
                    break;
            }
            
            // Test case for out-of-bounds condition when implementing movement in the future
            if ((Mathf.Abs(gameObject.transform.position.x - home.position.x) > range) || (Mathf.Abs(gameObject.transform.position.y - home.position.y) > range))
            {

            }
        }
        else if (brain.brainParameters.actionSpaceType == StateType.continuous)
        {

            gameObject.transform.Rotate(new Vector3(0, 0, act[0] * turnSpeed * Time.deltaTime));
            x = act[0]; fire = act[1];
            if (act[1] > 0)
            {
                fireArrow();
            }
        }
        reward += (180 - Vector2.Angle(gameObject.transform.up, target.transform.position - gameObject.transform.position)) / 180;
        currentAngle = 180 - Vector2.Angle(gameObject.transform.up, target.transform.position - gameObject.transform.position);
        //else
        //    reward += 2;

        // punish the player for leaving enemies alive, rather than killing them
        if (Time.time - lastKill > shotDelay)
        {
            transform.parent.GetComponent<LooterAgent>().reward -= 0.1f;
            reward -= 0.1f;
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
        shuffleTarget();
        startTime = Time.time;
        lastShot = Time.time;
        lastKill = Time.time;
    }
}
