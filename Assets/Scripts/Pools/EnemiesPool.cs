
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameSystem
{
    //public class EnemiesPool : AbstractPool<EnemyBase>
    //{        
    //    private GameObject _enemiesContainer;
    //    private List<string> _enemyNames = new();

    //    private void Awake()
    //    {
    //        _enemiesContainer = new GameObject("Pool_Container_Enemies");
    //        _enemyNames = Config.Enemies.Select(enemy => enemy.UsedName).ToList();
    //        CreateItemPools(Config.Enemies, Config.EnemyPoolStartCapacity, Config.EnemyPoolMaxCapacity, _enemiesContainer.transform, Config.EnemyPoolPrewarmCount);
    //    }

    //    protected override void Subscribe()
    //    {
    //        EventBus.EnemyDie += ScheduleForRelease;
    //        GameFlowSystem.ChangeGameState += OnChangeGameState;
    //        GameFlowSystem.SystemsUpdateTick += ReleasePendingItems;
    //    }

    //    protected override void Unsubscribe()
    //    {
    //        EventBus.EnemyDie -= ScheduleForRelease;
    //        GameFlowSystem.ChangeGameState -= OnChangeGameState;
    //        GameFlowSystem.SystemsUpdateTick -= ReleasePendingItems;
    //    }

    //    private void OnChangeGameState(GameState gameState)
    //    {
    //        if (gameState == GameState.GameOver || gameState == GameState.Victory)
    //        {
    //            ReleaseAll();
    //        }
    //    }

    //    public EnemyBase GetRandomEnemy()
    //    {
    //        var randomIndex = Random.Range(0, _enemyNames.Count);
    //        var randomName = _enemyNames[randomIndex];
    //        return Getitem(randomName);
    //    }
    //}
}