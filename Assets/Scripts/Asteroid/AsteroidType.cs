using System;

[Flags]
public enum AsteroidType
{
    Empty = 0,
    Titanium = 1 << 0,
    Copper = 1 << 1,
    Silver = 1 << 2,
    Nickel = 1 << 3,
    Gold = 1 << 4,

    Cluster = 1 << 5,
    Drifting = 1 << 6
}