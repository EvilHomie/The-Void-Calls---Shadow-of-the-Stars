using DI;
using GameInput;
using GameSystems;
using Helpers;
using Registries;

public class PlayerWeaponGroupSystem : GameSystemBase
{
    private IPlayerInput _playerInput;
    private ShipRegistry _shipRegistry;

    [Inject]
    public void Construct(IPlayerInput playerInput, ShipRegistry shipRegistry)
    {
        _playerInput = playerInput;
        _shipRegistry = shipRegistry;
    }

    protected override void Subscribe()
    {
        base.Subscribe();
        _playerInput.ChangeAttackState += OnPlayerChangeAttackState;
        _playerInput.SwitchWeaponGroupAction += OnSwitchWeaponGroupAction;
    }

    protected override void Unsubscribe()
    {
        base.Unsubscribe();
        _playerInput.ChangeAttackState -= OnPlayerChangeAttackState;
        _playerInput.SwitchWeaponGroupAction -= OnSwitchWeaponGroupAction;
    }

    private void OnSwitchWeaponGroupAction(WeaponGroup newActiveGroup)
    {
        var playerShip = _shipRegistry.PlayerShip;

        if (playerShip.ActiveWeaponGroup == newActiveGroup) return;

        var lastActiveGroup = playerShip.ActiveWeaponGroup;
        playerShip.ActiveWeaponGroup = newActiveGroup;

        if (!playerShip.IsAttacking)
        {
            foreach (var slot in playerShip.WeaponSlots)
            {
                bool isInNewGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);
                slot.IsInActiveGroup = isInNewGroup;
            }

            EventBus.PlayerSwitchWeaponsGroupAction?.Invoke();
            return;
        }

        foreach (var slot in playerShip.WeaponSlots)
        {
            var weapon = slot.Weapon;
            bool isInNewGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);
            bool isInOldGroup = slot.WeaponGroup.ContainsAny(lastActiveGroup);

            if (isInNewGroup && !isInOldGroup)
            {
                slot.IsInActiveGroup = true;
                EventBus.WeaponChangeAttackStateAction?.Invoke(weapon, true);
            }
            else if (isInOldGroup && !isInNewGroup)
            {
                slot.IsInActiveGroup = false;
                EventBus.WeaponChangeAttackStateAction?.Invoke(weapon, false);
            }
        }

        EventBus.PlayerSwitchWeaponsGroupAction?.Invoke();
    }

    private void OnPlayerChangeAttackState(bool state)
    {
        var playerShip = _shipRegistry.PlayerShip;
        playerShip.IsAttacking = state;

        foreach (var slot in playerShip.WeaponSlots)
        {
            bool isInGroup = slot.WeaponGroup.ContainsAny(playerShip.ActiveWeaponGroup);

            if (!isInGroup) continue;

            EventBus.WeaponChangeAttackStateAction?.Invoke(slot.Weapon, state);
        }
    }
}
