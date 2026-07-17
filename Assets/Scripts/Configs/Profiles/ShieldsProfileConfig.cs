using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Configs
{
    [CreateAssetMenu(fileName = "ShieldProfileConfig", menuName = "Scriptable Objects/Profile/ShieldProfileConfig")]
    public class ShieldsProfileConfig : ScriptableObject
    {
        [field: SerializeField] public ShieldsRolePreferences[] Profiles { get; private set; }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика и для PropertyDrawer
        {
            for (int i = 0; i < Profiles.Length; i++)
            {
                var name = Profiles[i].Role.ToString();
                Profiles[i].Name = name;

                for (int j = 0; j < Profiles[i].Preference.Length; j++)
                {
                    Profiles[i].Preference[j].Name = Profiles[i].Preference[j].Shield.ToString();
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public struct ShieldsRolePreferences
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ShipRoleProfile Role { get; private set; }
        [field: SerializeField] public ShieldPreference[] Preference { get; private set; }
    }

    [Serializable]
    public struct ShieldPreference
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ShieldId Shield { get; private set; }
        [field: SerializeField, Min(0)] public int Weight { get; private set; }
    }
}