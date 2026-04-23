using DI;
using GameSystems;
using Ships;
using UnityEngine;

public class ShipDataDebug : MonoBehaviour
{
    //public int ShipIndex = -1;

    //public Vector2 AimPosition;
    //public Vector2 Position;
    //public ChassisData ChassisData;
    //public MovementRuntimeData MovementData;
    //public EquipData EquipData;
    //public View ViewData;

    //private ShipsDataStorage _objectsStorage;

    //[Inject]
    //public void Construct(ShipsDataStorage objectsStorage)
    //{
    //    _objectsStorage = objectsStorage;
    //}

    //private void Update()
    //{
    //    if (ShipIndex < 0 || ShipIndex > _objectsStorage.LastUsedIndex)
    //    {
    //        return;
    //    }

    //    AimPosition = _objectsStorage.TargetPositions[ShipIndex];
    //    Position = _objectsStorage.SelfPositions[ShipIndex];
    //    ChassisData = _objectsStorage.MovementVisuals[ShipIndex];
    //    MovementData = _objectsStorage.Movements[ShipIndex];
    //    EquipData = _objectsStorage.Equips[ShipIndex];
    //    ViewData = _objectsStorage.Views[ShipIndex];
    //}
}
