using GameSystem;
using Projectile;
using UnityEngine;

public class DamageSystem : GameSystemBase
{

    protected override void Init()
    {

    }

    protected override void Subscribe()
    {
        EventBus.ProjectileHit += OnProjectileHit;
    }

   

    protected override void Unsubscribe()
    {
        EventBus.ProjectileHit -= OnProjectileHit;
    }

    private void OnProjectileHit(ProjectileBase projectile, Collider2D collider)
    {
        EventBus.ReturnProjectile(projectile);
    }



    //public static void ApplyDamage(IDamageable target, float amount, DamageType type)
    //{
    //    ref var profile = target.DamageProfile; // ссылка на данные

    //    switch (profile.ProfileType)
    //    {
    //        case DamageProfileType.Ship:
    //            ApplyShipDamage(ref profile, amount, type);
    //            break;

    //        case DamageProfileType.Asteroid:
    //            ApplyAsteroidDamage(ref profile, amount);
    //            break;
    //    }
    //}

    //private static void ApplyShipDamage(ref DamageProfile profile, float amount, DamageType type)
    //{
    //    switch (type)
    //    {
    //        case DamageType.Energy:
    //            if (profile.Shield > 0)
    //            {
    //                profile.Shield -= amount;
    //                return;
    //            }
    //            break;

    //        case DamageType.Kinetic:
    //            if (profile.Armor > 0)
    //            {
    //                profile.Armor -= amount;
    //                return;
    //            }
    //            break;
    //    }

    //    profile.Hull -= amount;
    //}

    //private static void ApplyAsteroidDamage(ref DamageProfile profile, float amount)
    //{
    //    profile.Structure -= amount;
    //}

}
