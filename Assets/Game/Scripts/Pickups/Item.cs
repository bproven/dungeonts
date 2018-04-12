using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Game.Scripts.Pickups
{

    /// <summary>
    /// Base class for Items
    /// </summary>
    public class Item : MonoBehaviour
    {

        public string itemName = string.Empty;
        public int value = 1;

        public float speedFactor = 0.0f;    // factor to add speed
        public float healthBonus = 0.0f;
        public float damageBonus = 0.0f;
        public float damageDeflection = 0.0f;
        public float rangeBonus = 0.0f;

        public float respawnRange = 1.0f;
        public bool respawn = false;

        public bool IsCountable { get; set; } = false;
        public virtual int Count { get; set; } = 1;

        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Update once per frame
        /// </summary>
        void Update()
        {
        }

        /// <summary>
        /// Initialize the Script Component
        /// </summary>
        public virtual void Initialize()
        {
            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            if (RewardSettings.item_values.ContainsKey(tag))
                value = RewardSettings.item_values[tag];
            //Randomize();
        }

        /// <summary>
        /// Do whatever Random Initialization
        /// </summary>
        public virtual void Randomize()
        {
            gameObject.transform.position = new Vector2(transform.parent.position.x + Random.Range(-respawnRange, respawnRange),
                                                        transform.parent.position.y + Random.Range(-respawnRange, respawnRange));
        }

        /// <summary>
        /// Deactivate the gameObject
        /// Used for items that are respawnable but won't respawn
        /// </summary>
        private void Toggle()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Destroy a gameObject
        /// Used for non-respawnable loot (boots, shield, etc.)
        /// </summary>
        private void Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Pickup this game object
        /// </summary>
        /// <param name="looter">The looter that is picking up the game object.</param>
        private void Pickup(GameObject looter)
        {
            for (int i = 0; i < looter.transform.childCount; i++)
            {
                LooterAgent looterAgent = looter.transform.GetChild(i).GetComponent<LooterAgent>();
                looterAgent?.Pickup(this);
            }
            if (respawn)
            {
                Randomize();
            }
            else
            {
                Toggle();
            }
        }

        /// <summary>
        /// Sent when a Collider collides with this 
        /// </summary>
        /// <param name="coll">The other collider</param>
        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.tag == "Player")
                Pickup(coll.gameObject);
        }

        /// <summary>
        /// Sent when a Collider stays within this trigger collider
        /// </summary>
        /// <param name="coll">The other collider</param>
        void OnTriggerStay2D(Collider2D coll)
        {
            if (coll.tag == "Player")
                Pickup(coll.gameObject);
        }

    }

}
