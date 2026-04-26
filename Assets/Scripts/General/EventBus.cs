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
        public static Action<ShipInstance> SpawnOtherShip { get; set; }
        public static Action<ShipInstance> RemoveOtherShip { get; set; }
        public static Action<ShipInstance> SpawnPlayerShip { get; set; }
        public static Action RemovePlayerShip { get; set; }



        public static Action<float> ChangeCameraOrtoSize { get; set; }
        public static Action<WeaponBase> CreateWeaponAction { get; set; }
        public static Action<WeaponBase, bool> WeaponChangeShootState { get; set; }
        public static Action<ShipInstance, bool> NonPlayerChangeAttackState { get; set; }


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

