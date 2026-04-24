using Ships;

namespace Helpers
{
    public class ShipInitHelper
    {
        public static void InitShip(ShipInstance shipInstance)
        {
            ref var movement = ref shipInstance.MovementRuntimeData;
            movement.InertiaDampingActive = true;

            ref var movementCharacteristics = ref shipInstance.MovementStaticData;
            ref var equip = ref shipInstance.Equip;

            var chassisMods = equip.Chassis.ChassisModificators;
            var mainEngineMods = equip.MainEngine.MainEngineModificators;
            var sideEngineMods = equip.SideEngine.ThrustersModificators;

            var totalMass = equip.Chassis.Mass + chassisMods.MassModPercent * equip.Chassis.Mass / 100;
            shipInstance.Rigidbody.mass = totalMass;

            var totalDirectDrag = equip.Chassis.DirectDrag + chassisMods.DirectDragModPercent * equip.Chassis.DirectDrag / 100;
            var totalReverseDrag = equip.Chassis.ReverseDrag + chassisMods.ReverseDragModPercent * equip.Chassis.ReverseDrag / 100;
            var totalStrafeDrag = equip.Chassis.StrafeDrag + chassisMods.StrafeDragModPercent * equip.Chassis.StrafeDrag / 100;
            var totalRotateDrag = equip.Chassis.RotateDrag + chassisMods.RotateDragModPercent * equip.Chassis.RotateDrag / 100;

            var worldUnitMod = WorldConfig.WorldUnitMod;
            var inertiaDampingForce = WorldConfig.InertiaDampingForce;
            var mainEngine = equip.MainEngine;
            var sideEngine = equip.SideEngine;

            var totalDirectThrust = mainEngine.DirectThrust + mainEngineMods.DirectThrustModPercent * mainEngine.DirectThrust / 100;
            var totalReverseThrust = mainEngine.ReverseThrust + mainEngineMods.ReverseThrustModPercent * mainEngine.ReverseThrust / 100;
            var totalStrafeThrust = sideEngine.StrafeThrust + sideEngineMods.StrafeThrustModPercent * sideEngine.StrafeThrust / 100;
            var totalRotateThrust = sideEngine.RotateThrust + sideEngineMods.RotateThrustModPercent * sideEngine.RotateThrust / 100;

            movementCharacteristics.DirectMaxSpeed = totalDirectThrust / totalDirectDrag * worldUnitMod;
            movementCharacteristics.DirectAcceleration = totalDirectThrust / totalMass * worldUnitMod;
            movementCharacteristics.DirectDampingAcceleration = totalDirectDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.ReverseMaxSpeed = totalReverseThrust / totalReverseDrag * worldUnitMod;
            movementCharacteristics.ReverseAcceleration = totalReverseThrust / totalMass * worldUnitMod;
            movementCharacteristics.ReverseDampingAcceleration = totalReverseDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.StrafeMaxSpeed = totalStrafeThrust / totalStrafeDrag * worldUnitMod;
            movementCharacteristics.StrafeAcceleration = totalStrafeThrust / totalMass * worldUnitMod;
            movementCharacteristics.StrafeDampingAcceleration = totalStrafeDrag * inertiaDampingForce * worldUnitMod;

            movementCharacteristics.RotateSpeed = totalRotateThrust / totalRotateDrag;

        }
    }
}

