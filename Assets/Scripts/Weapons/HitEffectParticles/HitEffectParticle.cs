using GamePools;

namespace HitParticles
{
    public class HitEffectParticle : PoolObjectBase
    {
        public bool IsPlaying;

        protected override void OnResolveDependencies()
        {
            IsPlaying = false;
        }

        void OnParticleSystemStopped()
        {
            IsPlaying = false;
        }
    }
}