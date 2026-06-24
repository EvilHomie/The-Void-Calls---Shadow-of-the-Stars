using DefenseLayers;
using System.Collections.Generic;
using Weapons;

namespace CoreGameSystems
{
    public class DamageSystem : GameSystemBase
    {
        public delegate void DamageAction(DefenseLayerBase defenseLayer, WeaponRuntimeDamage damageData);

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
            EventBus.HitAction += OnHit;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.HitAction -= OnHit;
        }       

        private void OnHit(WeaponRuntimeDamage damageData, DefenseLayerBase layer, HitData hitData)
        {
            var layerType = layer.LayerType;
            _damageStrategies[layerType].Invoke(layer, damageData);

            EventBus.DefenseLayerHitAction?.Invoke(layer, hitData);
        }


        private void ApplyShieldDamage(DefenseLayerBase defenseLayer, WeaponRuntimeDamage damageData)
        {
            defenseLayer.CurrentHealthPoints -= damageData.DamageShield;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
                defenseLayer.Collider.enabled = false;
            }
        }

        private void ApplyArmorDamage(DefenseLayerBase defenseLayer, WeaponRuntimeDamage damageData)
        {
            defenseLayer.CurrentHealthPoints -= damageData.DamageArmor;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
                defenseLayer.Collider.enabled = false;
            }
        }
        private void ApplyHullDamage(DefenseLayerBase defenseLayer, WeaponRuntimeDamage damageData)
        {
            defenseLayer.CurrentHealthPoints -= damageData.DamageHull;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
            }
        }

        private void ApplyAsteroidHullDamage(DefenseLayerBase defenseLayer, WeaponRuntimeDamage damageData)
        {
            defenseLayer.CurrentHealthPoints -= damageData.DamageAsteroid;

            if (defenseLayer.CurrentHealthPoints <= 0)
            {
                defenseLayer.CurrentHealthPoints = 0;
            }
        }
    }
}