using GamePools;
using GameSystems;

namespace HitParticles
{
    public class HitParticle : PoolObjectBase
    {
        void OnParticleSystemStopped()
        {
            EventBus.HitParticleStopped(this);
        }
    }
}