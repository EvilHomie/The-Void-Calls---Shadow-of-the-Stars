using UnityEngine;

[CreateAssetMenu(fileName = "SideEngines", menuName = "Scriptable Objects/SideEngines")]
public class SideEngines : ScriptableObject, IShipModule
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public Size Size { get; private set; }
    [field: SerializeField] public float StrafeThrust { get; private set; }
    [field: SerializeField] public float RotateThrust { get; private set; }
}
