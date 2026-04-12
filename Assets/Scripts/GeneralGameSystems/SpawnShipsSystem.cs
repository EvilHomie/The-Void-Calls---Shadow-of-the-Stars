using DI;
using Ships;
using UnityEngine;

namespace GameSystems
{
    public class SpawnShipsSystem : GameSystemBase
    {
        private ShipInstance _playerShip;
        [Inject]
        public void Construct(ShipInstance playerShip)
        {
            _playerShip = playerShip;
        }

        protected override void AwakeInit()
        {
            //QualitySettings.vSyncCount = 0;
        }

        protected override void Subscribe()
        {

        }

        protected override void Unsubscribe()
        {

        }

        private void Start()
        {
            InitShip(_playerShip);
            EventBus.SpawnPlayerShip?.Invoke(_playerShip);
            EventBus.GameStateChangeAction?.Invoke(GameState.CoreGameplay);
        }

        private void InitShip(ShipInstance shipInstance)
        {
            ref var shipView = ref shipInstance.View;

            ref var shipData = ref shipInstance.ShipInitialData;
            shipData.TargetPos = Vector3.zero;
            shipData.Position = shipView.Transform.position;

            ref var shipMovementData = ref shipInstance.ShipInitialData.MovementData;
            ref var shipChassisData = ref shipInstance.ShipInitialData.ChassisData;
            ref var shipEquipData = ref shipInstance.ShipInitialData.EquipData;

            shipChassisData.DirectMaxSpeed = shipEquipData.MainEngine.DirectThrust / shipChassisData.DirectDrag * Constants.WorldUnitMod;
            shipChassisData.DirectMaxAcceleration = shipEquipData.MainEngine.DirectThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipChassisData.ReverseMaxSpeed = shipEquipData.MainEngine.ReverseThrust / shipChassisData.ReverseDrag * Constants.WorldUnitMod;
            shipChassisData.ReverseMaxAcceleration = shipEquipData.MainEngine.ReverseThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipChassisData.StrafeMaxSpeed = shipEquipData.SideEngines.StrafeThrust / shipChassisData.StrafeDrag * Constants.WorldUnitMod;
            shipChassisData.StrafeMaxAcceleration = shipEquipData.SideEngines.StrafeThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipChassisData.RotateMaxSpeed = shipEquipData.SideEngines.RotateThrust / shipChassisData.RotateDrag;
            shipChassisData.RotateMaxAcceleration = shipEquipData.SideEngines.RotateThrust / shipChassisData.Mass;
        }
    }
}

