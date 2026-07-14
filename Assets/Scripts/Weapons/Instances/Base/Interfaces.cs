using UnityEngine;

namespace Weapons
{
    public interface IRigidBodyDependentWeapon
    {
        public Rigidbody2D ShipRB { get; }
        void CacheRigidBody(Rigidbody2D rb);
    }

    public interface IPoolDependentWeapon
    {
        public uint ProjectilePoolId { get; }
        void CachePool(uint poolId);
    }
}