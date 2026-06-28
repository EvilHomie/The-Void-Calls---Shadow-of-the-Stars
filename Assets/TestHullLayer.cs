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
        //shieldDefenseLayer.Init(ShieldPoints, 5);
    }

    private void Update()
    {
        transform.root.Rotate(new Vector3(0, 0, 10 * Time.deltaTime));
    }
}
