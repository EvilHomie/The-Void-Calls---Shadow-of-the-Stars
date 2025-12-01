namespace Asteroid
{
    public class ClusterAsteroid : AsteroidBase
    {
        private void OnBecameInvisible()
        {
            RB.simulated = false;
        }

        private void OnBecameVisible()
        {
            RB.simulated = true;
        }
    }
}