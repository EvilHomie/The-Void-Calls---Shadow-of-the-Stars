using GamePools;

namespace HitParticles
{
    public class HitParticle : PoolObjectBase
    {
        public bool IsPlaying;
        void OnParticleSystemStopped()
        {
            IsPlaying = false;
        }
    }
}