using System;
using UnityEngine;

namespace Ships
{
    [Serializable]
    public struct EquipData
    {        
        [field: SerializeField] public Chassis Chassis { get; set; }
        [field: SerializeField] public MainEngine MainEngine { get; set; }
        [field: SerializeField] public SideEngines SideEngines { get; set; }
    }
}