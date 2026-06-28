using System;

namespace ShipModules
{
    public interface IShipModule
    {
        public SizeType Size { get; }
        public string Name { get; }
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