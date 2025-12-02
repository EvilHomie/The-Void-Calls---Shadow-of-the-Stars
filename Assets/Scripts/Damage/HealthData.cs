using System;
using UnityEngine;

public class HealthData : MonoBehaviour
{
    public ResistanceType ResistanceType;
    public HealthPoints DefaultHealthPoints;
    public HealthPoints CurrentHealthPoints;
}

[Serializable]
public struct HealthPoints
{
    public float HullPoints;
    public float ArmorPoints;
    public float ShieldPoints;
}