using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipMovementView
    {
        [field: SerializeField] public ExhaustPlume[] DirectEngines { get; private set; }
        [field: SerializeField] public ExhaustPlume[] ReverseEngines { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineFL { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineFR { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineBL { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineBR { get; private set; }

        public void Init()
        {
            SideEngineFL.Init();
            SideEngineFR.Init();
            SideEngineBL.Init();
            SideEngineBR.Init();

            foreach (var engine in ReverseEngines)
            {
                engine.Init();
            }

            foreach (var engine in DirectEngines)
            {
                engine.Init();
            }
        }
    }
}