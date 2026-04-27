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
        public static Action<ProjectileBase, Collider2D> ProjectileHit { get; set; }
        public static Action<WeaponBase, Collider2D> BeamHit { get; set; }

        public static Action<ShipInstance, bool> NonPlayerChangeAttackState { get; set; }


        public static Func<PoolReference, ProjectileBase> GetProjectile { get; set; }
        public static Action<ProjectileBase> ProjectileFetched { get; set; }



        public static Func<PoolReference, HitParticle> GetHitParticle { get; set; }
        public static Action<HitParticle> HitParticleFetched { get; set; }
    }
}

