using UnityEngine;

namespace Ship
{
    public class ShipInstance : MonoBehaviour
    {         
        [field: SerializeField] public ShipData ShipData { get; private set; }
        [field: SerializeField] public ShipView View { get; private set; }
    }
}