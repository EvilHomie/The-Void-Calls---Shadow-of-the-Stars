using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Configs
{
    [CreateAssetMenu(fileName = "MainEngineProfileConfig", menuName = "Scriptable Objects/Profile/MainEngineProfileConfig")]
    public class MainEnginesProfileConfig : ScriptableObject
    {
        [field: SerializeField] public MainEnginesRolePreferences[] Profiles { get; private set; }

#if UNITY_EDITOR
        private void OnValidate() // просто для красоты чтобы в коллекции вместо номера элемента была конкретика и для PropertyDrawer
        {
            for (int i = 0; i < Profiles.Length; i++)
            {
                var name = Profiles[i].Role.ToString();
                Profiles[i].Name = name;

                for (int j = 0; j < Profiles[i].Preference.Length; j++)
                {
                    Profiles[i].Preference[j].Name = Profiles[i].Preference[j].MainEngine.ToString();
                }
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }

    [Serializable]
    public struct MainEnginesRolePreferences
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public ShipRoleProfile Role { get; private set; }
        [field: SerializeField] public MainEnginePreference[] Preference { get; private set; }
    }

    [Serializable]
    public struct MainEnginePreference
    {
        [HideInInspector] public string Name;
        [field: SerializeField] public MainEngineId MainEngine { get; private set; }
        [field: SerializeField, Min(0)] public int Weight { get; private set; }
    }
}