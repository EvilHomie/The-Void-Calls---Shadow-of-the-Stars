using Helper;
using Ship;
using UnityEngine;

namespace Player
{
    public class PlayerShip : MonoBehaviour
    {        
        public Vector3 MousePos { get; set; }

        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public ShipData ShipData { get; private set; }


        private void Start()
        {
            ShipInitHelper.Init(ShipData, Rigidbody);
        }
    }
}