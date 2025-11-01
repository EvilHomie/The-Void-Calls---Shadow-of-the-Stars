using System;

[Serializable]
public struct DamageProfile
{
    public float Shield;
    public float Armor;
    public float Hull;
    public float Structure;

    public DamageProfileType ProfileType;
}
