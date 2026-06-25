using DefenseLayers;
using System;
using UnityEngine;
using Weapons;

namespace CoreGameSystems
{
    public class HitRegistrationSystem : MonoBehaviour
    {
        public void RegisterHit(Collider2D collider, Vector2 position, SizeType size, in WeaponRuntimeDamage damageData)
        {
            var hitData = new HitData
            {
                Position = position,
                Size = size
            };

            collider.TryGetComponent(out DefenseLayerBase defenceLayer);

            switch (defenceLayer)
            {
                case ShieldDefenseLayer shield:
                    hitData.Damage = damageData.DamageShield;
                    EventBus.ShieldDamagedAction?.Invoke(shield, hitData);
                    break;

                case HullDefenseLayer hull:

                    if (hull.CurrentArmorPoints > 0)
                    {
                        hitData.Damage = damageData.DamageArmor;
                        EventBus.ArmorDamagedAction?.Invoke(hull, hitData);
                    }
                    else
                    {
                        hitData.Damage = damageData.DamageHull;
                        EventBus.HullDamagedAction?.Invoke(hull, hitData);
                    }
                    break;

                case AsteroidDefenseLayer asteroid:
                    hitData.Damage = damageData.DamageAsteroid;
                    EventBus.AsteroidDamagedAction?.Invoke(asteroid, hitData);
                    break;
            }
        }

        public void RegisterHitDynamic(Collider2D collider, Vector2 position, SizeType size, in WeaponRuntimeDamage damageData, float hitDelay)
        {
            var hitData = new HitData
            {
                Position = position,
                Size = size
            };

            collider.TryGetComponent(out DefenseLayerBase defenceLayer);

            switch (defenceLayer)
            {
                case ShieldDefenseLayer shield:
                    hitData.Damage = damageData.DamageShield * hitDelay;
                    EventBus.ShieldDamagedAction?.Invoke(shield, hitData);
                    break;

                case HullDefenseLayer hull:

                    if (hull.CurrentArmorPoints > 0)
                    {
                        hitData.Damage = damageData.DamageArmor * hitDelay;
                        EventBus.ArmorDamagedAction?.Invoke(hull, hitData);
                    }
                    else
                    {
                        hitData.Damage = damageData.DamageHull * hitDelay;
                        EventBus.HullDamagedAction?.Invoke(hull, hitData);
                    }
                    break;

                case AsteroidDefenseLayer asteroid:
                    hitData.Damage = damageData.DamageAsteroid * hitDelay;
                    EventBus.AsteroidDamagedAction?.Invoke(asteroid, hitData);
                    break;
            }
        }
    }

    [Serializable]
    public struct HitData
    {
        public float Damage;
        public Vector2 Position;
        public SizeType Size;
    }
}
