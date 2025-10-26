using UnityEngine;

[CreateAssetMenu(fileName = "SideEngines", menuName = "Scriptable Objects/SideEngines")]
public class SideEngines : ScriptableObject
{
    [field: SerializeField] public float StrafeThrust { get; private set; }
    [field: SerializeField] public float RotateThrust { get; private set; }
}
