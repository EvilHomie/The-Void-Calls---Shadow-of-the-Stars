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
        public static Action<ShipInstance, bool> NonPlayerChangeAttackState { get; set; }



        public static Action<WeaponBase, Collider2D> BeamHit { get; set; }
        public static Action<ProjectileBase, Collider2D> ProjectileHitAction { get; set; }


        public delegate void SpawnBoltDelegate(in BoltSpawnData data);
        public static SpawnBoltDelegate SpawnBoltAction;


        public delegate void SpawnHitEffectDelegate(in HitEffectSpawnData data);
        public static SpawnHitEffectDelegate SpawnHitEffectAction;
    }
}

