namespace DefenseLayers
{
    public class HullDefenseLayer : DefenseLayerBase
    {
        public float CurrentArmorPoints;
        public float MaxArmorPoints;
        public ModuleType OwnModule { get; private set; }

        public void Init(float basePoints, float armorPoints, ModuleType moduleType)
        {
            base.InitBase(basePoints);
            CurrentArmorPoints = armorPoints;
            MaxArmorPoints = armorPoints;
            OwnModule = moduleType;
        }
    }
}