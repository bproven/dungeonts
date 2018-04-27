using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using Assets.Game.Scripts.Pickups;

public class LooterAgent : Agent
{
    // UI references
    private GameObject resultsWindow;

    // PLAYER SETTINGS, used for default values on agent reset
    public static float HP = 3, TIME = 120;    // Max health, level timer
    public static float DEX = 1.5f;            // Movement speed
    public static float DEFLECTION = 0;       // without armor

    // Base stats
    public float mySpeed;               // Local Speed Stat
    public float myHealth;                // Local Health Stat
    public float myDamageDeflection;

    public float stateReward;

    private static string[] thingsICanSee =
    {
        "Enemy",
        "Wall",
        "Gold",
        "Boots",
        "Armor",
        "Bow",
        "Sword"
    };  // What items is this agent allowed to recognize by tag?

    /// <summary>
    /// The Agent's Speed as modified by Items
    /// </summary>
    private float Speed { get; set; }

    /// <summary>
    /// The Agent's Health as modified by Items
    /// </summary>
    public float Health { get; private set; }

    /// <summary>
    /// The Agent's damage deflection as modified by Items
    /// </summary>
    public float DamageDeflection { get; set; }

    // Set in scene
    public GameObject shooter;      // GameObject that the ArcherAgent script is attached to
    public GameObject levelManager; // GameObject that the LevelSpawner script is attached to

    public ArcherAgent Archer
    {
        get
        {
            return shooter.GetComponent<ArcherAgent>();
        }
    }

    // Vision specific variables
    public float sightDistance;
    public uint numRays;

    // Output debug
    public bool debug;
    public float myReward, x, y;    // Reward, output velocity components

    // Local timers
    private float roundStart;
    private float lastDamage;

    // list of items
    private List<Item> Items { get; } = new List<Item>();

    public Gold Gold
    {
        get
        {
            return Items.FirstOrDefault(i => i.GetType() == typeof(Gold)) as Gold;
        }
    }

