using System;
using UnityEngine;

namespace Ship
{
    [Serializable]
    public class ShipMovementView
    {
        [field: SerializeField] public ExhaustPlume MainEngine { get; private set; }
        [field: SerializeField] public ExhaustPlume[] ReverseMainEngines { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineFL { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineFR { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineBL { get; private set; }
        [field: SerializeField] public ExhaustPlume SideEngineBR { get; private set; }

        public void Init()
        {
            MainEngine.Init();
            SideEngineFL.Init();
            SideEngineFR.Init();
            SideEngineBL.Init();
            SideEngineBR.Init();

            foreach (var item in ReverseMainEngines)
            {
                item.Init();
            }
        }
    }
}