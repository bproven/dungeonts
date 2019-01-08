using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Game.Scripts.Pickups;

public class RewardSettings : MonoBehaviour
{

    public static float prox_gold = 0.1f,
        prox_enemy = 0.1f,
        prox_wall = 0.3f,

        attack_enemy = 15,
        attack_miss = 5,
        aim = 1,
        take_damage = 40.0f,
        collide_enemy = 5,
        collide_wall = 0.6f;

    // original UI setters
    public float TakeDamage { get { return take_damage; } set { take_damage = value; } }
    public float GoldValue { get { return Gold.Value; } set { Gold.Value = (int)value; } }
    public float AttackEnemy { get { return attack_enemy; } set { attack_enemy = value; } }
    public float ProxWall { get { return prox_wall; } set { prox_wall = value; } }

    // item settings dictionary
    public static Dictionary<string, ItemSettings> items = new Dictionary<string, ItemSettings>();

    // item names (keys)
    public const string ArmorName = "Armor";
    public const string BootsName = "Boots";
    public const string BowName = "Bow";
    public const string GoldName = "Gold";
    public const string SwordName = "Sword";

    // initializers for default item settings, by key
    private static Dictionary<string, Action<IItemSettings>> initializers = new Dictionary<string, Action<IItemSettings>>
    {
        { ArmorName, armor => { armor.Value = 1; armor.DamageDeflection = 0.5f; } },
        { BootsName, boots => { boots.Value = 1; boots.SpeedFactor = 1.0f; } },
        { BowName, bow => { bow.Value = 3; bow.RangeBonus = 1.5f; bow.DamageBonus = 1.5f; } },
        { GoldName, gold => { gold.Value = 1; } },
        { SwordName, sword => { sword.Value = 2; sword.DamageBonus = 2.0f; } }
    };

    // getter for all item settings types
    public static ItemSettings Get(string name)
    {
        return Get(name, null);
    }

    public static ItemSettings Get( string name, Action<ItemSettings> init )
    {
        ItemSettings result = null;
        if ( init == null )
        {
            if ( initializers.ContainsKey( name ) )
            {
                init = initializers[name];
            }
        }
        if (items.ContainsKey(name))
        {
            result = items[name];
        }
        else
        {
            result = new ItemSettings(name);
            items[name] = result;
            init?.Invoke(result);
        }
        return result;
    }

    // default item settings
    public ItemSettings Armor
    {
        get
        {
            return Get(ArmorName, initializers[ArmorName] );
        }
    }

    public ItemSettings Boots
    {
        get
        {
            return Get(BootsName, initializers[BootsName] );
        }
    }

    public ItemSettings Bow
    {
        get
        {
            return Get(BowName, initializers[BowName]);
        }
    }

    public ItemSettings Gold
    {
        get
        {
            return Get(GoldName, initializers[GoldName] );
        }
    }

    public ItemSettings Sword
    {
        get
        {
            return Get(SwordName, initializers[SwordName] );
        }
    }

    // UI setters and getters

    // Armor
    public float ArmorDamageDeflection { get { return Armor.DamageDeflection; } set { Armor.DamageDeflection = value; } }

    // Boots
    public float BootsSpeedFactor { get { return Boots.SpeedFactor; } set { Boots.SpeedFactor = value; } }

    // Bow
    public float BowDamageBonus { get { return Bow.DamageBonus; } set { Bow.DamageBonus = value; } }
    public float BowRangeBonus { get { return Bow.RangeBonus; } set { Bow.RangeBonus = value; } }

    // Sword
    public float SwordDamageBonus { get { return Sword.DamageBonus; } set { Sword.DamageBonus = value; } }

}
