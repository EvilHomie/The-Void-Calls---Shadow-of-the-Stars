using Ships;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class ShipsStorage : GameSystemBase
    {
        public readonly StructList<Vector2> AimPositions = new(1000);
        public readonly StructList<Vector2> Positions = new(1000);
        public readonly StructList<ChassisData> ChassisDatas = new(1000);
        public readonly StructList<MovementData> MovementDatas = new(1000);
        public readonly StructList<EquipData> EquipDatas = new(1000);
        public readonly StructList<ViewData> ViewsDatas = new(1000);
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

            GameFlow.PreUpdateTick += UpdateCollections;
        }

        protected override void Unsubscribe()
        {
            EventBus.SpawnNonPlayerShip -= OnSpawnNonPlayerShip;
            EventBus.RemoveNonPlayerShip -= OnRemoveNonPlayerShip;

            EventBus.SpawnPlayerShip -= OnSpawnPlayerShip;
            EventBus.RemovePlayerShip -= OnRemovePlayerShip;

            GameFlow.PreUpdateTick -= UpdateCollections;
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

            AimPositions.Add(Vector2.zero);
            Positions.Add(shipInstance.transform.position);
            ChassisDatas.Add(shipInstance.ShipInitialData.ChassisData);
            MovementDatas.Add(shipInstance.ShipInitialData.MovementData);
            ViewsDatas.Add(shipInstance.View);
            EquipDatas.Add(shipInstance.ShipInitialData.EquipData);


            //AimPositions[LastUsedIndex] = Vector3.zero;
            //Positions[LastUsedIndex] = shipInstance.transform.position;
            //ChassisDatas[LastUsedIndex] = shipInstance.ShipInitialData.ChassisData;
            //MovementDatas[LastUsedIndex] = shipInstance.ShipInitialData.MovementData;
            //ViewsDatas[LastUsedIndex] = shipInstance.View;
            //EquipDatas[LastUsedIndex] = shipInstance.ShipInitialData.EquipData;
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
                ref var movedMovementData = ref MovementDatas[LastUsedIndex];
                ref var movedViewData = ref ViewsDatas[LastUsedIndex];
                ref var movedEquipData = ref EquipDatas[LastUsedIndex];

                AimPositions[index] = movedAimPosition;
                Positions[index] = movedPosition;
                ChassisDatas[index] = movedChassisData;
                MovementDatas[index] = movedMovementData;
                ViewsDatas[index] = movedViewData;
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
            MovementDatas.RemoveAt(LastUsedIndex);
            ViewsDatas.RemoveAt(LastUsedIndex);
            EquipDatas.RemoveAt(LastUsedIndex);
            LastUsedIndex--;
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

