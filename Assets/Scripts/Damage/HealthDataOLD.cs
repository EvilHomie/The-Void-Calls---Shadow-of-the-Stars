using System;
using UnityEngine;

namespace Damage
{
    public class HealthDataOLD : MonoBehaviour
    {
        [SerializeField]
        public ResistanceType ResistanceType;
        public HealthPoints DefaultHealthPoints;
        public HealthPoints CurrentHealthPoints;
    }

    [Serializable]
    public struct HealthPoints
    {
        public float HullPoints;
        public float ArmorPoints;
        public float ShieldPoints;
    }
}