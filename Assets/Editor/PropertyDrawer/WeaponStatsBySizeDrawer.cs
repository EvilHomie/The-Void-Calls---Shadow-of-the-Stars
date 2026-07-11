using Configs;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WeaponStatsBySize))]
public class WeaponStatsBySizeDrawer : PropertyDrawer
{
    string SpecificStats = "Specific Stats";
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var weaponType = property.FindPropertyRelative("WeaponType");

        var sizeType = property.FindPropertyRelative("<SizeType>k__BackingField");
        var aimStats = property.FindPropertyRelative("<AimStats>k__BackingField");
        var damageStats = property.FindPropertyRelative("<DamageStats>k__BackingField");
        var hullStats = property.FindPropertyRelative("<HullStats>k__BackingField");

        var constantBeamWeaponStats = property.FindPropertyRelative("<ConstantBeamWeaponStats>k__BackingField");
        var projectileWeaponStats = property.FindPropertyRelative("<ProjectileWeaponStats>k__BackingField");
        var missile = property.FindPropertyRelative("<MissileWeaponStats>k__BackingField");

        Rect rect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        //Draw(ref rect, weaponType);
        Draw(ref rect, sizeType);

        Draw(ref rect, aimStats, true);
        Draw(ref rect, damageStats, true);
        Draw(ref rect, hullStats, true);

        switch ((WeaponType)weaponType.enumValueIndex)
        {
            case WeaponType.BoltRepeater:
                Draw(ref rect, projectileWeaponStats, SpecificStats, true);
                break;

            //case WeaponType.MiningDrill:
            //    Draw(ref rect, constantBeamWeaponStats, SpecificStats, true);
            //    break;

                //case WeaponType.Missile:
                //    Draw(ref rect, missile, true);
                //    break;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0;

        var weaponType = property.FindPropertyRelative("WeaponType");

        height += GetHeight(property.FindPropertyRelative("<WeaponType>k__BackingField"));
        height += GetHeight(property.FindPropertyRelative("<SizeType>k__BackingField"));

        height += GetHeight(property.FindPropertyRelative("<AimStats>k__BackingField"), true);
        height += GetHeight(property.FindPropertyRelative("<DamageStats>k__BackingField"), true);
        height += GetHeight(property.FindPropertyRelative("<HullStats>k__BackingField"), true);

        switch ((WeaponType)weaponType.enumValueIndex)
        {
            //case WeaponType.MiningDrill:
            //    height += GetHeight(property.FindPropertyRelative("<ConstantBeamWeaponStats>k__BackingField"), true);
            //    break;

            case WeaponType.BoltRepeater:
                height += GetHeight(property.FindPropertyRelative("<ProjectileWeaponStats>k__BackingField"), true);
                break;

                //case WeaponType.Missile:
                //    height += GetHeight(property.FindPropertyRelative("<MissileWeaponStats>k__BackingField"), true);
                //    break;
        }

        return height;
    }

    private static void Draw(ref Rect rect, SerializedProperty property, bool includeChildren = false)
    {
        float h = EditorGUI.GetPropertyHeight(property, includeChildren);

        rect.height = h;

        EditorGUI.PropertyField(rect, property, includeChildren);

        rect.y += h + EditorGUIUtility.standardVerticalSpacing;
    }

    private static void Draw(ref Rect rect, SerializedProperty property, string label, bool includeChildren = false)
    {
        float h = EditorGUI.GetPropertyHeight(property, includeChildren);

        rect.height = h;

        EditorGUI.PropertyField(rect, property, new GUIContent(label), includeChildren);

        rect.y += h + EditorGUIUtility.standardVerticalSpacing;
    }

    private static float GetHeight(SerializedProperty property, bool includeChildren = false)
    {
        return EditorGUI.GetPropertyHeight(property, includeChildren)
             + EditorGUIUtility.standardVerticalSpacing;
    }
}