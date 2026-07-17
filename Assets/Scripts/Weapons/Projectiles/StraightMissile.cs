namespace Projectiles
{
    public class StraightMissile : ProjectileBase
    {

        public float AccelerationSpeed;

        protected override void OnResolveDependencies()
        {
            ProjectileType = ProjectileType.StraightMissile;
        }
    }
}