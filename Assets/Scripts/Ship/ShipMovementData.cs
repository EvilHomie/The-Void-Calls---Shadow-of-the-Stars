using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipMovementData
    {
        [field: SerializeField] public MainEngine MainEngine { get; private set; }
        [field: SerializeField] public SideEngines SideEngines { get; private set; }

        // отображение в инспекторе сугубо для дебага
        [field: SerializeField] public float Throttle { get; set; }
        [field: SerializeField] public bool InertiaDamping { get; set; }
        [field: SerializeField] public float RotatePowerValue { get; set; }
        [field: SerializeField] public float DirectAcceleration { get; set; }
        [field: SerializeField] public float SideAcceleration { get; set; }
    }
}