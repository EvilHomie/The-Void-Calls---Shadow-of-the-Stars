using DefenseLayers;
using Projectiles;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class DamageSystem : GameSystemBase
    {
        public delegate void DamageAction(DefenseLayerBase defenseLayer, in DamageData damageData);

        private readonly Dictionary<DefenseLayerType, DamageAction> _damageStrategies = new();

        protected override void AwakeInit()
        {
            _damageStrategies.Add(DefenseLayerType.Hull, ApplyHullDamage);
            _damageStrategies.Add(DefenseLayerType.Armor, ApplyArmorDamage);
            _damageStrategies.Add(DefenseLayerType.Shield, ApplyShieldDamage);
            _damageStrategies.Add(DefenseLayerType.AsteroidHull, ApplyAsteroidHullDamage);
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

        private void OnBeamHit(in DamageData damageData, DefenseLayerBase layer, Vector2 position)
        {
            var layerType = layer.LayerType;
            _damageStrategies[layerType].Invoke(layer, damageData);

            EventBus.DefenseLayerDamagedAction?.Invoke(layer, position);
        }

        private void OnProjectileHit(Bolt bolt, DefenseLayerBase layer, Vector2 position)
        {
            var layerType = layer.LayerType;
            _damageStrategies[layerType].Invoke(layer, bolt.DamageData);

            EventBus.DefenseLayerDamagedAction?.Invoke(layer, position);
        }


        private void ApplyShieldDamage(DefenseLayerBase defenseLayer, in DamageData damageData)
        {
            var damageMultiplier = defenseLayer.ResistanceMultipliers.Energy;
            defenseLayer.CurrentHealthPoints -= damageData.Energy * damageMultiplier;            

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
                defenseLayer.Collider.enabled = false;
            }
        }

        private void ApplyArmorDamage(DefenseLayerBase defenseLayer, in DamageData damageData)
        {
            var damageMultiplier = defenseLayer.ResistanceMultipliers.Kinetic;
            defenseLayer.CurrentHealthPoints -= damageData.Kinetic * damageMultiplier;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
                defenseLayer.Collider.enabled = false;
            }
        }
        private void ApplyHullDamage(DefenseLayerBase defenseLayer, in DamageData damageData)
        {
            var energyMultiplier = defenseLayer.ResistanceMultipliers.Energy;
            var energyDamage = damageData.Energy * energyMultiplier;
            var kineticMultiplier = defenseLayer.ResistanceMultipliers.Kinetic;
            var kineticDamage = damageData.Kinetic * kineticMultiplier;
            defenseLayer.CurrentHealthPoints -= energyDamage + kineticDamage;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
            }
        }

        private void ApplyAsteroidHullDamage(DefenseLayerBase defenseLayer, in DamageData damageData)
        {
            defenseLayer.CurrentHealthPoints -= damageData.Asteroid;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
            }
        }
    }
}