    public int GoldValue
    {
        get
        {
            return Gold?.value ?? 0;
        }
        set
        {
            Gold gold = Gold;
            if (gold == null)
            {
                if (value > 0)
                {
                    gold = new Gold();
                    Items.Add(gold);
                }
            }
            else
            {
                if (value == 0)
                {
                    Items.Remove(gold);
                }
                else
                {
                    gold.value = value;
                }
            }
        }
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
        
        // New input, trying to add some kind of memory for confusing situations
        // Hey, past me, where did you want to go again?
        Vector2 dir = transform.parent.GetComponent<Rigidbody2D>().velocity;
        AddVectorObs(Speed != 0 ? dir.x / Speed : 0.0f);
        AddVectorObs(Speed != 0 ? dir.y / Speed : 0.0f);

        // Stats states
        AddVectorObs(Health / HP);   // Health stat
        AddVectorObs(Speed / 5.0f);  // Realistically our max speed shouldn't be over 5, though this could change in time
        AddVectorObs(DamageDeflection);  //  Armor stat, should be 0f-1f
        AddVectorObs(Archer.Range / sightDistance);    // How far can we kill things

        // New loop
        for (int i = 0; i < numRays; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Quaternion.AngleAxis((360f / numRays) * i, Vector3.forward) * transform.up, sightDistance);
            Color collisionColor = Color.white; // Default
            if (hit)
            {
                if (hit.collider.tag == "Gold")
                    stateReward += RewardSettings.prox_gold * (1 - (hit.distance / sightDistance)) / numRays;
                // Reward risk
                if (hit.collider.tag == "Enemy")
                    stateReward += RewardSettings.prox_enemy * (1 - (hit.distance / sightDistance)) / numRays;
                // Quit it with the wall hugging
                if (hit.collider.tag == "Wall")
                    stateReward -= RewardSettings.prox_wall * (Mathf.Pow(1 - (hit.distance / sightDistance), 2)) / numRays;

                if (debug)
                {
                    if (hit.collider.tag == "Enemy") collisionColor = Color.red;
                    else if (hit.collider.tag == "Gold") collisionColor = Color.yellow;
                    else if (hit.collider.tag == "Wall") collisionColor = Color.blue;
                    else collisionColor = Color.magenta;
                    Debug.DrawLine(gameObject.transform.position, hit.point, collisionColor);
                }
            }
            else if (debug)
                Debug.DrawLine(gameObject.transform.position, (gameObject.transform.position + (Quaternion.AngleAxis((360f / numRays) * i, Vector3.forward) * transform.up).normalized * sightDistance), collisionColor);
            AddVectorObs(sightDistance != 0 ? hit.distance / sightDistance : 0.0f);
            // Add information about what was hit
            foreach (string tag in thingsICanSee)
                AddVectorObs(hit && hit.collider.tag == tag ? 1.0f : 0.0f);
        }
        // Could we attack if we needed to?
        float cd = (Time.time - shooter.GetComponent<ArcherAgent>().lastShot) / shooter.GetComponent<ArcherAgent>().myFireRate;
        AddVectorObs(cd >= 1 ? 1 : 0);
        
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
        // Move
        else if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
            gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Clamp(vectorAction[0], -1, 1), Mathf.Clamp(vectorAction[1], -1, 1)) * Speed;
        
        if (Time.time - roundStart > TIME)
        {
            Debug.Log("Looter: Out of time.");
            Done();
        }
        AddReward(stateReward);
        stateReward = 0;
        // Debug
        myReward = GetCumulativeReward();
        SendHUDUpdate();
    }

    private void SendHUDUpdate()
    {
        // UI
        PlayerHUD.time = (TIME - (Time.time - roundStart));
        PlayerHUD.health = Mathf.Max(0, Health / HP);
    }

    /// <summary>
    /// This function is called at start, when the Academy resets and when the agent is done (if Reset On Done is checked).
    /// </summary>
    public override void AgentReset()
    {
        stateReward = 0;
        roundStart = Time.time;
        gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GoldValue = 0;
        transform.up = Vector2.up;
        levelManager.GetComponent<LevelSpawner>().resetLevel();
        lastDamage = Time.time - 0.5f;

        // PLAYER SETTINGS
        myHealth = HP;
        mySpeed = DEX;
        myDamageDeflection = DEFLECTION;
        
        DamageDeflection = myDamageDeflection;
        Health = myHealth;
        Speed = mySpeed;

        Items.Clear();  // TODO: what about starting items?

        Archer.AgentReset();

        transform.parent.position = levelManager.transform.GetChild(0).position;

    }

    void die()
    {
        Done();
        shooter.GetComponent<ArcherAgent>().Done();
        transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        SendHUDUpdate();
    }

    /// <summary>
    /// Called to have the looter take some damage
    /// </summary>
    /// <param name="damage"></param>
    public void looterTakeDamage(float damage)
    {
        if (Time.time - lastDamage > 0.5f)
        {
            // Score reduction
            damage = damage * (1 - DamageDeflection);
            ResultsWindow.Score_DamageTaken(damage);

            stateReward -= RewardSettings.take_damage;
            shooter.GetComponent<ArcherAgent>().stateReward -= RewardSettings.take_damage;
            Debug.Log("Taking Damage! Original: " + damage + " Deflection: " + DamageDeflection + " Outcome: " + damage);
            Health -= damage;
            if (Health <= 0)
                die();
            lastDamage = Time.time;
        }
    }

    /// <summary>
    /// Agent picks up an item and adds it to inventory
    /// </summary>
    /// <param name="item"></param>
    public void Pickup( Item item )
    {
        if ( item == null )
        {
            throw new ArgumentNullException("item");
        }
        string name = item.name;
        string tag = item.tag;
        if ( string.IsNullOrEmpty( name ) )
        {
            name = tag;
        }
        Debug.Log("Picking up " + name);
        stateReward += item.value;
        Health = Mathf.Min(HP, Health + item.healthBonus);    // Health was getting set to full each time it picked up a unique item
        ResultsWindow.Score_GatheredLoot(item.value);
        if ( item.IsCountable )
        {
            Item existingItem = Items.FirstOrDefault(i => i.GetType() == item.GetType());
            if ( existingItem == null )
            {
                existingItem = item;
                existingItem.Count = 1;
                Items.Add(existingItem);
            }
            else
            {
                existingItem.Count++;
            }
        }
        else
        {
            if (!Items.Any(i => i.tag == tag)) // TODO: just make the collection a set
            {
                Debug.Log("Picking up " + tag);
                Items.Add(item);
                UpdateStats();

                ResultsWindow.Score_ObtainedItem();
            }
            else
            {
                Debug.LogWarningFormat("LooterAgent already has a {0}", tag);
            }
        }
    }

    /// <summary>
    /// Recalcs stats when an item is added or removed
    /// </summary>
    private void UpdateStats()
    {
        // FIXME: Health is getting reset to full on item pickups

        float speed = mySpeed;
        float damageDeflection = myDamageDeflection;
        float strength = Archer.myStrength;
        float range = Archer.myRange;
        foreach ( Item item in Items )
        {
            // add bonuses
            speed += mySpeed * item.speedFactor;
            strength += Archer.myStrength * item.damageBonus;
            //damageDeflection = myDamageDeflection * item.damageDeflection;
            damageDeflection = myDamageDeflection + item.damageDeflection;
            range = item.rangeBonus != 0 ? item.rangeBonus : Archer.Range;
        }
        Speed = speed;
        DamageDeflection = damageDeflection;
        Archer.Strength = strength;
        Archer.Range = range;
    }

    /// <summary>
    /// If Reset On Done is not checked, this function will be called when the agent is done. 
    /// Reset() will only be called when the Academy resets.
    /// </summary>
    public override void AgentOnDone()
    {
        resultsWindow.GetComponent<ResultsWindow>().Show();   
    }

    private void Awake()
    {
        resultsWindow = GameObject.Find("ResultsWindow");
        if (!resultsWindow)
            Debug.Log("Failed to find the ResultsWindow GameObject");
    }

}