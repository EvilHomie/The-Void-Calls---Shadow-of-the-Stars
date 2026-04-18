using Ships;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class ShipsDataStorage : GameSystemBase
    {
        public readonly StructList<Vector2> AimPositions = new(1000);
        public readonly StructList<Vector2> Positions = new(1000);
        public readonly StructList<ChassisData> ChassisDatas = new(1000);
        public readonly StructList<MovementData> MovementRuntimeDatas = new(1000);
        public readonly StructList<EquipData> EquipDatas = new(1000);
        public readonly StructList<ViewData> Views = new(1000);
        public int LastUsedIndex { get; private set; } = -1;
        public int PlayerIndex { get; private set; }

        private readonly List<int> _removeQueue = new(50);
        private readonly List<ShipInstance> _addQueue = new(50);

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            EventBus.SpawnNonPlayerShip += OnSpawnNonPlayerShip;
            EventBus.RemoveNonPlayerShip += OnRemoveNonPlayerShip;

            EventBus.SpawnPlayerShip += OnSpawnPlayerShip;
            EventBus.RemovePlayerShip += OnRemovePlayerShip;

            GameFlowSystem.PreUpdateTick += UpdateCollections;
        }

        protected override void Unsubscribe()
        {
            EventBus.SpawnNonPlayerShip -= OnSpawnNonPlayerShip;
            EventBus.RemoveNonPlayerShip -= OnRemoveNonPlayerShip;

            EventBus.SpawnPlayerShip -= OnSpawnPlayerShip;
            EventBus.RemovePlayerShip -= OnRemovePlayerShip;

            GameFlowSystem.PreUpdateTick -= UpdateCollections;
        }

        private void UpdateCollections()
        {
            foreach (var index in _removeQueue)
            {
                RemoveShip(index);
            }

            _removeQueue.Clear();

            foreach (var ship in _addQueue)
            {
                AddShipData(ship);
            }

            _addQueue.Clear();
        }

        private void OnSpawnPlayerShip(ShipInstance shipInstance)
        {
            AddShipData(shipInstance);
            PlayerIndex = LastUsedIndex;
            EventBus.PlayerChangeShip?.Invoke();
        }

        private void OnSpawnNonPlayerShip(ShipInstance shipInstance)
        {
            _addQueue.Add(shipInstance);
        }

        private void AddShipData(ShipInstance shipInstance)
        {
            LastUsedIndex++;

            var chassisData = CreateChassisData(shipInstance);
            var movementData = new MovementData { InertiaDampingState = true };
            shipInstance.View.Rigidbody.mass = chassisData.Mass;

            AimPositions.Add(Vector2.zero);
            Positions.Add(shipInstance.transform.position);
            ChassisDatas.Add(chassisData);
            MovementRuntimeDatas.Add(movementData);
            Views.Add(shipInstance.View);
            EquipDatas.Add(shipInstance.EquipData);
            shipInstance.Index = LastUsedIndex;
        }

        private void OnRemovePlayerShip()
        {
            RemoveShip(PlayerIndex);
            PlayerIndex = -1;
        }

        private void OnRemoveNonPlayerShip(int shipIndex)
        {
            _removeQueue.Add(shipIndex);
        }

        private void RemoveShip(int index)
        {
            if (index != LastUsedIndex)
            {
                ref var movedAimPosition = ref AimPositions[LastUsedIndex];
                ref var movedPosition = ref Positions[LastUsedIndex];
                ref var movedChassisData = ref ChassisDatas[LastUsedIndex];
                ref var movedMovementData = ref MovementRuntimeDatas[LastUsedIndex];
                ref var movedViewData = ref Views[LastUsedIndex];
                ref var movedEquipData = ref EquipDatas[LastUsedIndex];

                AimPositions[index] = movedAimPosition;
                Positions[index] = movedPosition;
                ChassisDatas[index] = movedChassisData;
                MovementRuntimeDatas[index] = movedMovementData;
                Views[index] = movedViewData;
                EquipDatas[index] = movedEquipData;

                movedViewData.ShipInstance.Index = index;

                if (PlayerIndex == LastUsedIndex)
                {
                    PlayerIndex = index;
                }
            }

            AimPositions.RemoveAt(LastUsedIndex);
            Positions.RemoveAt(LastUsedIndex);
            ChassisDatas.RemoveAt(LastUsedIndex);
            MovementRuntimeDatas.RemoveAt(LastUsedIndex);
            Views.RemoveAt(LastUsedIndex);
            EquipDatas.RemoveAt(LastUsedIndex);
            LastUsedIndex--;
        }

        private ChassisData CreateChassisData(ShipInstance shipInstance)
        {
            var chassis = shipInstance.EquipData.Chassis;
            var mainEngine = shipInstance.EquipData.MainEngine;
            var sideEngine = shipInstance.EquipData.SideEngines;
            return new ChassisData
            {
                Size = chassis.Size,
                Mass = chassis.Mass,
                DirectDrag = chassis.DirectDrag,
                ReverseDrag = chassis.ReverseDrag,
                StrafeDrag = chassis.StrafeDrag,
                RotateDrag = chassis.RotateDrag,

                DirectMaxSpeed = mainEngine.DirectThrust / chassis.DirectDrag * Constants.WorldUnitMod,
                DirectMaxAcceleration = mainEngine.DirectThrust / chassis.Mass * Constants.WorldUnitMod,
                ReverseMaxSpeed = mainEngine.ReverseThrust / chassis.ReverseDrag * Constants.WorldUnitMod,
                ReverseMaxAcceleration = mainEngine.ReverseThrust / chassis.Mass * Constants.WorldUnitMod,
                StrafeMaxSpeed = sideEngine.StrafeThrust / chassis.StrafeDrag * Constants.WorldUnitMod,
                StrafeMaxAcceleration = sideEngine.StrafeThrust / chassis.Mass * Constants.WorldUnitMod,
                RotateMaxSpeed = sideEngine.RotateThrust / chassis.RotateDrag,
                RotateMaxAcceleration = sideEngine.RotateThrust / chassis.Mass
            };
        }
    }
}


