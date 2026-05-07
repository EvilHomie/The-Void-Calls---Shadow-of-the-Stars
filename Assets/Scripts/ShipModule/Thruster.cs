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

        public ThrustersMultipliers ThrustersMultipliers;
    }

    public struct ThrustersMultipliers
    {
        public float StrafeThrustMultiplier;
        public float RotateThrustMultiplier;
    }
}

