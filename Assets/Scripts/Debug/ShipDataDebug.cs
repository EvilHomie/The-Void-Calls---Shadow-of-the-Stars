using DI;
using GameSystems;
using Ships;
using UnityEngine;

public class ShipDataDebug : MonoBehaviour
{
    public int ShipIndex = -1;

    public Vector2 AimPosition;
    public Vector2 Position;
    public ChassisData ChassisData;
    public MovementData MovementData;
    public EquipData EquipData;
    public ViewData ViewData;

    private ShipsDataStorage _objectsStorage;

    [Inject]
    public void Construct(ShipsDataStorage objectsStorage)
    {
        _objectsStorage = objectsStorage;
    }

    private void Update()
    {
        if (ShipIndex < 0 || ShipIndex > _objectsStorage.LastUsedIndex)
        {
            return;
        }

        AimPosition = _objectsStorage.AimPositions[ShipIndex];
        Position = _objectsStorage.Positions[ShipIndex];
        ChassisData = _objectsStorage.ChassisDatas[ShipIndex];
        MovementData = _objectsStorage.MovementDatas[ShipIndex];
        EquipData = _objectsStorage.EquipDatas[ShipIndex];
        ViewData = _objectsStorage.Views[ShipIndex];
    }
}
