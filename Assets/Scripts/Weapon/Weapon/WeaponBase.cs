using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : MonoBehaviour, IShipModule
    {
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public string WeaponName { get; private set; }
        [field: SerializeField] public float MaxRotateAngle { get; private set; }
        [field: SerializeField] public float RotateSpeed { get; private set; }
        [field: SerializeField] public Transform CTransform { get; private set; }
        [field: SerializeField] public float MaxDistance { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        [field: SerializeField] public bool IsShooting { get; set; }
        public abstract WeaponType WeaponType { get; }
    }
}