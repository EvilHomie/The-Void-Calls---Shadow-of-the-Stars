using ShipModules;
using System;

namespace Ships
{
    [Serializable]
    public struct EquipData
    {
        public MainEngineId MainEngineId;
        public Thruster SideEngine;
    }
}