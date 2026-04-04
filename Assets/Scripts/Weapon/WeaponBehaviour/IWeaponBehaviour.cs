namespace Weapons
{
    public interface IWeaponBehaviour<TWeapon> where TWeapon : WeaponBase
    {
        void StartShoot(TWeapon weapon);
        void CancelShoot(TWeapon weapon);
        void ProceedShoot(TWeapon weapon);
    }
}