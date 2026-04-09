using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public struct ShipData
    {
        public Vector2 TargetPos;
        public Vector2 Position;

        public ChassisData ChassisData;
        public MovementData MovementData;

        public EquipData EquipData;
    }
}