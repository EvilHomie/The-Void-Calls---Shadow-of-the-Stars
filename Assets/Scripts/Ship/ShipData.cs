using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public struct ShipData
    {        
        public Vector3 TargetPos;
        public Vector3 Position;
        public ShipChassisData ChassisData;
        public ShipEquipData EquipData;
        public ShipMovementData MovementData;
    }
}