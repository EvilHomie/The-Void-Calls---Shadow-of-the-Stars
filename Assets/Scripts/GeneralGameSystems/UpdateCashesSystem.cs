using DI;

namespace GameSystems
{
    public class UpdateCashesSystem : GameSystemBase
    {
        private ShipsStorage _objectsStorage;

        [Inject]
        public void Construct(ShipsStorage  objectsStorage)
        {
            _objectsStorage = objectsStorage;
        }

        protected override void AwakeInit()
        {
        }

        protected override void Subscribe()
        {
            GameFlow.PreUpdateTick += UpdateCashes;
        }

        protected override void Unsubscribe()
        {
            GameFlow.PreUpdateTick -= UpdateCashes;
        }

        private void UpdateCashes()
        {
            for (int i = 0; i <= _objectsStorage.LastUsedIndex; i++)
            {
                ref var position = ref _objectsStorage.Positions[i];
                ref var view = ref _objectsStorage.ViewDatas[i];
                position = view.Transform.position;
            }
        }
    }
}