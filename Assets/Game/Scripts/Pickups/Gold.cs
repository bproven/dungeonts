using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Scripts.Pickups
{

    public class Gold : Item
    {
        
        public Gold()
        {
            // ignore itemName and always use tag [BP]
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
            //Value = Random.Range(1, 1); // for now let RewardSettings (Getting Treasure Slider) also indicate the value of a Gold drop. [BP]
        }

    }

}
