using Damage;
using Registries;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace Helpers
{
    public class ShipInitHelper
    {
        public static void InitShip(ShipInstance shipInstance)
        {
            ref var movement = ref shipInstance.MovementRuntimeData;
            movement.InertiaDampingIsActive = true;

            ref var movementCharacteristics = ref shipInstance.MovementStats;
            ref var equip = ref shipInstance.Equip;

            var chassisMultipliers = equip.Chassis.ChassisMultipliers;
            var mainEngineMultipliers = equip.MainEngine.MainEngineMultipliers;
            var sideEngineMultipliers = equip.SideEngine.ThrustersMultipliers;

            var totalMass = equip.Chassis.Mass + chassisMultipliers.MassMultiplier * equip.Chassis.Mass;
            shipInstance.Rigidbody.mass = totalMass;

            var totalDirectDrag = equip.Chassis.DirectDrag + chassisMultipliers.DirectDragMultiplier * equip.Chassis.DirectDrag;
            var totalReverseDrag = equip.Chassis.ReverseDrag + chassisMultipliers.ReverseDragMultiplier * equip.Chassis.ReverseDrag;
            var totalStrafeDrag = equip.Chassis.StrafeDrag + chassisMultipliers.StrafeDragMultiplier * equip.Chassis.StrafeDrag;
            var totalRotateDrag = equip.Chassis.RotateDrag + chassisMultipliers.RotateDragMultiplier * equip.Chassis.RotateDrag;

            var worldUnitMod = WorldConfig.WorldUnitMod;
            var inertiaDampingForce = WorldConfig.InertiaDampingForce;
            var mainEngine = equip.MainEngine;
            var sideEngine = equip.SideEngine;

            var totalDirectThrust = mainEngine.DirectThrust + mainEngineMultipliers.DirectThrustMultiplier * mainEngine.DirectThrust;
            var totalReverseThrust = mainEngine.ReverseThrust + mainEngineMultipliers.ReverseThrustMultiplier * mainEngine.ReverseThrust;
            var totalStrafeThrust = sideEngine.StrafeThrust + sideEngineMultipliers.StrafeThrustMultiplier * sideEngine.StrafeThrust;
            var totalRotateThrust = sideEngine.RotateThrust + sideEngineMultipliers.RotateThrustMultiplier * sideEngine.RotateThrust;
            var totalBoostThrust = mainEngine.BoostThrust + mainEngineMultipliers.BoostThrustMultiplier * mainEngine.BoostThrust;

            movementCharacteristics.DirectMaxSpeed = totalDirectThrust / totalDirectDrag * worldUnitMod;
            movementCharacteristics.DirectAcceleration = totalDirectThrust / totalMass * worldUnitMod;
            movementCharacteristics.DirectDampingAcceleration = totalDirectDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.ReverseMaxSpeed = totalReverseThrust / totalReverseDrag * worldUnitMod;
            movementCharacteristics.ReverseAcceleration = totalReverseThrust / totalMass * worldUnitMod;
            movementCharacteristics.ReverseDampingAcceleration = totalReverseDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.StrafeMaxSpeed = totalStrafeThrust / totalStrafeDrag * worldUnitMod;
            movementCharacteristics.StrafeAcceleration = totalStrafeThrust / totalMass * worldUnitMod;
            movementCharacteristics.StrafeDampingAcceleration = totalStrafeDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.RotateSpeed = totalRotateThrust / totalRotateDrag;


            movementCharacteristics.BoostersMaxSpeed = totalBoostThrust / totalDirectDrag * worldUnitMod;
            movementCharacteristics.BoostersAcceleration = totalBoostThrust / totalMass * worldUnitMod;
            movementCharacteristics.BoostersMaxPower = mainEngine.BoostMaxTime;
        }

        public static void UpdateHealthStats(ShipInstance shipInstance)
        {
            if (!shipInstance.TryGetComponent(out HealthComponent component))
            {
                throw new Exception($"HealthComponent NotFound  {shipInstance.gameObject.name}");
            }

            ref var resistanceMultipliers = ref component.ResistanceMultipliers;
            var resistance = component.Resistance;
            var maxResistance = WorldConfig.MaxResistance;
            var energyResistance = Mathf.Clamp(resistance.Energy, 0f, maxResistance);
            var kineticResistance = Mathf.Clamp(resistance.Kinetic, 0f, maxResistance);
            resistanceMultipliers.Energy = 1 - energyResistance;
            resistanceMultipliers.Kinetic = 1 - kineticResistance;
        }

        public static void InitWeapons(ShipInstance shipInstance)
        {
            var shipRb = shipInstance.Rigidbody;

            foreach (var slot in shipInstance.WeaponSlots)
            {
                var weapon = slot.Weapon;

                UpdateWeaponStats(weapon);

                if (weapon is IShipVelocityAware aware)
                {
                    aware.SetShipRigidbody(shipRb);
                }
            }
        }

        public static void UpdateWeaponStats(WeaponBase weaponBase)
        {
            if (weaponBase is BoltRepeater boltRepeater)
            {
                ref var weaponStats = ref boltRepeater.WeaponStats;
                weaponStats.Cached.ShootDelay = 1 / weaponStats.Config.FireRate;
                weaponStats.Cached.InvProjectileSpeed = 1 / weaponStats.Config.ProjectileSpeed;
            }

            ref var damage = ref weaponBase.Damage;
            var baseDamage = weaponBase.BaseDamage;
            var multipliers = weaponBase.DamageMultipliers;
            damage.Energy = multipliers.Energy * baseDamage.Energy;
            damage.Kinetic = multipliers.Kinetic * baseDamage.Kinetic;
            damage.Asteroid = multipliers.Asteroid * (damage.Energy + damage.Kinetic);
        }

        public static void RegisterShip(ShipInstance shipInstance, ShipRegistry shipRegistry, bool asPlayer)
        {
            if (!asPlayer)
            {
                shipRegistry.RequestAddOtherShip(shipInstance);
            }
            else
            {
                shipRegistry.RequestAddPlayerShip(shipInstance);
            }
        }
    }
}

