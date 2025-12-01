using UnityEngine;

[CreateAssetMenu(fileName = "PoolNameSO", menuName = "Scriptable Objects/PoolNameSO")]
public class PoolDataSO : ScriptableObject
{
    [field: SerializeField] public string PoolName { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}
