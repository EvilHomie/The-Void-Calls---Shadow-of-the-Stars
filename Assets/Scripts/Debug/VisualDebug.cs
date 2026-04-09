using DI;
using GameSystems;
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

    private ShipsStorage _objectsStorage;

    [Inject]
    public void Construct(ShipsStorage objectsStorage)
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
        ShowData();
        ShowFPS();
    }

    private void OnPlayerChangeShip()
    {
        ref var chassisData = ref _objectsStorage.ChassisDatas[_objectsStorage.PlayerIndex];

        MaxDirectSpeedText.text = $"MaxDirSpeed: {chassisData.DirectMaxSpeed * Constants.WorldUnitModReversed:F0} м/с";
        MaxReverseSpeedText.text = $"MaxRevSpeed: {chassisData.ReverseMaxSpeed * Constants.WorldUnitModReversed:F0} м/с";
        MaxStrafeSpeedText.text = $"MaxStrSpeed: {chassisData.StrafeMaxSpeed * Constants.WorldUnitModReversed:F0} м/с";

        float directAcceleration = chassisData.DirectMaxAcceleration;
        float reverseAcceleration = chassisData.ReverseMaxAcceleration;
        float strafeAcceleration = chassisData.StrafeMaxAcceleration;
        float rotateSpeed = chassisData.RotateMaxSpeed;

        DirectAcceleration.text = $"DirAccel: {directAcceleration * Constants.WorldUnitModReversed:F0} м/с²";
        ReverseAcceleration.text = $"RevAccel: {reverseAcceleration * Constants.WorldUnitModReversed:F0} м/с²";
        StrafeAcceleration.text = $"StrAccel: {strafeAcceleration * Constants.WorldUnitModReversed:F0} м/с²";
        RotateSpeed.text = $"RotSpeed: {rotateSpeed:F0} °";
    }

    private void ShowData()
    {
        var playerIndex = _objectsStorage.PlayerIndex;
        ref var movementData = ref _objectsStorage.MovementDatas[playerIndex];
        ref var view = ref _objectsStorage.ViewsDatas[playerIndex];
        ref var chassisData = ref _objectsStorage.ChassisDatas[playerIndex];

        float forwardVel = Vector2.Dot(view.Rigidbody.linearVelocity, view.Transform.up);
        float sideVel = Vector2.Dot(view.Rigidbody.linearVelocity, view.Transform.right);

        CurrentDirectSpeedText.text = $"DirSpeed: {forwardVel * Constants.WorldUnitModReversed:F0} м/с";
        CurrentStrafeSpeedText.text = $"StrSpeed: {sideVel * Constants.WorldUnitModReversed:F0} м/с";

        ThrottleText.text = $"Throttle: {movementData.Throttle * 100:F0} %";

        if (movementData.InertiaDampingState)
        {
            float speed = movementData.Throttle > 0
            ? chassisData.DirectMaxSpeed
            : chassisData.ReverseMaxSpeed;

            DirectSpeedDelta.text = $"TargetSpeed: {movementData.Throttle * speed * Constants.WorldUnitModReversed:F0} м/с";
        }
        else
        {
            float accel = movementData.Throttle > 0
            ? chassisData.DirectMaxAcceleration
            : chassisData.ReverseMaxAcceleration;

            DirectSpeedDelta.text = $"Acceleration: {movementData.Throttle * accel * Constants.WorldUnitModReversed:F0} м/с";
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
