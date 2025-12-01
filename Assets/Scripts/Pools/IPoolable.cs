using UnityEngine;

public interface IPoolable
{
    public string PoolName { get; set; }
    GameObject CachedGameObject { get; }
    Transform CachedTransform { get; }
    bool InPool { get; set; }
    void Init();
}
