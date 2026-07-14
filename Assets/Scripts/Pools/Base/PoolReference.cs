using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PoolReference", menuName = "Scriptable Objects/PoolReference")]
public class PoolReference : ScriptableObject
{
    [field: SerializeField] public GameObject Prefab { get; private set; }

    [SerializeField, HideInInspector] // критически важно оставить атрибуты иначе у всех будет значение по умолчанию
    private uint _id;
    public uint Id => _id;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_id != 0)
            return;

        string path = AssetDatabase.GetAssetPath(this);

        if (string.IsNullOrEmpty(path))
            return;

        string guid = AssetDatabase.AssetPathToGUID(path);

        _id = ComputeStableHash(guid);

        EditorUtility.SetDirty(this);
    }

    private static uint ComputeStableHash(string value)
    {
        unchecked
        {
            uint hash = 2166136261;

            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= 16777619;
            }

            return hash;
        }
    }
#endif
}
