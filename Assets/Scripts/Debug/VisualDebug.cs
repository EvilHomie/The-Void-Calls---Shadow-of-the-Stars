using DI;
using GameSystems;
using Helpers;
using Ships;
using TMPro;
using UnityEngine;

public class VisualDebug : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MaxDirectSpeedText;
    [SerializeField] TextMeshProUGUI MaxReverseSpeedText;
    [SerializeField] TextMeshProUGUI MaxStrafeSpeedText;
    [SerializeField] TextMeshProUGUI DirectAcceleration;
    [SerializeField] TextMeshProUGUI ReverseAcceleration;
    [SerializeField] TextMeshProUGUI StrafeAcceleration;
    [SerializeField] TextMeshProUGUI RotateSpeed;
    [SerializeField] TextMeshProUGUI CurrentDirectSpeedText;
    [SerializeField] TextMeshProUGUI CurrentStrafeSpeedText;
    [SerializeField] TextMeshProUGUI ThrottleText;
    [SerializeField] TextMeshProUGUI DirectSpeedDelta;
    [SerializeField] TextMeshProUGUI StrafeSpeedDelta;

    private ShipInstance _playerShip;

    private void Awake()
    {
        EventBus.SpawnPlayerShip += OnPlayerChangeShip;
    }

    private void OnDestroy()
    {
        EventBus.SpawnPlayerShip -= OnPlayerChangeShip;
    }

    private void Update()
    {
        ShowData(_playerShip);
    }

    private void OnPlayerChangeShip(ShipInstance shipInstance)
    {
        _playerShip = shipInstance;
        var worldUnitModReversed = Constants.WorldUnitModReversed;
        var movementStaticData = shipInstance.MovementStaticData;

        MaxDirectSpeedText.text = $"MaxDirSpeed: {movementStaticData.DirectMaxSpeed * worldUnitModReversed:F0} м/с";
        MaxReverseSpeedText.text = $"MaxRevSpeed: {movementStaticData.ReverseMaxSpeed * worldUnitModReversed:F0} м/с";
        MaxStrafeSpeedText.text = $"MaxStrSpeed: {movementStaticData.StrafeMaxSpeed * worldUnitModReversed:F0} м/с";

        float directAcceleration = movementStaticData.DirectAcceleration;
        float reverseAcceleration = movementStaticData.ReverseAcceleration;
        float strafeAcceleration = movementStaticData.StrafeAcceleration;
        float rotateSpeed = movementStaticData.RotateSpeed;

        DirectAcceleration.text = $"DirAccel: {directAcceleration * worldUnitModReversed:F0} м/с²";
        ReverseAcceleration.text = $"RevAccel: {reverseAcceleration * worldUnitModReversed:F0} м/с²";
        StrafeAcceleration.text = $"StrAccel: {strafeAcceleration * worldUnitModReversed:F0} м/с²";
        RotateSpeed.text = $"RotSpeed: {rotateSpeed:F0} °";
    }

    private void ShowData(ShipInstance shipInstance)
    {
        var worldUnitModReversed = Constants.WorldUnitModReversed;
        var movementRuntimeData = shipInstance.MovementRuntimeData;
        var movementStaticData = shipInstance.MovementStaticData;

        var rb = shipInstance.Rigidbody;

        float rad = _playerShip.Rigidbody.rotation * Mathf.Deg2Rad;
        var shipForward = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
        var shipRight = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        float forwardVel = Vector2.Dot(rb.linearVelocity, shipForward);
        float sideVel = Vector2.Dot(rb.linearVelocity, shipRight);

        CurrentDirectSpeedText.text = $"DirSpeed: {forwardVel * worldUnitModReversed:F0} м/с";
        CurrentStrafeSpeedText.text = $"StrSpeed: {sideVel * worldUnitModReversed:F0} м/с";

        ThrottleText.text = $"Throttle: {movementRuntimeData.Throttle * 100:F0} %";

        if (movementRuntimeData.InertiaDampingActive)
        {
            float speed = movementRuntimeData.Throttle > 0
            ? movementStaticData.DirectMaxSpeed
            : movementStaticData.ReverseMaxSpeed;

            DirectSpeedDelta.text = $"TargetSpeed: {movementRuntimeData.Throttle * speed * worldUnitModReversed:F0} м/с";
        }
        else
        {
            float accel = movementRuntimeData.Throttle > 0
            ? movementStaticData.DirectAcceleration
            : movementStaticData.ReverseAcceleration;

            DirectSpeedDelta.text = $"Acceleration: {movementRuntimeData.Throttle * accel * worldUnitModReversed:F0} м/с";
        }
    }
}
