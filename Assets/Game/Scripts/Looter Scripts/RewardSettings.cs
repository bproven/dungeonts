using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSettings : MonoBehaviour {
    public static float prox_gold,
        prox_enemy,
        prox_wall,

        attack_enemy,
        attack_miss,
        aim,
        take_damage,
        collide_enemy,
        collide_wall;

    public static Dictionary<string, int> item_values = new Dictionary<string, int>();
}
