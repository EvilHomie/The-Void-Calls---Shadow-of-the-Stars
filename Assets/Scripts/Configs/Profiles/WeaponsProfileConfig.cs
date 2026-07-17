using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Configs
{
    [CreateAssetMenu(fileName = "WeaponProfileConfig", menuName = "Scriptable Objects/Profile/WeaponProfileConfig")]
    public class WeaponsProfileConfig : ScriptableObject
    {
        [field: SerializeField] public WeaponsRolePreferences[] Profiles { get; private set; }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика и для PropertyDrawer
        {
            for (int i = 0; i < Profiles.Length; i++)
            {
                var name = Profiles[i].Role.ToString();
                Profiles[i].Name = name;

                for (int j = 0; j < Profiles[i].Preference.Length; j++)
                {
                    Profiles[i].Preference[j].Name = Profiles[i].Preference[j].Weapon.ToString();
                }

                for (int j = 0; j < Profiles[i].Density.Length; j++)
                {
                    Profiles[i].Preference[j].Name = Profiles[i].Density[j].Density.ToString();
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public struct WeaponsRolePreferences
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ShipRoleProfile Role { get; private set; }
        [field: SerializeField] public WeaponPreference[] Preference { get; private set; }
        [field: SerializeField] public WeaponDensityPreference[] Density { get; private set; }
    }

    [Serializable]
    public struct WeaponPreference
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public WeaponType Weapon { get; private set; }
        [field: SerializeField, Min(0)] public int Weight { get; private set; }
    }

    [Serializable]
    public struct WeaponDensityPreference
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public WeaponDensityProfile Density { get; private set; }
        [field: SerializeField, Min(0)] public int Weight { get; private set; }
    }

    public enum WeaponDensityProfile
    {
        Quarter,
        Half,
        Full
    }

}