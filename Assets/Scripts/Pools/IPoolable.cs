using UnityEngine;

public interface IPoolable
{
    string ObjectName { get; }
    GameObject CachedGameObject { get; }
    Transform CachedTransform { get; }
    void Init();
}
