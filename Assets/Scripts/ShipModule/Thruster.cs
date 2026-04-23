using UnityEngine;

namespace ShipModules
{
    [CreateAssetMenu(fileName = "Thruster", menuName = "Scriptable Objects/Thruster")]
    public class Thruster : ScriptableObject, IShipModule
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public float StrafeThrust { get; private set; }
        [field: SerializeField] public float RotateThrust { get; private set; }

        public ThrustersModificators ThrustersModificators;
    }

    public struct ThrustersModificators
    {
        public int StrafeThrustModPercent;
        public int RotateThrustModPercent;
    }
}

