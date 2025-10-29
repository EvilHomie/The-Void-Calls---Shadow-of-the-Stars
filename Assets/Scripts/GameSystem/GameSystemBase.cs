using UnityEngine;

public abstract class GameSystemBase : MonoBehaviour
{
    protected abstract void Init();
    protected abstract void Subscribe();
    protected abstract void Unsubscribe();

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}
