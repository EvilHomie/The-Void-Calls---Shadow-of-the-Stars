using UnityEngine;

[CreateAssetMenu(fileName = "PoolData", menuName = "Scriptable Objects/PoolData")]
public class PoolData : ScriptableObject
{
    [field: SerializeField] public PoolReference PoolReference { get; private set; }
    [field: SerializeField] public GameObject Prefab { get; private set; }
}

[CreateAssetMenu(fileName = "PoolReference", menuName = "Scriptable Objects/PoolReference")]
public class PoolReference : ScriptableObject
{
    // Просто как ссылка для пулов
}
