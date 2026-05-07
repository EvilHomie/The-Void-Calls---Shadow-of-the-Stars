using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }

        public bool IsAttacking;
        public WeaponGroup ActiveWeaponGroup;
        public TargetData TargetData;
        public MovementStats MovementStats;
        public MovementRuntimeData MovementRuntimeData;

        public EquipData Equip;
        public View View;
    }
}

//FlagsHelper.RemoveFlag(ref clusterAsteroid.AsteroidType, AsteroidType.Drifting);
//FlagsHelper.AddFlag(ref clusterAsteroid.AsteroidType, AsteroidType.Cluster);                        

//clusterAsteroid.HealthData.ResistanceType = ResistanceType.None;
//FlagsHelper.AddFlag(ref clusterAsteroid.HealthData.ResistanceType, ResistanceType.None);