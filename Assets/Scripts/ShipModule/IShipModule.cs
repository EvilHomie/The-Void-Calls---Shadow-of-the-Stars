using System;

namespace ShipModules
{
    public interface IShipModule
    {
        public SizeType Size { get; }
        public ModuleType ModuleType { get; }
        //public string Name { get; }
    }
}

[Serializable]
public enum SizeType
{
    S,
    M,
    L,
    XL
}

[Serializable]
public enum ModuleType
{
    MainWeapon,
    Turret,
    Thruster,
    Chassis,
    MainEngine
}