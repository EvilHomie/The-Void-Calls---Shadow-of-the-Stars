using DefenseLayers;
using GameSystems;
using UnityEngine;

namespace Weapons
{
    public class MiningDrillBehaviour : IWeaponBehaviour<MiningDrill>
    {
        private static readonly RaycastHit2D[] _beamHits = new RaycastHit2D[16];
        private static readonly Collider2D[] _overlapHits = new Collider2D[16];
        public void HandleStartShoot(MiningDrill weapon)
        {
            weapon.BeamLineLR.enabled = true;
            weapon.ShootSpotPS.Play();
            ProcessShooting(weapon);
        }

        public void HandleCancelShoot(MiningDrill weapon)
        {
            weapon.BeamLineLR.enabled = false;
            weapon.ShootSpotPS.Stop();
        }

        public void ProcessShooting(MiningDrill weapon)
        {
            ref var aimStats = ref weapon.AimStats;

            var beamTransform = weapon.BeamLineTransform;
            var startPosition = beamTransform.position;
            var direction = beamTransform.up;

            int ignoredCount = 0;

            int overlapCount = Physics2D.OverlapPointNonAlloc(startPosition, _overlapHits);

            for (int i = 0; i < overlapCount; i++)
            {
                var collider = _overlapHits[i];

                if (!collider.TryGetComponent(out DefenseLayerBase layer)) continue;

                if (layer.OwnerId == weapon.OwnerId || layer.LayerType == DefenseLayerType.Shield)
                {
                    _overlapHits[ignoredCount] = collider;
                    ignoredCount++;
                }
            }

            Vector2 endPosition = startPosition + direction * aimStats.MaxDistance;
            int hitCount = Physics2D.LinecastNonAlloc(startPosition, endPosition, _beamHits, weapon.HitLayers);

            Collider2D bestCollider = null;
            Vector2 bestHitPos = weapon.WeaponStats.Cached.NoHitTargetPoint;
            int bestPriority = int.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _beamHits[i];
                var collider = hit.collider;
                bool ignored = false;

                for (int j = 0; j < ignoredCount; j++)
                {
                    if (_overlapHits[j] == collider)
                    {
                        ignored = true;
                        break;
                    }
                }

                if (ignored) continue;

                int priority;
                int layer = collider.gameObject.layer;

                if (layer == LayersId.ShieldLayer) priority = 0;
                else if (layer == LayersId.ArmorLayer) priority = 1;
                else priority = 2;

                if (priority < bestPriority)
                {
                    bestPriority = priority;
                    bestCollider = hit.collider;
                    bestHitPos = hit.point;
                }
            }

            if (bestCollider == null)
            {
                weapon.BeamLineLR.SetPosition(1, bestHitPos);
                return;
            }

            bestCollider.TryGetComponent(out DefenseLayerBase hitLayer);

            ref var runTime = ref weapon.WeaponStats.Runtime;

            if (runTime.NextHitTime < GameFlowSystem.CoreTime)
            {
                var hitDelay = weapon.WeaponStats.Cached.HitDelay;

                runTime.NextHitTime = GameFlowSystem.CoreTime + hitDelay;

                var damage = weapon.Damage;

                damage.Energy *= hitDelay;
                damage.Kinetic *= hitDelay;
                damage.Asteroid *= hitDelay;

                EventBus.BeamHitAction?.Invoke(damage, hitLayer, bestHitPos);
            }

            Vector2 localHitPos = beamTransform.InverseTransformPoint(bestHitPos);
            weapon.BeamLineLR.SetPosition(1, localHitPos);
        }
    }
}