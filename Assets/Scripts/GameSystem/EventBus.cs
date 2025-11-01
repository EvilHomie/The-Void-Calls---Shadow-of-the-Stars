using Ship;
using System;
using UnityEngine;

namespace GameSystem
{
    public class EventBus : MonoBehaviour
    {
        public static Action<ShipData> PlayerChangeShip { get; set; }        
        public static Action<float> ChangeCameraOrtoSize { get; set; }        


    }
}

