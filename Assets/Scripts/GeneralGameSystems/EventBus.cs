using Projectiles;
using Ship;
using System;
using UnityEngine;
using Weapons;

namespace GameSystems
{
    public class EventBus : MonoBehaviour
    {
        public static Action<ShipInstance> SpawnShip { get; set; }
        public static Action<ShipInstance> RemoveShip { get; set; }

        public static Action PlayerChangeShip { get; set; }
        public static Action<float> ChangeCameraOrtoSize { get; set; }
        public static Action<WeaponBase> CreateWeaponAction { get; set; }
        public static Action<WeaponBase, bool> WeaponChangeShootState { get; set; }
        public static Action<ShipInstance, bool> NonPlayerChangeAttackState { get; set; }


        public static Action<ProjectileBase, Collider2D> ProjectileHit { get; set; }
        public static Action<WeaponBase, Collider2D> BeamHit { get; set; }



        public static Func<string, ProjectileBase> GetProjectile { get; set; }
        public static Action<ProjectileBase> ProjectileFetched { get; set; }
        public static Action<ProjectileBase> ReturnProjectile { get; set; }

        public static Func<string, HitParticle> GetHitParticle { get; set; }
        public static Action<HitParticle> ReturnHitParticle { get; set; }

        public static Action<GameState> GameStateChangeAction { get; set; }
    }
}

