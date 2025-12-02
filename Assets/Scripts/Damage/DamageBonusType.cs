using System;

[Flags]
public enum DamageBonusType
{
    None = 0,
    AsteroidBonus = 1 << 0,
    ShieldBonus = 1 << 1,
    ArmorBonus = 1 << 2,
    HullBonus = 1 << 3
}
