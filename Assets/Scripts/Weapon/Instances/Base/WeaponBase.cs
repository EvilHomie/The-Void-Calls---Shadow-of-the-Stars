using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        public abstract WeaponType WeaponType { get; }
        public abstract ref WeaponBaseStats BaseStats { get; }
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        
        public DamageData DamageData;        
        public abstract void Init();
    }
}