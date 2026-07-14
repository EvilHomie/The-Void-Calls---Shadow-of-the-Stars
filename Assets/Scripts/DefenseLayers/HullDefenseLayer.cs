namespace DefenseLayers
{
    public class HullDefenseLayer : DefenseLayerBase
    {
        public float CurrentArmorPoints;
        public float MaxArmorPoints;
        public ModuleType OwnModule { get; private set; }

        public void Setup(float basePoints, float armorPoints, ModuleType moduleType)
        {
            base.SetupBase(basePoints);
            CurrentArmorPoints = armorPoints;
            MaxArmorPoints = armorPoints;
            OwnModule = moduleType;
        }
    }
}