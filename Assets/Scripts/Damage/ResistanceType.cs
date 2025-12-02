using System;

[Flags]
public enum ResistanceType
{
    None = 0,
    KineticResistance = 1 << 1,
    EnergyResistance = 1 << 2
}