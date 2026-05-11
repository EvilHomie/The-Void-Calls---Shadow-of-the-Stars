using Helpers;
using Ships;
using UnityEngine;

public class TestShip : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var instance = GetComponent<ShipInstance>();
        InitHelper.InitShip(instance);
    }
}
