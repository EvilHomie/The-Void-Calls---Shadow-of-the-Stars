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

            if (playerShield != null)
            {
                UpdateShieldTransformRuntimeData(playerShield);
                HandleRegeneration(playerShield, deltaTime);
            }

            foreach (var ship in _shipRegistry.Lod0Ships)
            {
                var shield = ship.Shield;

                if (shield != null)
                {
                    UpdateShieldTransformRuntimeData(shield);
                    HandleRegeneration(shield, deltaTime);
                }  
            }
        }

        private void UpdateShieldTransformRuntimeData(ShieldDefenseLayer layer)
        {
            ref var shieldTransformRuntimeData = ref layer.ShieldTransformRuntimeData;
            var transform = layer.Transform;
            shieldTransformRuntimeData.WorldPosition = transform.position;
            shieldTransformRuntimeData.WorldRotation = transform.rotation;
            var currentLocalScale = shieldTransformRuntimeData.DeffaultLocalScale * shieldTransformRuntimeData.RadiusMod;
            transform.localScale = currentLocalScale;
            shieldTransformRuntimeData.CurrentLocalScale = currentLocalScale;
            shieldTransformRuntimeData.CurrentLossyScale = currentLocalScale * shieldTransformRuntimeData.lossySize;
        }

        private void HandleRegeneration(ShieldDefenseLayer layer, float deltaTime)
        {
            layer.CurrentBasePoints += layer.RegRate * deltaTime;
            layer.CurrentBasePoints = Mathf.Clamp(layer.CurrentBasePoints, 0, layer.MaxBasePoints);
        }
    }
}

