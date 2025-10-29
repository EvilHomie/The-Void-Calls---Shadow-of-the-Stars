using DI;
using GameSystem;
using Player;
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


    private PlayerShipData _playerShipData;

    [Inject]
    public void Construct(PlayerShipData ship)
    {
        _playerShipData = ship;
    }

    private void Update()
    {
        OnUpdateShip(_playerShipData);
    }

    private void OnUpdateShip(PlayerShipData ship)
    {
        float maxDirectSpeed = ship.MovementData.MainEngine.DirectThrust / ship.ChassisData.DirectDrag;
        float maxReverseSpeed = ship.MovementData.MainEngine.ReverseThrust / ship.ChassisData.ReverseDrag;
        float maxStrafeSpeed = ship.MovementData.SideEngines.StrafeThrust / ship.ChassisData.StrafeDrag;
        float directAcceleration = ship.MovementData.MainEngine.DirectThrust / ship.ChassisData.Mass;
        float reverseAcceleration = ship.MovementData.MainEngine.ReverseThrust / ship.ChassisData.Mass;
        float strafeAcceleration = ship.MovementData.SideEngines.StrafeThrust / ship.ChassisData.Mass;
        float rotateSpeed = PlayerMovementSystem._rotateMod * ship.MovementData.SideEngines.RotateThrust / ship.ChassisData.RotateDrag;

        MaxDirectSpeedText.text = $"MaxDirSpeed: {maxDirectSpeed * 1000:F0} м/с";
        MaxReverseSpeedText.text = $"MaxRevSpeed: {maxReverseSpeed * 1000:F0} м/с";
        MaxStrafeSpeedText.text = $"MaxStrSpeed: {maxStrafeSpeed * 1000:F0} м/с";

        float forwardVel = Vector2.Dot(ship.Rigidbody.linearVelocity, ship.transform.up);
        float sideVel = Vector2.Dot(ship.Rigidbody.linearVelocity, ship.transform.right);

        CurrentDirectSpeedText.text = $"DirSpeed: {forwardVel * 1000:F0} м/с";
        CurrentStrafeSpeedText.text = $"StrSpeed: {sideVel * 1000:F0} м/с";

        if (ship.MovementData.InertiaDamping)
        {
            float speed = ship.MovementData.Throttle > 0
            ? maxDirectSpeed
            : maxReverseSpeed;

            ThrottleText.text = $"Throttle: {ship.MovementData.Throttle * speed * 1000:F0} м/с";
        }
        else
        {
            float accel = ship.MovementData.Throttle > 0
            ? directAcceleration
            : reverseAcceleration;

            ThrottleText.text = $"Throttle: {ship.MovementData.Throttle * accel * 1000:F0} м/с";
        }


        DirectAcceleration.text = $"DirAccel: {directAcceleration * 1000:F0} м/с²";
        ReverseAcceleration.text = $"RevAccel: {reverseAcceleration * 1000:F0} м/с²";
        StrafeAcceleration.text = $"StrAccel: {strafeAcceleration * 1000:F0} м/с²";
        RotateSpeed.text = $"RotSpeed: {rotateSpeed:F0} °";
    }
}
