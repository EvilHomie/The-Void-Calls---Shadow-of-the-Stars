using DefenseLayers;
using DI;
using Registries;
using UnityEngine;

namespace CoreGameSystems
{
    public class ShieldsControlSystem : MonoBehaviour
    {
        private ShipRegistry _shipRegistry;

        [Inject]
        public void Construct(ShipRegistry shipRegistry)
        {
            _shipRegistry = shipRegistry;
        }

        public void Execute(float deltaTime)
        {
            var playerShip = _shipRegistry.PlayerShip;
            var playerShield = playerShip.Shield;
            UpdateShieldTransformRuntimeData(playerShield);
            HandleRegeneration(playerShield, deltaTime);

            foreach (var ship in _shipRegistry.Lod0Ships)
            {
                var shield = ship.Shield;
                UpdateShieldTransformRuntimeData(shield);
                HandleRegeneration(shield, deltaTime);
            }
        }

        private void UpdateShieldTransformRuntimeData(ShieldDefenseLayer layer)
        {
            ref var shieldTransformRuntimeData = ref layer.ShieldTransformRuntimeData;
            var transform = layer.transform;
            shieldTransformRuntimeData.Position = transform.position;
            shieldTransformRuntimeData.Rotation = transform.rotation;
            shieldTransformRuntimeData.LossyScale = transform.lossyScale;
        }

        private void HandleRegeneration(ShieldDefenseLayer layer, float deltaTime)
        {
            layer.CurrentBasePoints += layer.RegRate * deltaTime;
            layer.CurrentBasePoints = Mathf.Clamp(layer.CurrentBasePoints, 0, layer.MaxBasePoints);
        }
    }
}

