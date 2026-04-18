using UnityEngine;

namespace Ships
{
    public class ShipInstance : MonoBehaviour
    {
        public ChassisData ChassisData;
        public MovementData MovementData;
        public EquipData EquipData;
        public ViewData View;







        public int Index = -1;




        public readonly StructList<Vector2> AimPositions = new(1000);
        public readonly StructList<Vector2> Positions = new(1000);
        public readonly StructList<ChassisData> ChassisDatas = new(1000);
        public readonly StructList<MovementData> MovementRuntimeDatas = new(1000);
        public readonly StructList<EquipData> EquipDatas = new(1000);
        public readonly StructList<ViewData> Views = new(1000);
    }
}