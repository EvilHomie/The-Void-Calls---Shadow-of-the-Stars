using DI;
using Ship;
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

        }

        protected override void Subscribe()
        {

        }

        protected override void Unsubscribe()
        {

        }

        private void Start()
        {
            _playerShip.IsPlayer = true;
            InitShip(_playerShip);
            EventBus.SpawnShip?.Invoke(_playerShip);
            EventBus.GameStateChangeAction?.Invoke(GameState.CoreGameplay);
        }

        private void InitShip(ShipInstance shipInstance)
        {
            ref var shipMovementData = ref shipInstance.ShipInitialData.MovementData;
            ref var shipChassisData = ref shipInstance.ShipInitialData.ChassisData;
            ref var shipEquipData = ref shipInstance.ShipInitialData.EquipData;

            shipMovementData.InertiaDampingLastState = shipMovementData.InertiaDamping;
            shipMovementData.DirectMaxSpeed = shipEquipData.MainEngine.DirectThrust / shipChassisData.DirectDrag * Constants.WorldUnitMod;
            shipMovementData.DirectAcceleration = shipEquipData.MainEngine.DirectThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipMovementData.ReverseMaxSpeed = shipEquipData.MainEngine.ReverseThrust / shipChassisData.ReverseDrag * Constants.WorldUnitMod;
            shipMovementData.ReverseAcceleration = shipEquipData.MainEngine.ReverseThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipMovementData.StrafeMaxSpeed = shipEquipData.SideEngines.StrafeThrust / shipChassisData.StrafeDrag * Constants.WorldUnitMod;
            shipMovementData.StrafeAcceleration = shipEquipData.SideEngines.StrafeThrust / shipChassisData.Mass * Constants.WorldUnitMod;
            shipMovementData.RotateSpeed = shipEquipData.SideEngines.RotateThrust / shipChassisData.RotateDrag;
        }
    }
}

