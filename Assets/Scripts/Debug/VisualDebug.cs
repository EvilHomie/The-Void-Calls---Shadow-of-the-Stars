using DI;
using GameSystems;
using Ship;
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
    [SerializeField] TextMeshProUGUI FPSText;

    private ObjectsStorage _objectsStorage;

    [Inject]
    public void Construct(ShipInstance ship, ObjectsStorage objectsStorage)
    {
        _objectsStorage = objectsStorage;
    }

    private void Awake()
    {
        EventBus.PlayerChangeShip += OnPlayerChangeShip;
    }

    private void OnDestroy()
    {
        EventBus.PlayerChangeShip -= OnPlayerChangeShip;
    }

    private void Update()
    {
        UpdateShipData();
        ShowFPS();
    }

    private void OnPlayerChangeShip()
    {
        ref var playerShipData = ref _objectsStorage.PlayerShipData;

        float maxDirectSpeed = playerShipData.MovementData.DirectMaxSpeed;
        float maxReverseSpeed = playerShipData.MovementData.ReverseMaxSpeed;
        float maxStrafeSpeed = playerShipData.MovementData.StrafeMaxSpeed;

        MaxDirectSpeedText.text = $"MaxDirSpeed: {maxDirectSpeed * Constants.WorldUnitModReversed:F0} м/с";
        MaxReverseSpeedText.text = $"MaxRevSpeed: {maxReverseSpeed * Constants.WorldUnitModReversed:F0} м/с";
        MaxStrafeSpeedText.text = $"MaxStrSpeed: {maxStrafeSpeed * Constants.WorldUnitModReversed:F0} м/с";

        float directAcceleration = playerShipData.MovementData.DirectAcceleration;
        float reverseAcceleration = playerShipData.MovementData.ReverseAcceleration;
        float strafeAcceleration = playerShipData.MovementData.StrafeAcceleration;
        float rotateSpeed = playerShipData.MovementData.RotateSpeed;

        DirectAcceleration.text = $"DirAccel: {directAcceleration * Constants.WorldUnitModReversed:F0} м/с²";
        ReverseAcceleration.text = $"RevAccel: {reverseAcceleration * Constants.WorldUnitModReversed:F0} м/с²";
        StrafeAcceleration.text = $"StrAccel: {strafeAcceleration * Constants.WorldUnitModReversed:F0} м/с²";
        RotateSpeed.text = $"RotSpeed: {rotateSpeed:F0} °";
    }

    private void UpdateShipData()
    {
        ref var playerShipData = ref _objectsStorage.PlayerShipData;
        ref var playerShipView = ref _objectsStorage.PlayerShipView;

        float forwardVel = Vector2.Dot(playerShipView.Rigidbody.linearVelocity, playerShipView.Transform.up);
        float sideVel = Vector2.Dot(playerShipView.Rigidbody.linearVelocity, playerShipView.Transform.right);

        CurrentDirectSpeedText.text = $"DirSpeed: {forwardVel * Constants.WorldUnitModReversed:F0} м/с";
        CurrentStrafeSpeedText.text = $"StrSpeed: {sideVel * Constants.WorldUnitModReversed:F0} м/с";

        ThrottleText.text = $"Throttle: {playerShipData.MovementData.Throttle * 100:F0} %";

        if (playerShipData.MovementData.InertiaDamping)
        {
            float speed = playerShipData.MovementData.Throttle > 0
            ? playerShipData.MovementData.DirectMaxSpeed
            : playerShipData.MovementData.ReverseMaxSpeed;

            DirectSpeedDelta.text = $"TargetSpeed: {playerShipData.MovementData.Throttle * speed * Constants.WorldUnitModReversed:F0} м/с";
        }
        else
        {
            float accel = playerShipData.MovementData.Throttle > 0
            ? playerShipData.MovementData.DirectAcceleration
            : playerShipData.MovementData.ReverseAcceleration;

            DirectSpeedDelta.text = $"Acceleration: {playerShipData.MovementData.Throttle * accel * Constants.WorldUnitModReversed:F0} м/с";
        }
    }

    private readonly float _fpsTickRate = 10;
    float FPSCooldown;
    void ShowFPS()
    {
        FPSCooldown -= Time.deltaTime;

        if (FPSCooldown <= 0)
        {
            FPSCooldown = 1 / _fpsTickRate;
            FPSText.text = $"FPS: {1 / Time.deltaTime:F0}";
        }
    }
}
