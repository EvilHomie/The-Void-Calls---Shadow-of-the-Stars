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
    [SerializeField] TextMeshProUGUI FPSText;

    private PlayerShip _playerShip;

    [Inject]
    public void Construct(PlayerShip ship)
    {
        _playerShip = ship;
    }

    private void Update()
    {
        OnUpdateShip(_playerShip);
        ShowFPS();
    }

    private void OnUpdateShip(PlayerShip ship)
    {
        float maxDirectSpeed = ship.ShipData.MovementData.MainEngine.DirectThrust / ship.ShipData.ChassisData.DirectDrag;
        float maxReverseSpeed = ship.ShipData.MovementData.MainEngine.ReverseThrust / ship.ShipData.ChassisData.ReverseDrag;
        float maxStrafeSpeed = ship.ShipData.MovementData.SideEngines.StrafeThrust / ship.ShipData.ChassisData.StrafeDrag;
        float directAcceleration = ship.ShipData.MovementData.MainEngine.DirectThrust / ship.ShipData.ChassisData.Mass;
        float reverseAcceleration = ship.ShipData.MovementData.MainEngine.ReverseThrust / ship.ShipData.ChassisData.Mass;
        float strafeAcceleration = ship.ShipData.MovementData.SideEngines.StrafeThrust / ship.ShipData.ChassisData.Mass;
        float rotateSpeed = ship.ShipData.MovementData.SideEngines.RotateThrust / ship.ShipData.ChassisData.RotateDrag;

        MaxDirectSpeedText.text = $"MaxDirSpeed: {maxDirectSpeed:F0} м/с";
        MaxReverseSpeedText.text = $"MaxRevSpeed: {maxReverseSpeed:F0} м/с";
        MaxStrafeSpeedText.text = $"MaxStrSpeed: {maxStrafeSpeed:F0} м/с";

        float forwardVel = Vector2.Dot(ship.Rigidbody.linearVelocity, ship.transform.up);
        float sideVel = Vector2.Dot(ship.Rigidbody.linearVelocity, ship.transform.right);

        CurrentDirectSpeedText.text = $"DirSpeed: {forwardVel * Constants.WorldUnitMod:F0} м/с";
        CurrentStrafeSpeedText.text = $"StrSpeed: {sideVel * Constants.WorldUnitMod:F0} м/с";

        if (ship.ShipData.MovementData.InertiaDamping)
        {
            float speed = ship.ShipData.MovementData.Throttle > 0
            ? maxDirectSpeed
            : maxReverseSpeed;

            ThrottleText.text = $"Throttle: {ship.ShipData.MovementData.Throttle * speed:F0} м/с";
        }
        else
        {
            float accel = ship.ShipData.MovementData.Throttle > 0
            ? directAcceleration
            : reverseAcceleration;

            ThrottleText.text = $"Throttle: {ship.ShipData.MovementData.Throttle * accel:F0} м/с";
        }


        DirectAcceleration.text = $"DirAccel: {directAcceleration:F0} м/с²";
        ReverseAcceleration.text = $"RevAccel: {reverseAcceleration:F0} м/с²";
        StrafeAcceleration.text = $"StrAccel: {strafeAcceleration:F0} м/с²";
        RotateSpeed.text = $"RotSpeed: {rotateSpeed:F0} °";


    }

    float fpsTickRate = 5;
    float FPSCooldown;
    void ShowFPS()
    {
        FPSCooldown -= Time.deltaTime;

        if (FPSCooldown <= 0)
        {
            FPSCooldown = 1 / fpsTickRate;
            FPSText.text = $"FPS: {1 / Time.deltaTime:F0}";
        }
    }
}
