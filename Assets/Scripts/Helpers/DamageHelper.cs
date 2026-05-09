namespace Helpers
{
    //public enum DestroyedPartType
    //{
    //    None = 0,
    //    Shield,
    //    Armor,
    //    Hull
    //}

    //public class DamageHelper
    //{
    //    public static float _damageBonus = 2;
    //    public static float _damageResist = 0.5f;

    //    public static void ApplyDamage(HealthData healthData, float kineticDamage, float energyDamage, DamageBonusType damageBonus, out DestroyedPartType destroyedPartType)
    //    {
    //        if (healthData.CurrentHealthPoints.ShieldPoints > 0)
    //        {
    //            energyDamage = damageBonus.HasFlag(DamageBonusType.ShieldBonus) ? energyDamage * _damageBonus : energyDamage;
    //            energyDamage = healthData.ResistanceType.HasFlag( ResistanceType.EnergyResistance) ? energyDamage * _damageResist : energyDamage;

    //            healthData.CurrentHealthPoints.ShieldPoints -= energyDamage;
    //            destroyedPartType = healthData.CurrentHealthPoints.ShieldPoints > 0 ? DestroyedPartType.None : DestroyedPartType.Shield;
    //            return;
    //        }

    //        if (healthData.CurrentHealthPoints.ArmorPoints > 0)
    //        {
    //            kineticDamage = damageBonus.HasFlag(DamageBonusType.ArmorBonus) ? kineticDamage * _damageBonus : kineticDamage;
    //            kineticDamage = healthData.ResistanceType.HasFlag(ResistanceType.KineticResistance) ? kineticDamage * _damageResist : kineticDamage;

    //            healthData.CurrentHealthPoints.ArmorPoints -= kineticDamage;
    //            destroyedPartType = healthData.CurrentHealthPoints.ArmorPoints > 0 ? DestroyedPartType.None : DestroyedPartType.Armor;
    //            return;
    //        }

    //        energyDamage = healthData.ResistanceType.HasFlag(ResistanceType.EnergyResistance) ? energyDamage * _damageResist : energyDamage;
    //        kineticDamage = healthData.ResistanceType.HasFlag(ResistanceType.KineticResistance) ? kineticDamage * _damageResist : kineticDamage;

    //        float totalDamage = energyDamage + kineticDamage;
    //        totalDamage = damageBonus.HasFlag(DamageBonusType.HullBonus) ? totalDamage * _damageBonus : totalDamage;

    //        healthData.CurrentHealthPoints.HullPoints -= totalDamage;
    //        destroyedPartType = healthData.CurrentHealthPoints.HullPoints > 0 ? DestroyedPartType.None : DestroyedPartType.Hull;
    //        return;
    //    }
    //}
}

