using GameSystem;
using Ship;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerShipData : ShipBase
    {

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                //EventBus.UpdateDampingModule?.Invoke(MovementData.DampingModule);
            }
        }
    }
   
}