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

        if (!playerShip.IsAttacking)
        {
            playerShip.ActiveWeaponGroup = newActiveGroup;
            EventBus.PlayerSwitchWeaponGroupAction?.Invoke();
            return;
        }

        var lastActiveGroup = playerShip.ActiveWeaponGroup;
        playerShip.ActiveWeaponGroup = newActiveGroup;

        foreach (var slot in playerShip.WeaponSlots)
        {
            var weapon = slot.Weapon;
            //if (weapon == null) continue;

            bool isInNewGroup = slot.WeaponGroup.ContainsAny(newActiveGroup);
            bool isInOldGroup = slot.WeaponGroup.ContainsAny(lastActiveGroup);

            if (isInNewGroup && !isInOldGroup)
            {
                EventBus.WeaponChangeAttackStateAction?.Invoke(weapon, true);
            }
            else if (isInOldGroup && !isInNewGroup)
            {
                EventBus.WeaponChangeAttackStateAction?.Invoke(weapon, false);
            }
        }

        EventBus.PlayerSwitchWeaponGroupAction?.Invoke();
    }

    private void OnPlayerChangeAttackState(bool state)
    {
        var playerShip = _shipRegistry.PlayerShip;
        playerShip.IsAttacking = state;

        foreach (var slot in playerShip.WeaponSlots)
        {
            //if (slot.Weapon == null) continue;

            bool isInGroup = slot.WeaponGroup.ContainsAny(playerShip.ActiveWeaponGroup);

            if (!isInGroup) continue;

            EventBus.WeaponChangeAttackStateAction?.Invoke(slot.Weapon, state);
        }
    }
}
