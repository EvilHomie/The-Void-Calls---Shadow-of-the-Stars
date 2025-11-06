using Helper;
using Ship;
using UnityEngine;

namespace Player
{
    public class PlayerShip : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public ShipData ShipData { get; private set; }

        public Vector3 MousePos { get; set; }

        private void Start()
        {
            InitHelper.Init(ShipData, Rigidbody);
        }
    }
}