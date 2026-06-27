using DefenseLayers;
using UnityEngine;

public class TestHullLayer : MonoBehaviour
{
    [SerializeField] float hullPoints = 1000;
    [SerializeField] float armorPoints = 100;
    [SerializeField] float ShieldPoints = 100;
    [SerializeField] HullDefenseLayer hullDefenseLayer;
    [SerializeField] ShieldDefenseLayer shieldDefenseLayer;

    private void Start()
    {
        hullDefenseLayer.Init(hullPoints, armorPoints);
        shieldDefenseLayer.Init(ShieldPoints);
    }
}
