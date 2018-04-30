using System;

namespace Assets.Game.Scripts.Pickups
{

    public interface IItemSettings
    {
        string ItemName { get; set; }
        int Value { get; set; }
        float SpeedFactor { get; set; }    // factor to add speed
        float HealthBonus { get; set; }
        float DamageBonus { get; set; }
        float DamageDeflection { get; set; }
        float RangeBonus { get; set; }
    }

    public class ItemSettings : IItemSettings
    {

        public string ItemName { get; set; }
        public int Value { get; set; }
        public float SpeedFactor { get; set; }    // factor to add speed
        public float HealthBonus { get; set; }
        public float DamageBonus { get; set; }
        public float DamageDeflection { get; set; }
        public float RangeBonus { get; set; }

        public ItemSettings( string name )
        {
            if ( string.IsNullOrEmpty( name ) )
            {
                throw new ArgumentNullException("name");
            }
            ItemName = name;
        }

    }

}
