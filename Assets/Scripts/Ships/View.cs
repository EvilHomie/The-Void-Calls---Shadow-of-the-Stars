using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public struct View
    {
        [field: SerializeField] public MainEnginePlume[] MainEnginesPlumes { get; private set; }
        [field: SerializeField] public MainEnginePlume[] BoostersPlumes { get; private set; }
        [field: SerializeField] public ThrusterPlume ThrusterFL { get; private set; }
        [field: SerializeField] public ThrusterPlume ThrusterFR { get; private set; }
        [field: SerializeField] public ThrusterPlume ThrusterBL { get; private set; }
        [field: SerializeField] public ThrusterPlume ThrusterBR { get; private set; }
    }
}