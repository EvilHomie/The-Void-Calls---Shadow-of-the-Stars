using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public struct MovementView
    {
        public float LastMainEnginePowerValue;
        public bool LastBoostersState;
        //public float LastRotatePowerValue;
        //public float LastStrafePowerValue;

        public ThrustersPower ThrustersPower;

        [field: SerializeField] public MainEnginePlume[] MainEnginesPlumes { get; private set; }
        [field: SerializeField] public BoosterPlume[] BoostersPlumes { get; private set; }
        [field: SerializeField] public ThrusterPlume[] ThrustersFL { get; private set; }
        [field: SerializeField] public ThrusterPlume[] ThrustersFR { get; private set; }
        [field: SerializeField] public ThrusterPlume[] ThrustersBL { get; private set; }
        [field: SerializeField] public ThrusterPlume[] ThrustersBR { get; private set; }
    }
}