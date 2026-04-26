using HitParticles;
using Projectiles;
using Ships;
using System;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class EventBus : MonoBehaviour
    {
        // Ships Actions
        public static Action<ShipInstance> SpawnOtherShip { get; set; }
        public static Action<ShipInstance> DestroyOtherShip { get; set; }
        public static Action<ShipInstance> SpawnPlayerShip { get; set; }
        public static Action<ShipInstance> DestroyPlayerShip { get; set; }


        // Weapon Actions
        public static Action<WeaponBase> WeaponStartAttack { get; set; }
        public static Action<WeaponBase> WeaponStopAttack { get; set; }
        public static Action<WeaponBase> WeaponCreated { get; set; }
        public static Action<WeaponBase> WeaponDestroyed { get; set; }


        public static Action<float> ChangeCameraOrtoSize { get; set; }

        public static Action<ProjectileBase, Collider2D> ProjectileHit { get; set; }
        public static Action<WeaponBase, Collider2D> BeamHit { get; set; }



        public static Func<PoolReference, ProjectileBase> GetProjectile { get; set; }
        public static Action<ProjectileBase> ProjectileFetched { get; set; }
        public static Action<ProjectileBase> ReturnProjectile { get; set; }

        public static Func<PoolReference, HitParticle> GetHitParticle { get; set; }
        public static Action<HitParticle> ReturnHitParticle { get; set; }

        public static Action<GameState> GameStateChangeAction { get; set; }
    }
}

