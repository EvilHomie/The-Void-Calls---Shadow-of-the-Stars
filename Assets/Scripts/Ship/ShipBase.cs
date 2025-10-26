using UnityEngine;

namespace Ship
{
    public abstract class ShipBase : MonoBehaviour
    {
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }       
        [field: SerializeField] public ShipChassisData ChassisData { get; private set; }
        [field: SerializeField] public ShipMovementData MovementData { get; private set; }
        [field: SerializeField] public ShipMovementView MovementView { get; private set; }
    }
}