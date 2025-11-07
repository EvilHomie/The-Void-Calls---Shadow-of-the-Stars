using UnityEngine;

public interface IPoolable
{
    PoolDataSO PoolData { get; }
    GameObject CachedGameObject { get; }
    Transform CachedTransform { get; }
    bool InPool { get; set; }
    void Init();
}
