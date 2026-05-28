using Asteroids;
using DefenseLayers;
using Registries;
using Ships;
using UnityEngine;
using Weapons;

namespace Helpers
{
    public class InitHelper
    {
        public static void InitShip(ShipInstance shipInstance)
        {
            InitShipStats(shipInstance);
            InitDefenceLayers(shipInstance);
            InitWeapons(shipInstance);
        }

        private static void InitShipStats(ShipInstance shipInstance)
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

        private static void InitDefenceLayers(ShipInstance shipInstance)
        {
            //shipInstance.BodySprite.sortingOrder = SpriteSortingOrders.Ship;

            var resistanceStats = shipInstance.ResistanceStats;
            var maxResistance = WorldConfig.MaxResistance;
            var energyResistance = Mathf.Clamp(resistanceStats.Energy, 0f, maxResistance);
            var kineticResistance = Mathf.Clamp(resistanceStats.Kinetic, 0f, maxResistance);

            var resistanceMultipliers = new ResistanceMultipliers
            {
                Energy = 1 - energyResistance,
                Kinetic = 1 - kineticResistance
            };

            foreach (var defenseLayer in shipInstance.DefenseLayers)
            {
                defenseLayer.ResistanceMultipliers = resistanceMultipliers;                

                if (defenseLayer.LayerType == DefenseLayerType.Hull)
                {
                    var hp = shipInstance.Equip.Chassis.Hull;
                    defenseLayer.Init(hp);
                }
                else if (defenseLayer.LayerType == DefenseLayerType.Shield)
                {
                    defenseLayer.Init(2000);
                }
                else //if (defenseLayer.LayerType == DefenseLayerType.Armor)
                {
                    defenseLayer.Init(2000);
                }

                shipInstance.IgnoredColliders.Add(defenseLayer.Collider);
            }
        }

        public static void InitAsteroid(Asteroid asteroid)
        {
            var rigidBody = asteroid.Rigidbody;
            //asteroid.BodySprite.sortingOrder = SpriteSortingOrders.Asteroid;           

            if (asteroid.AsteroidType.Contains(AsteroidType.Cluster))
            {
                rigidBody.angularDamping = WorldConfig.ClusterAsteroidAngularDamping;
                rigidBody.linearDamping = WorldConfig.ClusterAsteroidLinearDamping;
            }
            else
            {
                rigidBody.angularDamping = WorldConfig.DriftingAsteroidAngularDamping;
                rigidBody.linearDamping = WorldConfig.DriftingAsteroidLinearDamping;
            }

            var massMod = GetMassModifier(asteroid.AsteroidType);
            var scale = asteroid.Transform.localScale.x;
            var baseMass = WorldConfig.AsteroidBaseMass * scale * scale;
            var mass = massMod * baseMass;
            rigidBody.mass = mass;

            ref var resistanceMultipliers = ref asteroid.AsteroidHullLayer.ResistanceMultipliers;
            resistanceMultipliers.Energy = 1;
            resistanceMultipliers.Kinetic = 1;

            var hp = mass * WorldConfig.AsteroidTonHP;
            asteroid.AsteroidHullLayer.Init(hp);
        }

        public static float GetMassModifier(AsteroidType asteroidType)
        {
            float sum = 0f;
            int count = 0;

            foreach (var pair in WorldConfig.AsteroidMassModByType)
            {
                if (!asteroidType.Contains(pair.Key)) continue;

                sum += pair.Value;
                count++;
            }

            return count == 0 ? 1f : sum / count;
        }

        public static void InitWeapons(ShipInstance shipInstance)
        {
            var shipRb = shipInstance.Rigidbody;

            foreach (var slot in shipInstance.WeaponSlots)
            {
                var weapon = slot.Weapon;
                weapon.IgnoredColliders = shipInstance.IgnoredColliders;
                UpdateWeaponStats(weapon);
                weapon.Init(shipInstance.AimData);

                if (weapon is IShipVelocityAware aware)
                {
                    aware.SetShipRigidbody(shipRb);
                }
            }
        }

        public static void UpdateWeaponStats(WeaponBase weaponBase)
        {
            ref var baseStats = ref weaponBase.BaseStats;

            if (weaponBase is BoltRepeater boltRepeater)
            {
                ref var weaponData = ref boltRepeater.Data;                
                weaponData.ShootDelay = 1f / weaponData.FireRate;
                var invProjectileSpeed = 1f / weaponData.ProjectileSpeed;
                weaponData.InvProjectileSpeed = invProjectileSpeed;
                weaponData.ProjectileLifeTime = baseStats.MaxDistance * invProjectileSpeed;
            }
            else if ((weaponBase is MiningDrill miningDrill))
            {
                ref var weaponData = ref miningDrill.Data;
                weaponData.HitDelay = 1f / weaponData.HitRate;
                weaponData.NoHitTargetPoint = Vector2.up * baseStats.MaxDistance;
            }

            ref var damage = ref weaponBase.CurrentDamageData;
            damage.Energy = baseStats.EnergyDamageMultipliers * baseStats.BaseDamageEnergy;
            damage.Kinetic = baseStats.KineticDamageMultipliers * baseStats.BaseDamageKinetic;
            damage.Asteroid = baseStats.AsteroidDamageMultipliers * (damage.Energy + damage.Kinetic);
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

    public static class EntityIdGenerator
    {
        private static uint _nextId = 1;
        
        public static uint Generate()
        {            
            return _nextId++;
        }
    }
}

