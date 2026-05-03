using GamePools;

namespace HitParticles
{
    public class HitEffectParticle : PoolObjectBase
    {
        public bool IsPlaying;
        void OnParticleSystemStopped()
        {
            IsPlaying = false;
        }
    }
}