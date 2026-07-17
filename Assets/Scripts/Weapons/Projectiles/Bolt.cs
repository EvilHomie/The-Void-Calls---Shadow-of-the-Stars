namespace Projectiles
{
    public class Bolt : ProjectileBase
    {
        protected override void OnResolveDependencies()
        {
            ProjectileType = ProjectileType.Bolt;
        }
    }
}