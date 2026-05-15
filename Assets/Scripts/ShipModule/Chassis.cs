using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Chassis", menuName = "Scriptable Objects/Chassis")]
public class Chassis : ScriptableObject, IShipModule
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public SizeType Size { get; private set; }
    [field: SerializeField] public int Hull { get; private set; }
    [field: SerializeField] public float Mass { get; private set; }
    [field: SerializeField] public float DirectDrag { get; private set; }
    [field: SerializeField] public float ReverseDrag { get; private set; }
    [field: SerializeField] public float StrafeDrag { get; private set; }
    [field: SerializeField] public float RotateDrag { get; private set; }

    public ChassisMultipliers ChassisMultipliers;
}

public struct ChassisMultipliers
{
    public float MassMultiplier;
    public float HullModPercent;
    public float DirectDragMultiplier;
    public float ReverseDragMultiplier;
    public float StrafeDragMultiplier;
    public float RotateDragMultiplier;
}

[Serializable]
public enum SizeType
{
    S,
    M,
    L,
    XL
}