using DefenseLayers;
using DI;
using GameCamera;
using Registries;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class ShieldRepulsionSystem : GameSystemBase, IFixedUpdateTickObserver
    {
        [Inject]
        public void Construct(ShipRegistry shipRegistry, MouseCursor mouseCursor)
        {
            //_mouseCursor = mouseCursor;
            //_shipRegistry = shipRegistry;
            ActiveGameState = GameState.CoreGameplay;
        }

        public void FixedUpdateTick(float fixedDT)
        {
            if (!SystemIsActive) return;

            foreach ((Rigidbody2D rb, Transform transform) in _trackedBodies)
            {
                if (!rb)
                    continue;

                ProcessRepulsion(rb, transform);
            }
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            EventBus.OnShieldCross += HandleShieldCross;
        }

        protected override void Unsubscribe()
        {
            base.Unsubscribe();
            EventBus.OnShieldCross -= HandleShieldCross;
        }


        [SerializeField] private float _shieldForce = 25f;
        [SerializeField] private float _deadZone = 0.02f;

        private readonly HashSet<(Rigidbody2D rb, Transform transform)> _trackedBodies = new();

        private void HandleShieldCross(Collider2D other, Transform transform, bool entered)
        {
            if (!other.attachedRigidbody)
                return;

            Rigidbody2D rb = other.attachedRigidbody;

            if (entered)
            {
                _trackedBodies.Add((rb, transform));
            }
            else
            {
                _trackedBodies.Remove((rb, transform));
            }
        }

        private void ProcessRepulsion(Rigidbody2D rb, Transform transform)
        {
            Vector2 shieldCenter = transform.position;
            Vector2 objectPosition = rb.position;

            Vector2 fromCenter = objectPosition - shieldCenter;

            // Shield ellipse radius
            float radiusX = transform.localScale.x * 0.5f;
            float radiusY = transform.localScale.y * 0.5f;

            // Distance inside ellipse
            float normalizedDistance =
                Mathf.Sqrt(
                    Mathf.Pow(fromCenter.x / radiusX, 2f) +
                    Mathf.Pow(fromCenter.y / radiusY, 2f));

            // 0 = edge
            // 1 = center
            float penetration = 1f - normalizedDistance;

            if (penetration <= _deadZone)
                return;

            // Чем глубже объект внутри —
            // тем сильнее выталкивание
            float pushForce = penetration * _shieldForce;

            rb.AddForce(fromCenter * pushForce, ForceMode2D.Force);
        }
    }
}

