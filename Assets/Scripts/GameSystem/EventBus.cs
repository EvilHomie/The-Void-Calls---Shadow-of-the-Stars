using Player;
using System;
using UnityEngine;

namespace GameSystem
{
    public class EventBus : MonoBehaviour
    {
        //PlayerShip
        public static Action<PlayerShipData> UpdateShip { get; set; }
        public static Action<MainEngine> UpdateMainEngine { get; set; }
        public static Action<SideEngines> UpdateSideEngines { get; set; }
        public static Action<DampingModule> UpdateDampingModule { get; set; }
    }
}

