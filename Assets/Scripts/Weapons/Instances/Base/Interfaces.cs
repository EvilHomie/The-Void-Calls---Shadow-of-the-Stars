using UnityEngine;

namespace Weapons
{
    public interface IRigidBodyDependentWeapon
    {
        public Rigidbody2D ShipRB { get; }
        void ResolveRigidBodyDependency(Rigidbody2D rb);
    }

    public interface IProjectileDependentWeapon
    {
        public uint ProjectilePoolId { get; }
        void ResolveProjectileDependency(uint poolId);
    }
}