/* Мысли на будущее
public readonly List<int> LOD1Indexes; // те что визуально видны и должны обновлятся с каждым кадром
        public readonly List<int> LOD2Indexes; // те что визуально не видно и обновляются раз в секунду
        public readonly List<int> LOD3Indexes; // те что визуально не видно и обновляются раз в 2 секунды
*/



/* логика когда еще корабль игрока был в общей коллекции

private void AddShip(ShipInstance shipInstance)
        {
            NonPlayerShipsInstance.Add(shipInstance);
            NonPlayerShipsData.Add(shipInstance.ShipInitialData);
            NonPlayerShipsView.Add(shipInstance.View);

            int index = NonPlayerShipsInstance.Count - 1;

            shipInstance.Index = index;

            if (NonPlayerShipsInstance[index].IsPlayer)
            {
                PlayerShipIndex = index;
            }
        }

        private void RemoveShip(int index)
        {
            int lastIndex = NonPlayerShipsInstance.Count - 1;

            if (index != lastIndex)
            {
                ref var movedData = ref NonPlayerShipsData[lastIndex];
                ref var movedView = ref NonPlayerShipsView[lastIndex];
                var movedInstance = NonPlayerShipsInstance[lastIndex];

                NonPlayerShipsData[index] = movedData;
                NonPlayerShipsView[index] = movedView;
                NonPlayerShipsInstance[index] = movedInstance;

                movedInstance.Index = index;
            }

            if (PlayerShipIndex == lastIndex)
            {
                PlayerShipIndex = index;
            }
            else if (PlayerShipIndex == index)
            {
                PlayerShipIndex = -1;
            }

            NonPlayerShipsData.RemoveAt(lastIndex);
            NonPlayerShipsView.RemoveAt(lastIndex);
        }


*/

