using DI;
using Registries;
using UnityEngine;

namespace GameSystems
{
    public class ShieldRepulsionSystem : GameSystemBase, IFixedUpdateTickObserver
    {
        [SerializeField] private float _shieldForce = 25f;
        [SerializeField] private float _deadZone = 0.02f;

        private ShieldRepulsionRegistry _shieldRepulsionRegistry;

        [Inject]
        public void Construct(ShieldRepulsionRegistry shieldRepulsionRegistry)
        {
            _shieldRepulsionRegistry = shieldRepulsionRegistry;
            ActiveGameState = GameState.CoreGameplay;
        }

        public void FixedUpdateTick(float fixedDT)
        {
            if (!SystemIsActive) return;

            foreach ((Rigidbody2D rb, Transform transform) in _shieldRepulsionRegistry.TrackedBodies)
            {
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

        private void HandleShieldCross(Collider2D other, Transform transform, bool entered)
        {
            if (!other.attachedRigidbody) return;
            var rb = other.attachedRigidbody;

            if (entered) _shieldRepulsionRegistry.RequestAdd(rb, transform);
            else _shieldRepulsionRegistry.RequestRemove(rb, transform);
        }

        private void ProcessRepulsion(Rigidbody2D rb, Transform transform)
        {
            Vector2 shieldCenter = transform.position;
            var objectPosition = rb.position;

            var fromCenter = objectPosition - shieldCenter;

            // Shield ellipse radius
            var radiusX = transform.localScale.x * 0.5f;
            var radiusY = transform.localScale.y * 0.5f;
            var x = fromCenter.x / radiusX;
            var y = fromCenter.y / radiusY;
            var normalizedDistanceSq = x * x + y * y;

            // глубина проникновения где 0 = edge 1 = center
            var penetration = 1f - normalizedDistanceSq;
            // отсекает эффект на границе и если за пределами.
            if (penetration <= _deadZone) return;

            // Чем глубже объект внутри тем сильнее выталкивание
            var pushForce = penetration * _shieldForce;

            rb.AddForce(fromCenter.normalized * pushForce, ForceMode2D.Force);
        }
    }
}

