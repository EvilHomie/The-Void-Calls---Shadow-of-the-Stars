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

    private readonly List<(Vector2 pos, float radius)> _reservedPositions = new();
    private readonly List<GameObject> _spawnedGO = new();

    private GameObject _newClaster;

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
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _spawnedGO.Clear();
        _reservedPositions.Clear();
        Destroy(_newClaster);

        _newClaster = new GameObject("NewClaster");
        _newClaster.transform.position = transform.position;

        int spawned = 0;
        int attempts = 0;

        while (spawned < _asteroidCount && attempts < _maxAttempts)
        {
            attempts++;

            var asteroidPrefab = GetRandomAsteroid();
            Vector2 spawmPos = Random.insideUnitCircle * _fieldRadius;
            spawmPos += (Vector2)transform.position;
            float spawnDiametr = Random.Range(_minScale, _maxScale);

            bool overlap = false;

            foreach (var (reservedPos, reservedDiametr) in _reservedPositions)
            {
                if (Vector2.Distance(spawmPos, reservedPos) < (reservedDiametr + spawnDiametr) / 2)
                {
                    overlap = true;
                    break;
                }
            }

            if (overlap)
                continue;

            var newRotation = Quaternion.Euler(0, 0, Random.Range(-180, 180));
            var asteroid = Instantiate(asteroidPrefab, spawmPos, newRotation, _newClaster.transform);
            asteroid.transform.localScale = Constants.Vector3One * spawnDiametr;
            _reservedPositions.Add((spawmPos, spawnDiametr));
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
        var asteroidsCluster = _newClaster.AddComponent<AsteroidsCluster>();
        asteroidsCluster.Asteroids = new();
        Debug.Log(asteroidsCluster.Asteroids.Count);

        foreach (var asteroid in _spawnedGO)
        {
            var ast = asteroid.AddComponent<ClusterAsteroid>();
            asteroidsCluster.Asteroids.Add(ast);
        }

        foreach (var asteroid in asteroidsCluster.Asteroids)
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
            asteroid.CurrentDamageProfile = newDamageProfile;
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