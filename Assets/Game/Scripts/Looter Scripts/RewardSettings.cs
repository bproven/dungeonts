using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardSettings : MonoBehaviour {
    public static float prox_gold = 0.1f,
        prox_enemy = 0.1f,
        prox_wall = 0.3f,

        attack_enemy = 15,
        attack_miss = 5,
        aim = 1,
        take_damage = 40,
        collide_enemy = 5,
        collide_wall = 0.6f;

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
