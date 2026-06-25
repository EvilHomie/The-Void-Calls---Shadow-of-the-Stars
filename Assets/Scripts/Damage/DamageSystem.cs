using DefenseLayers;
using DI;
using UnityEngine;

namespace CoreGameSystems
{
    public class DamageSystem : MonoBehaviour
    {
        [Inject]
        public void Construct()
        {
            EventBus.ShieldDamagedAction += ApplyShieldDamage;
            EventBus.ArmorDamagedAction += ApplyArmorDamage;
            EventBus.HullDamagedAction += ApplyHullDamage;
            EventBus.AsteroidDamagedAction += ApplyAsteroidDamage;
        }

        private void ApplyShieldDamage(ShieldDefenseLayer layer, HitData hitData)
        {
            layer.CurrentPoints -= hitData.Damage;

            if (layer.CurrentPoints <= 0)
            {
                layer.CurrentPoints = 0;
                layer.Collider.enabled = false;

                Debug.LogError("Shield Destroyed");
            }
        }

        private void ApplyAsteroidDamage(AsteroidDefenseLayer layer, HitData hitData)
        {
            layer.CurrentPoints -= hitData.Damage;

            if (layer.CurrentPoints <= 0)
            {
                layer.CurrentPoints = 0;

                Debug.LogError("Asteroid Destoyed");
            }
        }

        private void ApplyArmorDamage(HullDefenseLayer layer, HitData hitData)
        {
            layer.CurrentArmorPoints -= hitData.Damage;

            if (layer.CurrentArmorPoints <= 0)
            {
                layer.CurrentArmorPoints = 0;

                Debug.LogError("Armor Destroyed");
            }
        }

        private void ApplyHullDamage(HullDefenseLayer layer, HitData hitData)
        {
            layer.CurrentHullPoints -= hitData.Damage;

            if (layer.CurrentHullPoints <= 0)
            {
                layer.CurrentHullPoints = 0;

                Debug.LogError("Hull Destroyed");
            }
        }
    }
}