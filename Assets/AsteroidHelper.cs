using Asteroid;
using UnityEngine;

public class AsteroidHelper
{
    public static void Init(DriftingAsteroid asteroid)
    {
        asteroid.AsteroidBehaviourType =  AsteroidBehaviourType.DriftingAsteroid;
        asteroid.CachedGameObject = asteroid.gameObject;
        asteroid.CachedTransform = asteroid.transform;
        asteroid.RB = asteroid.GetComponent<Rigidbody2D>();
    }
    public static void ResetParams(DriftingAsteroid asteroid)
    {
        asteroid.CurrentDamageProfile = asteroid.DefaultDamageProfile;
    }




    
}
