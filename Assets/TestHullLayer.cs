using DefenseLayers;
using UnityEngine;

public class TestHullLayer : MonoBehaviour
{
    [SerializeField] float hullPoints = 1000;
    [SerializeField] float armorPoints = 100;
    private void Start()
    {
        TryGetComponent(out HullDefenseLayer component);
        component.CurrentHullPoints = hullPoints;
        component.CurrentArmorPoints = armorPoints;
    }
}
