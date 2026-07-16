//using General;
//using Helpers;
//using Ships;
//using UnityEngine;

//public class TestShip : MonoBehaviour
//{
//    ShipInstance shipInstance;

//    Rigidbody2D lastTarget;
//    void Start()
//    {
//        shipInstance = GetComponent<ShipInstance>();
//        InitHelper.InitShip(shipInstance);
//    }

//    private void Update()
//    {
//        if (lastTarget != shipInstance.AimData.TargetRigidBody)
//        {
//            lastTarget = shipInstance.AimData.TargetRigidBody;
//            EventBus.ChangeTargetAction?.Invoke(shipInstance, lastTarget);
//        }
//    }
//}
