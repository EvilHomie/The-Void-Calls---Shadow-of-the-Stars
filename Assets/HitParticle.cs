using GameSystem;
using UnityEngine;

namespace Weapon
{
    public class HitParticle : MonoBehaviour, IPoolable
    {
        public string PoolName { get; set; }
        public GameObject CachedGameObject { get; private set; }
        public Transform CachedTransform { get; private set; }
        public bool InPool { get; set; }

        public virtual void Init()
        {
            CachedGameObject = gameObject;
            CachedTransform = transform;
        }

        void OnParticleSystemStopped()
        {
            EventBus.ReturnHitParticle(this);
        }
    }
}