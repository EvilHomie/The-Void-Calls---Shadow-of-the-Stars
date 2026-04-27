using DI;
using GameInput;
using Helpers;
using Registries;
using Ships;

namespace GameSystems
{
    public class WeaponAttackSystem : GameSystemBase, IUpdateTickObserver
    {
        private WeaponsBehaviour _weaponsBehaviour;
        private IPlayerInput _playerInput;
        private WeaponRegistry _weaponRegistry;
        private ShipRegistry _shipRegistry;

        [Inject]
        public void Construct(IPlayerInput playerInput, WeaponRegistry weaponRegistry, ShipRegistry shipRegistry)
        {
            _playerInput = playerInput;
            _weaponRegistry = weaponRegistry;
            _shipRegistry = shipRegistry;
            _weaponsBehaviour = new WeaponsBehaviour();
            ActiveGameState = GameState.CoreGameplay;
        }

        public void UpdateTick(float deltaTime)
        {
            if (!SystemIsActive) return;

            OnUpdateTick(deltaTime);
        }

        protected override void Subscribe()
        {
            base.Subscribe();
            _playerInput.ChangeAtackState += OnPlayerChangeAttackState;
            EventBus.NonPlayerChangeAttackState += OnNonPlayerChangeAttackState;
        }

        protected override void Unsubscribe()
        {
            _playerInput.ChangeAtackState -= OnPlayerChangeAttackState;
            EventBus.NonPlayerChangeAttackState += OnNonPlayerChangeAttackState;
        }
        
        private void OnPlayerChangeAttackState(bool state)
        {
            var playerShip = _shipRegistry.PlayerShip;
            OnShipChangeAttackState(playerShip, state);
        }
        private void OnNonPlayerChangeAttackState(ShipInstance ship, bool state)
        {
            OnShipChangeAttackState(ship, state);
        }


        private void OnShipChangeAttackState(ShipInstance ship, bool state)
        {
            if (state)
            {
                foreach (var slot in ship.WeaponSlots)
                {
                    if (slot.IsActive)
                    {
                        _weaponRegistry.RequestAddOnStartAttack(slot.Weapon);
                        _weaponsBehaviour.StartShoot(slot.Weapon);
                    }
                }
            }
            else
            {
                foreach (var slot in ship.WeaponSlots)
                {
                    _weaponRegistry.RequestRemoveOnStopAttack(slot.Weapon);
                    _weaponsBehaviour.CancelShoot(slot.Weapon);
                }
            }
        }

        private void OnUpdateTick(float dTime)
        {
            Aim(dTime);
            ProceedShooting();
        }

        private void Aim(float dTime)
        {
            var playerShip = _shipRegistry.PlayerShip;
            WeaponSystemHelper.AimAtTarget(playerShip, dTime);

            foreach (var ship in _shipRegistry.ShipsInFight)
            {
                WeaponSystemHelper.AimAtTarget(ship, dTime);
            }
        }

        private void ProceedShooting()
        {
            foreach (var weapon in _weaponRegistry.ActiveWeapons)
            {
                _weaponsBehaviour.ProceedShoot(weapon);
            }
        }
    }
}
