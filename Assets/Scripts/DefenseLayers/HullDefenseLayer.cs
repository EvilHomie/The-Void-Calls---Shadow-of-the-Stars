namespace DefenseLayers
{
    public class HullDefenseLayer : DefenseLayerBase
    {
        public float CurrentArmorPoints;
        public float MaxArmorPoints;

        public void Init(float basePoints, float armorPoints = 0)
        {
            base.InitBase(basePoints);
            CurrentArmorPoints = armorPoints;
            MaxArmorPoints = armorPoints;
        }
    }
}