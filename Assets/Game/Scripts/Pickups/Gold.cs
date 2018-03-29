using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Pickups
{

    public class Gold : Item
    {

        public Gold()
        {
            itemName = "Gold";
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public override void Randomize()
        {
            base.Randomize();
            // I know this is deterministic, but I'm dialing in the reward values.
            // In the future, the loot drops might scale differently
            value = Random.Range(1, 1);
        }

    }

}
