using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        [field: SerializeField] public string Name { get; private set; }
        public abstract WeaponType WeaponType { get; }
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public float MaxDistance { get; private set; }
        [field: SerializeField] public float MaxRotateAngle { get; private set; }

        public float RotateSpeed;
        public bool IsShooting;
    }
}