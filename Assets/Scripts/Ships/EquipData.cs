using ShipModules;
using System;

namespace Ships
{
    [Serializable]
    public struct EquipData
    {
        public MainEngine MainEngine;
        public Thruster SideEngine;
    }
}