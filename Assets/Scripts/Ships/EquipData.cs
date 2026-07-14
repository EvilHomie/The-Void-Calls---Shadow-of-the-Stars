using System;

namespace Ships
{
    [Serializable]
    public struct EquipData
    {
        public MainEngineId MainEngineId;
        public ThrusterId ThrusterId;
    }
}