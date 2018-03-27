using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Game.Scripts.Pickups
{

    public class Item : MonoBehaviour
    {

        public virtual int Value { get; set; } = 1;
        public virtual float RespawnRange { get; set; } = 1;
        public virtual bool IsRespawn { get; set; } = false;

        /// <summary>
        /// Use this for initialization
        /// </summary>
        void Start()
        {
        }

        /// <summary>
        /// Update once per frame
        /// </summary>
        void Update()
        {
        }

        /// <summary>
        /// Do whatever Random Initialization
        /// </summary>
        public virtual void Randomize()
        {
            gameObject.transform.position = new Vector2(transform.parent.position.x + Random.Range(-RespawnRange, RespawnRange),
                                                        transform.parent.position.y + Random.Range(-RespawnRange, RespawnRange));
            // I know this is deterministic, but I'm dialing in the reward values.
            // In the future, the loot drops might scale differently
            Value = Random.Range(1, 1);

            GetComponent<Collider2D>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
        }

        private void Toggle()
        {
            gameObject.SetActive(false);
        }

        private void Pickup(GameObject looter)
        {
            looter.GetComponent<Gold>().value += Value;
            for (int i = 0; i < looter.transform.childCount; i++)
                if (looter.transform.GetChild(i).GetComponent<LooterAgent>())
                    looter.transform.GetChild(i).GetComponent<LooterAgent>().reward += Value;
            if (IsRespawn)
                Randomize();
            else
                Toggle();
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.tag == "Player")
                Pickup(coll.gameObject);
        }

        void OnTriggerStay2D(Collider2D coll)
        {
            if (coll.tag == "Player")
                Pickup(coll.gameObject);
        }


    }


}
