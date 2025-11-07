using Projectile;
using Ship;
using System;
using UnityEngine;
using Weapon;

namespace GameSystem
{
    public class EventBus : MonoBehaviour
    {
        public static Action<ShipData> PlayerChangeShip { get; set; }
        public static Action<float> ChangeCameraOrtoSize { get; set; }
        public static Action<WeaponBase> CreateWeaponAction { get; set; }
        public static Action<WeaponBase, bool> WeaponChangeState { get; set; }


        public static Action<ProjectileBase, Collider2D> ProjectileHit { get; set; }

        public static Func<string, ProjectileBase> GetProjectile { get; set; }
        public static Action<ProjectileBase> ProjectileFetched { get; set; }
        public static Action<ProjectileBase> ReturnProjectile { get; set; }
    }
}

