using UnityEngine;

namespace Weapon
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [field: SerializeField] public float MaxRotateAngle { get; private set; }
        [field: SerializeField] public float RotateSpeed { get; private set; }
        [field: SerializeField] public Transform CTransform { get; private set; }
        [field: SerializeField] public float MaxDistance { get; private set; }
        [field: SerializeField] public LayerMask HitLayers { get; private set; }
        public abstract WeaponType WeaponType { get; }
    }
}