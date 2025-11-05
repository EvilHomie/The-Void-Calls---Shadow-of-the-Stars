using Unity.Cinemachine;
using UnityEngine;

namespace Ship
{
    public class ShipData : MonoBehaviour
    {
        [MinMaxRangeSlider(1f, 10f)]
        [SerializeField] Vector2 _minMaxViewDistance;
        public Vector2 MinMaxViewDistance => _minMaxViewDistance;
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public ShipChassisData ChassisData { get; private set; }
        [field: SerializeField] public ShipMovementData MovementData { get; private set; }
        [field: SerializeField] public ShipMovementView MovementView { get; private set; }
        [field: SerializeField] public ShipWeaponData WeaponData { get; private set; }

        private void Start()
        {
            InitAllModuls();
        }
        private void InitAllModuls()
        {
            MovementView.Init();
            WeaponData.Init();
        }
    }
}