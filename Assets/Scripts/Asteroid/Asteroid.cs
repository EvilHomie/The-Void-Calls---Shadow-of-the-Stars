using Damage;
using GamePools;
using UnityEngine;

namespace Asteroids
{
    public class Asteroid : PoolObjectBase
    {
        [field: SerializeField] public Rigidbody2D RB { get; private set; }
        [field: SerializeField] public HealthDataOLD HealthData { get; private set; }
        [field: SerializeField] public AsteroidType AsteroidType { get; private set; }

        //public override void Init()
        //{
        //    base.Init();
        //    HealthData.ResistanceType = ResistanceType.None;
        //}
        public void ResetParams()
        {
            HealthData.CurrentHealthPoints = HealthData.DefaultHealthPoints;
        }

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