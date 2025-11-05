using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipMovementData
    {
        [field: SerializeField] public MainEngine MainEngine { get; set; }
        [field: SerializeField] public SideEngines SideEngines { get; set; }

        // отображение в инспекторе сугубо для дебага
        [field: SerializeField] public float Throttle { get; set; }
        [field: SerializeField] public bool InertiaDamping { get; set; }
        [field: SerializeField] public float RotatePowerValue { get; set; }
        [field: SerializeField] public float DirectAccelerationPower { get; set; }
        [field: SerializeField] public float SideAcceleration { get; set; }
    }
}