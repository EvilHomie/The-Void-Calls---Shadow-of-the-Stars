using ShipModules;
using System;

namespace Ships
{
    [Serializable]
    public struct EquipData
    {
        public Chassis Chassis;
        public MainEngine MainEngine;
        public Thruster SideEngine;
    }
}