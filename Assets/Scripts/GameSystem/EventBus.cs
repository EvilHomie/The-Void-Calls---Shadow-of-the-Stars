using Ship;
using System;
using UnityEngine;
using Weapon;

namespace GameSystem
{
    public class EventBus : MonoBehaviour
    {
        public static Action<ShipData> PlayerChangeShip { get; set; }        
        public static Action<float> ChangeCameraOrtoSize { get; set; }        
        public static Action<WeaponBase> CreateWeaponAction { get; set; }   
    }
}

