using GameSystems;
using Ships;
using TMPro;
using UnityEngine;

public class ShipStatsDebug : MonoBehaviour
{
    // StaticData
    [SerializeField] TextMeshProUGUI MaxDirectSpeedText;
    [SerializeField] TextMeshProUGUI DirectDamperForceText;
    [SerializeField] TextMeshProUGUI MaxReverseSpeedText;
    [SerializeField] TextMeshProUGUI ReverseDamperForceText;
    [SerializeField] TextMeshProUGUI MaxStrafeSpeedText;
    [SerializeField] TextMeshProUGUI StrafeDamperForceText;
    [SerializeField] TextMeshProUGUI DirectAcceleration;
    [SerializeField] TextMeshProUGUI ReverseAcceleration;
    [SerializeField] TextMeshProUGUI StrafeAcceleration;
    [SerializeField] TextMeshProUGUI RotateSpeed;

    // RealTimeData
    [SerializeField] TextMeshProUGUI CurrentDirectSpeedText;
    [SerializeField] TextMeshProUGUI CurrentDirectAccelerationText;
    [SerializeField] TextMeshProUGUI CurrentStrafeSpeedText;
    [SerializeField] TextMeshProUGUI CurrentStrafeAccelerationText;
    [SerializeField] TextMeshProUGUI ThrottleText;
    [SerializeField] TextMeshProUGUI DirectSpeedDelta;
    [SerializeField] TextMeshProUGUI StrafeSpeedDelta;
    [SerializeField] TextMeshProUGUI BoosterPower;

    private ShipInstance _playerShip;
    private Vector2 _lastVelocity;

    private void Awake()
    {
        EventBus.SpawnPlayerShip += OnPlayerChangeShip;
        GameFlowSystem.FixedGameTick += ShowData;
    }

    private void OnDestroy()
    {
        EventBus.SpawnPlayerShip -= OnPlayerChangeShip;
        GameFlowSystem.FixedGameTick -= ShowData;
    }

    private void OnPlayerChangeShip(ShipInstance shipInstance)
    {
        _playerShip = shipInstance;
        var worldUnitModReversed = WorldConfig.WorldUnitModReversed;
        var movementStaticData = shipInstance.MovementStaticData;

        MaxDirectSpeedText.text = $"MaxDirSpeed: {movementStaticData.DirectMaxSpeed * worldUnitModReversed:F0} м/с";
        MaxReverseSpeedText.text = $"MaxRevSpeed: {movementStaticData.ReverseMaxSpeed * worldUnitModReversed:F0} м/с";
        MaxStrafeSpeedText.text = $"MaxStrSpeed: {movementStaticData.StrafeMaxSpeed * worldUnitModReversed:F0} м/с";

        DirectDamperForceText.text = $"DirectDamper: {movementStaticData.DirectDampingAcceleration * worldUnitModReversed:F0} м/с";
        ReverseDamperForceText.text = $"ReverseDamper: {movementStaticData.ReverseDampingAcceleration * worldUnitModReversed:F0} м/с";
        StrafeDamperForceText.text = $"StrafeDamper: {movementStaticData.StrafeDampingAcceleration * worldUnitModReversed:F0} м/с";

        float directAcceleration = movementStaticData.DirectAcceleration;
        float reverseAcceleration = movementStaticData.ReverseAcceleration;
        float strafeAcceleration = movementStaticData.StrafeAcceleration;
        float rotateSpeed = movementStaticData.RotateSpeed;

        DirectAcceleration.text = $"DirAccel: {directAcceleration * worldUnitModReversed:F0} м/с²";
        ReverseAcceleration.text = $"RevAccel: {reverseAcceleration * worldUnitModReversed:F0} м/с²";
        StrafeAcceleration.text = $"StrAccel: {strafeAcceleration * worldUnitModReversed:F0} м/с²";
        RotateSpeed.text = $"RotSpeed: {rotateSpeed:F0} °";
    }

    private void ShowData(float fixedDT)
    {
        var worldUnitModReversed = WorldConfig.WorldUnitModReversed;
        var movementRuntimeData = _playerShip.MovementRuntimeData;
        var movementStaticData = _playerShip.MovementStaticData;

        var rb = _playerShip.Rigidbody;

        var rad = _playerShip.Rigidbody.rotation * Mathf.Deg2Rad;
        var shipForward = new Vector2(-Mathf.Sin(rad), Mathf.Cos(rad));
        var shipRight = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        var currentForwardVel = Vector2.Dot(rb.linearVelocity, shipForward);
        var currentStrafeVel = Vector2.Dot(rb.linearVelocity, shipRight);

        var lastForwardVel = Vector2.Dot(_lastVelocity, shipForward);
        var lastStrafeVel = Vector2.Dot(_lastVelocity, shipRight);

        CurrentDirectSpeedText.text = $"DirSpeed: {currentForwardVel * worldUnitModReversed:F0} м/с";
        CurrentStrafeSpeedText.text = $"StrSpeed: {currentStrafeVel * worldUnitModReversed:F0} м/с";

        CurrentDirectAccelerationText.text = $"DirAccel: {(currentForwardVel - lastForwardVel) * worldUnitModReversed / fixedDT:F2} м/с";
        CurrentStrafeAccelerationText.text = $"StrAccel: {(currentStrafeVel - lastStrafeVel) * worldUnitModReversed / fixedDT:F2} м/с";

        ThrottleText.text = $"Throttle: {movementRuntimeData.DirectThrottle * 100:F0} %";

        if (movementRuntimeData.InertiaDampingIsActive)
        {
            float speed;

            if (movementRuntimeData.BoostersIsActive)
            {
                speed = movementStaticData.BoostersMaxSpeed;
            }
            else
            {
                speed = movementRuntimeData.DirectThrottle > 0
                    ? movementStaticData.DirectMaxSpeed
                    : movementStaticData.ReverseMaxSpeed;
            }

            DirectSpeedDelta.text = $"TargetSpeed: {movementRuntimeData.DirectThrottle * speed * worldUnitModReversed:F0} м/с";
        }
        else
        {
            float accel = movementRuntimeData.DirectThrottle > 0
                   ? movementStaticData.DirectAcceleration
                   : movementStaticData.ReverseAcceleration;

            if (movementRuntimeData.BoostersIsActive)
            {
                accel = movementStaticData.BoostersAcceleration;
            }
            else
            {
                accel = movementRuntimeData.DirectThrottle > 0
                   ? movementStaticData.DirectAcceleration
                   : movementStaticData.ReverseAcceleration;
            }

            DirectSpeedDelta.text = $"Acceleration: {movementRuntimeData.DirectThrottle * accel * worldUnitModReversed:F0} м/с";
        }

        BoosterPower.text = $"Power: {movementRuntimeData.BoostersPower:F1} м/с";

        _lastVelocity = _playerShip.Rigidbody.linearVelocity;
    }
}
