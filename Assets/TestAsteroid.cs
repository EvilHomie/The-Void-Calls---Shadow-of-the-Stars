using Asteroids;
using Helpers;
using UnityEngine;

public class TestAsteroid : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var instance = GetComponent<Asteroid>();
        instance.Init(default);
        InitHelper.InitAsteroid(instance);
        instance.Rigidbody.AddTorque(2, ForceMode2D.Impulse);
    }
}
