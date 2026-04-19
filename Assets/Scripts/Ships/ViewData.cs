using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public struct ViewData
    {
        [field: SerializeField] public ShipInstance ShipInstance { get; private set; }
        [field: SerializeField] public Rigidbody2D Rigidbody { get; private set; }
        [field: SerializeField] public Transform Transform { get; private set; }
        [field: SerializeField] public MainEnginePlume[] MainEnginesPlumes { get; private set; }
        [field: SerializeField] public MainEnginePlume SideEngineFL { get; private set; }
        [field: SerializeField] public MainEnginePlume SideEngineFR { get; private set; }
        [field: SerializeField] public MainEnginePlume SideEngineBL { get; private set; }
        [field: SerializeField] public MainEnginePlume SideEngineBR { get; private set; }
        [field: SerializeField] public WeaponSlot[] WeaponSlots { get; private set; }
    }
}