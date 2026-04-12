using Ships;

namespace Helper
{
    public class ShipInitHelper
    {
        
        //public static void Init(ShipData shipData)
        //{
        //    //InitView(shipData.View);
        //    //Init(shipData.WeaponData, rigidbody);
        //}

        //private static void InitView(ViewData shipMovementView)
        //{
        //    shipMovementView.SideEngineFL.Init();
        //    shipMovementView.SideEngineFR.Init();
        //    shipMovementView.SideEngineBL.Init();
        //    shipMovementView.SideEngineBR.Init();

        //    foreach (var engine in shipMovementView.ReverseEngines)
        //    {
        //        engine.Init();
        //    }

        //    foreach (var engine in shipMovementView.DirectEngines)
        //    {
        //        engine.Init();
        //    }
        //}

        //private static void Init(ShipWeaponData shipWeaponData, Rigidbody2D rigidbody)
        //{
        //    foreach (var slot in shipWeaponData.WeaponSlots)
        //    {
        //        if (slot.Weapon is BoltRepeater weapon)
        //        {
        //            weapon.ShipRigidBody = rigidbody;
        //        }

        //        if (slot.Weapon != null) EventBus.CreateWeaponAction?.Invoke(slot.Weapon);
        //    }
        //}
    }
}

