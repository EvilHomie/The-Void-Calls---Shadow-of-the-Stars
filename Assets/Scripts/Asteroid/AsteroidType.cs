using System;

[Flags]
public enum AsteroidType
{
    None = 0,
    Titanium = 1 << 0,
    Copper = 1 << 1,
    Silver = 1 << 2,
    Platina = 1 << 3,
    Gold = 1 << 4,
    Empty = 1 << 5,

    Cluster = 1 << 6,
    Drifting = 1 << 7
}