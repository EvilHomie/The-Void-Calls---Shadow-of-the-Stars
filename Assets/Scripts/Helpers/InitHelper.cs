using Asteroids;
using Configs;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace Helpers
{
    public class InitHelper
    {
        public static void InitShip(ShipInstance shipInstance)
        {
            var chassisStats = GameConfig.ChassisBaseStats.GetStats(shipInstance.Id);
            var size = GameConfig.ChassisBaseStats.GetShipSizeType(shipInstance.Id);

            SetupMovement(shipInstance, chassisStats, size);
            SetupShipDefenceLayers(shipInstance, chassisStats, size);
            InitWeapons(shipInstance);
            InitEngines(shipInstance);
        }

        private static void SetupMovement(ShipInstance shipInstance, in ChassisBaseStats chassisStats, SizeType size)
        {
            ref var movement = ref shipInstance.MovementRuntimeData;
            movement.InertiaDampingIsActive = true;

            ref var movementCharacteristics = ref shipInstance.MovementStats;
            ref var equip = ref shipInstance.Equip;

            var mainEnginesStats = GameConfig.MainEnginesBaseStats.GetStats(shipInstance.Equip.MainEngineId, size);
            var thrustersStats = GameConfig.ThrustersBaseStats.GetStats(shipInstance.Equip.ThrusterId, size);


            var worldUnitMod = GameConfig.WorldUnitMod;
            var inertiaDampingForce = GameConfig.InertiaDampingForce;

            var mass = chassisStats.Mass;
            var directDrag = chassisStats.DirectDrag;
            var reverseDrag = chassisStats.ReverseDrag;
            var strafeDrag = chassisStats.StrafeDrag;

            var directThrust = mainEnginesStats.DirectThrust;
            var reverseThrust = mainEnginesStats.ReverseThrust;
            var boostThrust = mainEnginesStats.BoostThrust;
            var boostMaxTime = mainEnginesStats.BoostMaxTime;
            var boostRechargeSpeed = mainEnginesStats.BoostRechargeSpeed;

            var strafeThrust = thrustersStats.StrafeThrust;
            var rotateThrust = thrustersStats.RotateThrust;

            shipInstance.Rigidbody.mass = mass;

            movementCharacteristics.DirectMaxSpeed = directThrust / directDrag * worldUnitMod;
            movementCharacteristics.DirectAcceleration = directThrust / mass * worldUnitMod;
            movementCharacteristics.DirectDampingAcceleration = directDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.ReverseMaxSpeed = reverseThrust / reverseDrag * worldUnitMod;
            movementCharacteristics.ReverseAcceleration = reverseThrust / mass * worldUnitMod;
            movementCharacteristics.ReverseDampingAcceleration = reverseDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.StrafeMaxSpeed = strafeThrust / strafeDrag * worldUnitMod;
            movementCharacteristics.StrafeAcceleration = strafeThrust / mass * worldUnitMod;
            movementCharacteristics.StrafeDampingAcceleration = strafeDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.RotateSpeed = rotateThrust / chassisStats.RotateDrag;


            movementCharacteristics.BoostersMaxSpeed = boostThrust / directDrag * worldUnitMod + movementCharacteristics.DirectMaxSpeed;
            movementCharacteristics.BoostersAcceleration = boostThrust / mass * worldUnitMod + movementCharacteristics.DirectAcceleration;
            movementCharacteristics.BoostersMaxPower = boostMaxTime;
            movementCharacteristics.BoostRechargeSpeed = boostRechargeSpeed;
        }

        private static void SetupShipDefenceLayers(ShipInstance shipInstance, in ChassisBaseStats chassisBaseStats, SizeType size)
        {
            var shield = shipInstance.Shield;
            var lossySize = GameConfig.SizeMap[size];
            shipInstance.Transform.localScale = Vector3.one * lossySize;

            if (shield != null)
            {
                shield.CacheDependencies();
                shield.Setup(100, 1, lossySize);
                shield.gameObject.layer = GameLayers.ShipLayer;
                shipInstance.OwnColliders.Add(shield.Collider);
            }


            var hull = shipInstance.Hull;
            var hullPoints = chassisBaseStats.HullPoints;
            hull.CacheDependencies();
            hull.Setup(hullPoints, 100, ModuleType.Chassis);

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
            var weaponsSlots = shipInstance.WeaponSlotsContainer.GetComponentsInChildren<WeaponSlot>();
            shipInstance.MainWeaponsSlots.Clear();
            shipInstance.TurretsSlots.Clear();

            foreach (var slot in weaponsSlots)
            {
                var weaponsCollection = slot.WeaponMountType == WeaponMountType.MainWeapon ? shipInstance.MainWeaponsSlots : shipInstance.TurretsSlots;
                weaponsCollection.Add(slot);
            }

            var shipRb = shipInstance.Rigidbody;

            foreach (var slot in weaponsSlots)
            {
                slot.CacheDependencies();
                var weapon = slot.Weapon;

                if (weapon == null) continue;

                weapon.InitBase(shipInstance.AimData, slot.Size, shipInstance.OwnColliders);
                SetWeaponStats(weapon);

                weapon.gameObject.layer = GameLayers.WeaponLayer;

                if (weapon.HullDefenseLayer != null)
                {
                    shipInstance.OwnColliders.Add(weapon.HullDefenseLayer.Collider);
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

        public static void SetWeaponStats(WeaponBase weaponBase)
        {
            var moduleType = weaponBase.ModuleType;

            var statsConfig = moduleType switch
            {
                ModuleType.MainWeapon => GameConfig.MainWeaponsBaseStats,
                ModuleType.Turret => GameConfig.TurretsBaseStats,
                _ => throw new Exception($" Weapon {weaponBase.Name} has wrong module Type= {moduleType}")
            };

            var baseStats = statsConfig.GetStats(weaponBase.WeaponType, weaponBase.Size);
            var aimStats = baseStats.AimStats;
            var damageStats = baseStats.DamageStats;
            var hullStats = baseStats.HullStats;

            ref var runtimeAimStats = ref weaponBase.RuntimeAimStats;
            runtimeAimStats.MaxDistance = aimStats.Distance;
            runtimeAimStats.MaxRotateAngle = aimStats.RotateAngle;
            runtimeAimStats.RotateSpeed = aimStats.RotateSpeed;

            ref var runtimeDamage = ref weaponBase.RuntimeDamage;
            runtimeDamage.DamageArmor = damageStats.DamageKinetic;
            runtimeDamage.DamageShield = damageStats.DamageEnergy;
            runtimeDamage.DamageHull = damageStats.DamageKinetic + damageStats.DamageEnergy;
            runtimeDamage.DamageAsteroid = runtimeDamage.DamageHull + runtimeDamage.DamageHull * damageStats.AsteroidBonusPercent * 0.01f;

            weaponBase.HullDefenseLayer.Setup(hullStats.HullPoints, hullStats.ArmorPoints, moduleType);

            if (weaponBase is ProjectileWeapon projectileWeapon)
            {
                var projectileWeaponStats = baseStats.ProjectileWeaponStats;

                ref var runtimeFireStats = ref projectileWeapon.RuntimeFireStats;
                runtimeFireStats.FireRate = projectileWeaponStats.FireRate;
                runtimeFireStats.SpreadAngle = projectileWeaponStats.SpreadAngle;
                runtimeFireStats.ProjectileSpeed = projectileWeaponStats.ProjectileSpeed;

                ref var logicStats = ref projectileWeapon.LogicStats;
                logicStats.ShootDelay = 1f / projectileWeaponStats.FireRate;
                var invProjectileSpeed = 1f / projectileWeaponStats.ProjectileSpeed;
                logicStats.InvProjectileSpeed = invProjectileSpeed;
                logicStats.ProjectileLifeTime = aimStats.Distance * invProjectileSpeed;
                logicStats.SpreadTimeMultiplier = projectileWeaponStats.SpreadAngle * 0.01f;

                projectileWeapon.Init(projectileWeaponStats.PoolReference.Id);
            }
            else if (weaponBase is ConstantBeamWeapon miningDrill)
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

