using Asteroids;
using CoreGameSystems;
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
            InitShipDefenceLayers(shipInstance);
            InitWeapons(shipInstance);
            InitEngines(shipInstance);
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

            var worldUnitMod = GameConfig.WorldUnitMod;
            var inertiaDampingForce = GameConfig.InertiaDampingForce;
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

        private static void InitShipDefenceLayers(ShipInstance shipInstance)
        {
            var shield = shipInstance.Shield;
            shield.Init(100, 1);
            shield.gameObject.layer = GameLayers.ShipLayer;
            shipInstance.OwnColliders.Add(shield.Collider);

            var hull = shipInstance.Hull;
            var hullHP = shipInstance.Equip.Chassis.Hull;
            hull.Init(hullHP, 100);

            hull.gameObject.layer = GameLayers.ShipLayer;
            shipInstance.OwnColliders.Add(hull.Collider);
        }

        public static void InitAsteroid(Asteroid asteroid)
        {
            var rigidBody = asteroid.Rigidbody;
            //asteroid.BodySprite.sortingOrder = SpriteSortingOrders.Asteroid;           

            if (asteroid.AsteroidType.Contains(AsteroidType.Cluster))
            {
                rigidBody.angularDamping = GameConfig.ClusterAsteroidAngularDamping;
                rigidBody.linearDamping = GameConfig.ClusterAsteroidLinearDamping;
            }
            else
            {
                rigidBody.angularDamping = GameConfig.DriftingAsteroidAngularDamping;
                rigidBody.linearDamping = GameConfig.DriftingAsteroidLinearDamping;
            }

            var massMod = GetMassModifier(asteroid.AsteroidType);
            var scale = asteroid.Transform.localScale.x;
            var baseMass = GameConfig.AsteroidBaseMass * scale * scale;
            var mass = massMod * baseMass;
            rigidBody.mass = mass;

            var hp = mass * GameConfig.AsteroidTonHP;
            asteroid.AsteroidHullLayer.Init(hp);
            asteroid.gameObject.layer = GameLayers.AsteroidsLayer;
        }

        public static float GetMassModifier(AsteroidType asteroidType)
        {
            float sum = 0f;
            int count = 0;

            foreach (var pair in GameConfig.AsteroidMassModByType)
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
                UpdateWeaponStats(weapon);
                weapon.InitBase(shipInstance.AimData, shipInstance.Size, shipInstance.OwnColliders, 100);
                weapon.gameObject.layer = GameLayers.WeaponLayer;

                if (weapon.TryGetComponent<HullDefenseLayer>(out var hullDefenseLayer))
                {
                    shipInstance.OwnColliders.Add(hullDefenseLayer.Collider);
                }

                if (weapon is IRigidBodyDependentWeapon dependentWeapon)
                {
                    dependentWeapon.Init(shipRb);
                }
            }
        }

        public static void InitEngines(ShipInstance shipInstance)
        {
            ref var view = ref shipInstance.MovementView;
            view.LastMainEnginePowerValue = -100f;
            view.LastBoostersState = true;

            var newThrustersPower = new ThrustersPower() // значения -100 сугубо для инициализации
            {
                FrontLeft = -100,
                FrontRight = -100f,
                BackLeft = -100f,
                BackRight = -100f,
            };
            view.ThrustersPower = newThrustersPower;

            foreach (var plume in view.MainEnginesPlumes) plume.Init();
            foreach (var plume in view.BoostersPlumes) plume.Init();
            foreach (var plume in view.ThrustersFL) plume.Init();
            foreach (var plume in view.ThrustersFR) plume.Init();
            foreach (var plume in view.ThrustersBL) plume.Init();
            foreach (var plume in view.ThrustersBR) plume.Init();
        }

        public static void UpdateWeaponStats(WeaponBase weaponBase)
        {
            var baseAimStats = weaponBase.BaseAimStats;
            weaponBase.RuntimeAimStats = baseAimStats;
            var baseDamage = weaponBase.BaseDamage;
            ref var runtimeDamage = ref weaponBase.RuntimeDamage;
            runtimeDamage.DamageArmor = baseDamage.DamageKinetic;
            runtimeDamage.DamageShield = baseDamage.DamageEnergy;
            runtimeDamage.DamageHull = baseDamage.DamageKinetic + baseDamage.DamageEnergy;
            runtimeDamage.DamageAsteroid = runtimeDamage.DamageHull * baseDamage.AsteroidMultiplier;

            if (weaponBase is BoltRepeater boltRepeater)
            {
                var baseFireStats = boltRepeater.BaseFireStats;
                boltRepeater.RuntimeFireStats = baseFireStats;
                ref var logicStats = ref boltRepeater.LogicStats;
                logicStats.ShootDelay = 1f / baseFireStats.FireRate;
                var invProjectileSpeed = 1f / baseFireStats.ProjectileSpeed;
                logicStats.InvProjectileSpeed = invProjectileSpeed;
                logicStats.ProjectileLifeTime = baseAimStats.MaxDistance * invProjectileSpeed;
                logicStats.SpreadTimeMultiplier = baseFireStats.SpreadAngle / 100;
            }
            else if (weaponBase is MiningDrill miningDrill)
            {
                miningDrill.HitDelay = 1f / GameConfig.ConstantBeamHitRate;
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

