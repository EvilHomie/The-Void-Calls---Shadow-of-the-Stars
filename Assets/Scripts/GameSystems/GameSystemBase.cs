using DI;
using GameSystems;
using UnityEngine;

public abstract class GameSystemBase : MonoBehaviour, ITickObserver
{
    protected GameFlowSystem GameFlowSystem;

    [Inject]
    public void Construct(GameFlowSystem gameFlowSystem)
    {
        GameFlowSystem = gameFlowSystem;
    }

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

    protected virtual void AwakeInit() { }
    protected virtual void Subscribe()
    {
        GameFlowSystem.AddTickObserver(this);
    }
    protected virtual void Unsubscribe()
    {
        GameFlowSystem.RemoveTickObserver(this);
    }
}
