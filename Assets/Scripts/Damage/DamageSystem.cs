using DefenseLayers;
using DI;
using UnityEngine;
using General;

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
            layer.CurrentBasePoints -= hitData.Damage;

            if (layer.CurrentBasePoints <= 0)
            {
                layer.CurrentBasePoints = 0;
                layer.gameObject.SetActive(false);

                Debug.LogError("Shield Destroyed");
            }
        }

        private void ApplyAsteroidDamage(AsteroidDefenseLayer layer, HitData hitData)
        {
            layer.CurrentBasePoints -= hitData.Damage;

            if (layer.CurrentBasePoints <= 0)
            {
                layer.CurrentBasePoints = 0;
                layer.Collider.enabled = false;
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
            layer.CurrentBasePoints -= hitData.Damage;

            if (layer.CurrentBasePoints <= 0)
            {
                layer.CurrentBasePoints = 0;
                layer.Collider.enabled = false;
                Debug.LogError("Hull Destroyed");
            }
        }
    }
}