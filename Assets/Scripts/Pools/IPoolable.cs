using UnityEngine;

public interface IPoolable
{
    string PoolName { get; }
    GameObject CachedGameObject { get; }
    Transform CachedTransform { get; }
    void Init();
    void ResetParams();
}
