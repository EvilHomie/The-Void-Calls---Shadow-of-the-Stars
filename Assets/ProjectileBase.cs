using Enviroment;
using UnityEngine;
using Weapon;

namespace Projectile
{
    public class ProjectileBase : PoolObjectBase
    {
        [field: SerializeField] public Rigidbody2D RigidBody { get; private set; }
        public WeaponBase Weapon { get; set; }
        public float LifeTime { get; set; }
    }
}