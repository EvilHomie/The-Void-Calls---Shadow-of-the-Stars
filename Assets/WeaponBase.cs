using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    [field: SerializeField] public float MaxRotateAngle { get; private set; }
    [field: SerializeField] public float RotateSpeed { get; private set; }
}
