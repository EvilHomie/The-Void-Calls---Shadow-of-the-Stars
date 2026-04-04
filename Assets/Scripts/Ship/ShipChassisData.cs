using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipChassisData
    {
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public float Mass { get; private set; }
        [field: SerializeField] public float DirectDrag { get; private set; }
        [field: SerializeField] public float ReverseDrag { get; private set; }
        [field: SerializeField] public float StrafeDrag { get; private set; }
        [field: SerializeField] public float RotateDrag { get; private set; }
    }
}