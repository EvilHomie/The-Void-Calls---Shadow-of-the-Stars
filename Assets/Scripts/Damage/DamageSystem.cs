using Damage;
using Projectiles;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class DamageSystem : GameSystemBase
    {
        public delegate void DamageAction(HealthComponent healthComponent, in DamageData damageData);

        private Dictionary<ObjectType, DamageAction> _damageStrategies = new();

        protected override void AwakeInit()
        {
            _damageStrategies.Add(ObjectType.Asteroid, ApplyDamageAsteroid);
            _damageStrategies.Add(ObjectType.Ship, ApplyDamageShip);
            _damageStrategies.Add(ObjectType.Station, ApplyDamageStation);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.BoltHitAction += OnProjectileHit;
            EventBus.BeamHitAction += OnBeamHit;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.BoltHitAction -= OnProjectileHit;
            EventBus.BeamHitAction -= OnBeamHit;
        }

        private void OnBeamHit(in DamageData damageData, Collider2D collider2D, Vector2 position)
        {
            if (!collider2D.TryGetComponent(out HitBox hitBox)) return;

            if (hitBox.HealthComponent.Health.HullPoints <= 0) return;

            var objectType = hitBox.HealthComponent.ObjectType;
            _damageStrategies[objectType].Invoke(hitBox.HealthComponent, damageData);
        }

        private void OnProjectileHit(Bolt bolt, Collider2D collider2D, Vector2 position)
        {
            if (!collider2D.TryGetComponent(out HitBox hitBox)) return;

            if (hitBox.HealthComponent.Health.HullPoints <= 0) return;

            var objectType = hitBox.HealthComponent.ObjectType;
            _damageStrategies[objectType].Invoke(hitBox.HealthComponent, bolt.DamageData);
        }

        private void ApplyDamageAsteroid(HealthComponent healthComponent, in DamageData damageData)
        {
            ref var healthData = ref healthComponent.Health;
            healthData.HullPoints -= damageData.Asteroid;

            if (healthData.HullPoints <= 0)
            {
                healthData.HullPoints = 0;
                healthComponent.HullDestroyedAction?.Invoke();
            }
        }

        private void ApplyDamageShip(HealthComponent healthComponent, in DamageData damageData)
        {
            ref var healthData = ref healthComponent.Health;

            if (healthData.ShieldPoints > 0)
            {
                ApplyShieldDamage(healthComponent, damageData);
                return;
            }

            if (healthData.ArmorPoints > 0)
            {
                ApplyArmorDamage(healthComponent, damageData);
                return;
            }

            ApplyHullDamage(healthComponent, damageData);
        }

        private void ApplyDamageStation(HealthComponent healthComponent, in DamageData damageData)
        {
            ref var healthData = ref healthComponent.Health;

            if (healthData.ShieldPoints > 0)
            {
                ApplyShieldDamage(healthComponent, damageData);
                return;
            }

            ApplyHullDamage(healthComponent, damageData);
        }

        private void ApplyShieldDamage(HealthComponent healthComponent, in DamageData damageData)
        {
            ref var healthData = ref healthComponent.Health;
            var resistanceMultipliers = healthComponent.ResistanceMultipliers;

            healthData.ShieldPoints -= damageData.Energy * resistanceMultipliers.Energy;

            if (healthData.ShieldPoints <= 0)
            {
                healthData.ShieldPoints = 0;
                healthComponent.ShieldDestroyedAction?.Invoke();
            }
        }
        private void ApplyArmorDamage(HealthComponent healthComponent, in DamageData damageData)
        {
            ref var healthData = ref healthComponent.Health;
            var resistanceMultipliers = healthComponent.ResistanceMultipliers;

            healthData.ArmorPoints -= damageData.Kinetic * resistanceMultipliers.Kinetic;

            if (healthData.ArmorPoints <= 0)
            {
                healthData.ArmorPoints = 0;
                healthComponent.ArmorDestroyedAction?.Invoke();
            }

        }
        private void ApplyHullDamage(HealthComponent healthComponent, in DamageData damageData)
        {
            ref var healthData = ref healthComponent.Health;
            var resistanceMultipliers = healthComponent.ResistanceMultipliers;

            var energyDamage = damageData.Energy * resistanceMultipliers.Energy;
            var kineticDamage = damageData.Kinetic * resistanceMultipliers.Kinetic;
            var hullDamage = energyDamage + kineticDamage;
            healthData.HullPoints -= hullDamage;

            if (healthData.HullPoints <= 0)
            {
                healthData.HullPoints = 0;
                healthComponent.HullDestroyedAction?.Invoke();
            }

        }
    }
}