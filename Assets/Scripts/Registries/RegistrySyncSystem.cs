using Registries;
using System.Collections.Generic;

namespace GameSystems
{
    public class RegistrySyncSystem : GameSystemBase
    {
        private readonly List<ISyncable> _syncables = new(10);

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            GameFlowSystem.PreUpdateTick += ApplyPendings;
        }

        protected override void Unsubscribe()
        {
            GameFlowSystem.PreUpdateTick -= ApplyPendings;
        }

        public void ApplyPendings()
        {
            foreach (var syncable in _syncables)
            {
                syncable.Sync();
            }
        }

        public void Add(ISyncable syncable)
        {
            _syncables.Add(syncable);
        }
    }
}