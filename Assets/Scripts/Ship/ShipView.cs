using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipView
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public ExhaustPlume[] DirectEngines { get; private set; }
        [field: SerializeField] public ExhaustPlume[] ReverseEngines { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineFL { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineFR { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineBL { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineBR { get; private set; }
    }
}