using UnityEngine;

public abstract class GameSystemBase : MonoBehaviour
{
    protected abstract void AwakeInit();
    protected abstract void Subscribe();
    protected abstract void Unsubscribe();

    private void Awake()
    {
        AwakeInit();
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
