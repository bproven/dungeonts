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


    public void AdjustProxGold(float newProx_gold)
    {
        prox_gold = newProx_gold;
    }

    public void AdjustAttackEnemy(float newAttack_enemy)
    {
        attack_enemy = newAttack_enemy;
    }

    public void AdjustTakeDamage(float newTake_damage)
    {
        take_damage = newTake_damage;
    }

    public void AdjustProxWall(float newProx_wall)
    {
        prox_wall = newProx_wall;
    }


}
