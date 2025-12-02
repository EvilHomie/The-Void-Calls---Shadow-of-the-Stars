using Asteroids;
using GameSystems;
using Ship;
using UnityEngine;
using Weapons;

namespace Helper
{
    public class InitHelper
    {
        public static void Init(ShipData shipData, Rigidbody2D rigidbody)
        {
            Init(shipData.MovementView);
            Init(shipData.WeaponData, rigidbody);
        }

        private static void Init(ShipMovementView shipMovementView)
        {
            shipMovementView.SideEngineFL.Init();
            shipMovementView.SideEngineFR.Init();
            shipMovementView.SideEngineBL.Init();
            shipMovementView.SideEngineBR.Init();

            foreach (var engine in shipMovementView.ReverseEngines)
            {
                engine.Init();
            }

            foreach (var engine in shipMovementView.DirectEngines)
            {
                engine.Init();
            }
        }

        private static void Init(ShipWeaponData shipWeaponData, Rigidbody2D rigidbody)
        {
            foreach (var slot in shipWeaponData.WeaponSlots)
            {
                if (slot.Weapon is BoltRepeater weapon)
                {
                    weapon.ShipRigidBody = rigidbody;
                }

                if (slot.Weapon != null) EventBus.CreateWeaponAction?.Invoke(slot.Weapon);
            }
        }
    }
}

