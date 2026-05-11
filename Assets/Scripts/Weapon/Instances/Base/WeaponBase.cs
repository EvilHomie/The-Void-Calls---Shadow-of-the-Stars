using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        public abstract WeaponType WeaponType { get; }
        public abstract ref AimStatsData AimStats { get; }
        public abstract ref DamageData Damage { get; }
        public abstract ref BaseDamageData BaseDamage { get; }
        public abstract ref DamageMultipliersData DamageMultipliers { get; }
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public SpriteRenderer BodySprite { get; private set; }

        public uint OwnerId;
    }
}