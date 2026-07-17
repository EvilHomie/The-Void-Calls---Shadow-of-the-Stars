using DI;
using GamePools;
using General;
using Helpers;
using Registries;
using Ships;
using System.Threading.Tasks;
using UnityEngine;

namespace CoreGameSystems
{
    public class SpawnShipsSystem : MonoBehaviour
    {
        private ShipRegistry _shipRegistry;
        private GameFlowSystem _gameFlowSystem;
        private ShipsPool _shipsPool;

        [SerializeField] ShipId testPlayerShip;
        [SerializeField] ShipId[] testEnemies;

        [Inject]
        public void Construct(ShipRegistry shipRegistry, GameFlowSystem gameFlowSystem, ShipsPool shipsPool)
        {
            _shipRegistry = shipRegistry;
            _gameFlowSystem = gameFlowSystem;
            _shipsPool = shipsPool;
        }

        private async Awaitable Start() // временный метод для запуска кор логики
        {
            var shipPoolId = _shipsPool.GetPoolIdByShipId(testPlayerShip);

            var playerShipInstance = _shipsPool.Getitem(shipPoolId);
            playerShipInstance.Transform.position = Vector3.zero;


            InitHelper.InitShip(playerShipInstance);
            _shipRegistry.RegisterPlayerShip(playerShipInstance);
            EventBus.PlayerShipSpawned?.Invoke(playerShipInstance);

            SetDepthPos(playerShipInstance, Vector2.zero);
            await SpawnEnemies();




            _gameFlowSystem.ChangeGameState(GameState.CoreGameplay);
        }

        private async Awaitable SpawnEnemies()
        {
            foreach (var enemyId in testEnemies)
            {
                await Awaitable.NextFrameAsync();
                var placementRadius = GameConfig.ChassisBaseStats.GetStats(enemyId).PlacementRadius;

                if (SpawnPositionFinder.TryFindPosition(Vector2.one * 2, placementRadius, out Vector2 spawnPosition))
                {
                    var enemyShipPoolId = _shipsPool.GetPoolIdByShipId(enemyId);
                    var enemyInstance = _shipsPool.Getitem(enemyShipPoolId);

                    InitHelper.InitShip(enemyInstance);
                    //enemyInstance.AimData.TargetRigidBody = playerShipInstance.Rigidbody;
                    _shipRegistry.RegisterShip(enemyInstance, SimulationLevel.Lod0);
                    SetDepthPos(enemyInstance, spawnPosition);
                }
            }
        }

        private void SetDepthPos(ShipInstance shipInstance, Vector3 pos)
        {
            pos.z = 1;
            shipInstance.transform.position = pos;
        }

    }
}

public static class SpawnPositionFinder
{
    private const int MaxRings = 4;
    private const float RingStepMultiplier = 1f;
    private const int FirstRingDirections = 8;
    private static readonly Vector2[] SpawnOffsets;

    static SpawnPositionFinder()
    {
        int totalPoints = 0;

        for (int ring = 1; ring <= MaxRings; ring++)
        {
            totalPoints += ring * FirstRingDirections;
        }

        SpawnOffsets = new Vector2[totalPoints];
        int index = 0;

        for (int ring = 1; ring <= MaxRings; ring++)
        {
            int points = ring * FirstRingDirections;
            float radius = ring * RingStepMultiplier;
            float angleStep = 2f * Mathf.PI / points;

            for (int i = 0; i < points; i++)
            {
                float angle = i * angleStep;

                SpawnOffsets[index++] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }
        }
    }

    public static bool TryFindPosition(Vector2 center, float placementRadius, out Vector2 position)
    {
        if (placementRadius <= 0f || IsPositionFree(center, placementRadius))
        {
            position = center;
            return true;
        }

        foreach (Vector2 offset in SpawnOffsets)
        {
            Vector2 candidate = center + offset * placementRadius;

            if (IsPositionFree(candidate, placementRadius))
            {
                position = candidate;
                return true;
            }
        }

        position = center;
        return false;
    }

    private static bool IsPositionFree(Vector2 position, float placementRadius)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, placementRadius);
        return hit == null;
    }
}