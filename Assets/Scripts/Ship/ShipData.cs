using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public struct ShipData
    {
        public bool IsPlayer;
        public int Index;
        public Vector3 TargetPos;
        public Vector3 Position;
        [field: SerializeField] public ShipChassisData ChassisData { get; private set; }
        [field: SerializeField] public ShipEquipData EquipData { get; private set; }
        [field: SerializeField] public ShipMovementData MovementData { get; private set; }
    }
}