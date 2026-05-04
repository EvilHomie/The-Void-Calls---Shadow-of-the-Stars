using DI;
using GameSystems;
using UnityEngine;

public abstract class GameSystemBase : MonoBehaviour, ITickObserver
{
    protected GameState ActiveGameState;
    protected GameFlowSystem GameFlowSystem;
    protected bool SystemIsActive;

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
        GameFlowSystem.GameStateChanged += OnGameStateChange;
        GameFlowSystem.AddTickObserver(this);
    }
    protected virtual void Unsubscribe()
    {
        GameFlowSystem.GameStateChanged -= OnGameStateChange;
        GameFlowSystem.RemoveTickObserver(this);
    }
    private void OnGameStateChange(GameState gameState)
    {
        SystemIsActive = gameState == ActiveGameState;
    }
}
