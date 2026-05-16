using DI;
using GameSystems;
using System.Collections.Generic;
using UnityEngine;

namespace Registries
{
    public class ShieldRepulsionRegistry : MonoBehaviour, ICorePreUpdateTickObserver
    {
        public IReadOnlyCollection<(Rigidbody2D rb, Transform transform)> TrackedBodies => _trackedBodies;

        private readonly HashSet<(Rigidbody2D rb, Transform transform)> _trackedBodies = new(200);
        private readonly HashSet<(Rigidbody2D rb, Transform transform)> _trackedBodiesToAdd = new(20);
        private readonly HashSet<(Rigidbody2D rb, Transform transform)> _trackedBodiesToRemove = new(20);

        [Inject]
        public void Construct(GameFlowSystem gameFlowSystem)
        {
            gameFlowSystem.AddTickObserver(this);
        }

        public void CorePreUpdateTick()
        {
            Sync();
        }

        public void RequestAdd(Rigidbody2D rb, Transform transform)
        {            
            _trackedBodiesToRemove.Remove((rb, transform));
            _trackedBodiesToAdd.Add((rb, transform));
        }

        public void RequestRemove(Rigidbody2D rb, Transform transform)
        {
            _trackedBodiesToAdd.Remove((rb, transform));
            _trackedBodiesToRemove.Add((rb, transform));
        }

        public void Sync()
        {
            foreach (var body in _trackedBodiesToRemove)
            {
                _trackedBodies.Remove(body);
            }

            _trackedBodiesToRemove.Clear();

            foreach (var body in _trackedBodiesToAdd)
            {
                _trackedBodies.Add(body);
            }

            _trackedBodiesToAdd.Clear();
        }
    }
}