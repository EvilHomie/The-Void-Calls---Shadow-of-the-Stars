using DefenseLayers;
using GamePools;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

namespace Projectiles
{
    public abstract class ProjectileBase : PoolObjectBase
    {
        public float DestroyTime;
        public LayerMask HitLayers;
        public DamageData DamageData;
        public uint OwnerId;
        public HashSet<DefenseLayerBase> IgnoredLayers = new(6);

        public abstract ProjectileType ProjectileType { get; }
    }
}