using Asteroid;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidsClusterGenerator : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] ContainedAsteroids _containedAsteroids;
    [SerializeField] float _fieldRadius;
    [SerializeField] int _maxAttempts;
    [SerializeField] int _asteroidCount;
    [Range(0.5f, 10f)] public float _minScale;
    [Range(0.5f, 10f)] public float _maxScale;

    private readonly List<(Vector2 pos, float radius)> _spawned = new();
    private readonly List<GameObject> _spawnedGO = new();

    private GameObject GetRandomAsteroid()
    {
        float total = 0f;

        foreach (var asteroid in _containedAsteroids.WeightedAsteroid)
        {
            total += asteroid.SpawnWeight;
        }

        float roll = Random.value * total;

        foreach (var asteroid in _containedAsteroids.WeightedAsteroid)
        {
            roll -= asteroid.SpawnWeight;
            if (roll <= 0f)
                return asteroid.Prefab;
        }

        return _containedAsteroids.WeightedAsteroid[0].Prefab;
    }

    private void OnEnable()
    {
        Generate();
    }

    private void Generate()
    {
        _spawned.Clear();

        foreach (var go in _spawnedGO)
        {
            Destroy(go);
        }
        _spawnedGO.Clear();

        int spawned = 0;
        int attempts = 0;

        while (spawned < _asteroidCount && attempts < _maxAttempts)
        {
            attempts++;

            var asteroidPrefab = GetRandomAsteroid();
            Vector2 pos = Random.insideUnitCircle * _fieldRadius;
            pos.x += transform.position.x;
            pos.y += transform.position.y;
            float newRadius = Random.Range(_minScale, _maxScale);

            bool overlap = false;
            foreach (var (p, r) in _spawned)
            {
                if (Vector2.Distance(pos, p) < (r + newRadius) / 2)
                {
                    overlap = true;
                    break;
                }
            }

            if (overlap)
                continue;

            var newRotation = Quaternion.Euler(0, 0, Random.Range(-180, 180));
            var asteroid = Instantiate(asteroidPrefab, pos, newRotation, transform);
            asteroid.transform.localScale = Constants.Vector3One * newRadius;
            _spawned.Add((pos, newRadius));
            _spawnedGO.Add(asteroid);
            spawned++;
        }

        FillData();
        Debug.Log($"Generated {spawned}/{_asteroidCount} asteroids in {attempts} attempts");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _fieldRadius);
    }



    private readonly float _asteroidBaseMass = 10; // масса в тоннах при scale = 1;

    public void FillData()
    {
        var AsteroidsFieldComponent = GetComponent<AsteroidsCluster>();
        AsteroidsFieldComponent.Asteroids.Clear();

        foreach (var asteroid in _spawnedGO)
        {
            var ast = asteroid.AddComponent<ClusterAsteroid>();
            AsteroidsFieldComponent.Asteroids.Add(ast);
        }

        foreach (var asteroid in AsteroidsFieldComponent.Asteroids)
        {
            asteroid.AsteroidBehaviourType = AsteroidBehaviourType.ClusterAsteroid;
            asteroid.RB = asteroid.GetComponent<Rigidbody2D>();
            asteroid.RB.angularDamping = 0;
            asteroid.RB.linearDamping = 0.2f;
            asteroid.RB.mass = _asteroidBaseMass * Mathf.Pow(asteroid.transform.localScale.x, 2f);

            DamageProfile newDamageProfile = new()
            {
                Structure = asteroid.RB.mass,
                ProfileType = DamageProfileType.Asteroid
            };

            asteroid.DefaultDamageProfile = newDamageProfile;
        }

    }
}

[Serializable]
public struct WeightedAsteroid
{
    [SerializeField] public GameObject Prefab;
    [Range(0f, 1f)] public float SpawnWeight;
}

[Serializable]
public struct ContainedAsteroids
{
    [field: SerializeField] public WeightedAsteroid[] WeightedAsteroid { get; private set; }
}