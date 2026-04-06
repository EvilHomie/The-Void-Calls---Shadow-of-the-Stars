using DI;

namespace GameSystems
{
    public class UpdateCashesSystem : GameSystemBase
    {
        private ObjectsStorage _objectsStorage;

        [Inject]
        public void Construct(ObjectsStorage  objectsStorage)
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
            for (int i = 0; i < _objectsStorage.NonPlayerShipsData.Count; i++)
            {
                ref var data = ref _objectsStorage.NonPlayerShipsData[i];
                ref var view = ref _objectsStorage.NonPlayerShipsView[i];
                data.Position = view.Transform.position;
            }

            _objectsStorage.PlayerShipData.Position = _objectsStorage.PlayerShipView.Transform.position;
        }
    }
}