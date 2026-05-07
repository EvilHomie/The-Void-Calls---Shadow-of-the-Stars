using UnityEngine;

namespace ShipModules
{
    [CreateAssetMenu(fileName = "MainEngine", menuName = "Scriptable Objects/MainEngine")]
    public class MainEngine : ScriptableObject, IShipModule
    {
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Size Size { get; private set; }
        [field: SerializeField] public float DirectThrust { get; private set; }
        [field: SerializeField] public float ReverseThrust { get; private set; }
        [field: SerializeField] public float BoostThrust { get; private set; }
        [field: SerializeField] public float BoostMaxTime { get; private set; }

        public MainEngineMultipliers MainEngineMultipliers;
    }

    public struct MainEngineMultipliers
    {
        public float DirectThrustMultiplier;
        public float ReverseThrustMultiplier;
        public float BoostThrustMultiplier;
    }
}