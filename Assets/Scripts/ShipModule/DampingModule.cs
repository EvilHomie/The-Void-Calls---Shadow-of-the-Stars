using Player;
using UnityEngine;

[CreateAssetMenu(fileName = "DampingModule", menuName = "Scriptable Objects/DampingModule")]
public class DampingModule : ScriptableObject, IShipModule
{
    [field: SerializeField] public Size Size { get; private set; }
    [field: SerializeField] public float DampingMod { get; private set; }
}
