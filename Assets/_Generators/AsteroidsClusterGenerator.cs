using Asteroids;
using Helpers;
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
    private Vector3 Vector3One;

    private void Awake()
    {
        Vector3One = Vector3.one;
    }

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

        _newClaster = new GameObject("AsteroidsClaster_NEW");
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
            asteroid.transform.localScale = Vector3One * spawnDiametr;
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

        foreach (var spawnedAsteroid in _spawnedGO)
        {
            if (!spawnedAsteroid.TryGetComponent(out Asteroid asteroidComponent))
            {
                throw new Exception("Asteroid Component NotFound");
            }

            var clusterAsteroid = spawnedAsteroid.AddComponent<ClusterAsteroid>();
            asteroidsCluster.Asteroids.Add(clusterAsteroid);

            clusterAsteroid.RB = asteroidComponent.Rigidbody;
            //clusterAsteroid.HealthData = asteroidComponent.HealthData;
            clusterAsteroid.AsteroidType = asteroidComponent.AsteroidType;
            Destroy(asteroidComponent);
        }

        foreach (var clusterAsteroid in asteroidsCluster.Asteroids)
        {
            clusterAsteroid.RB.angularDamping = 0;
            clusterAsteroid.RB.linearDamping = 0.2f;
            clusterAsteroid.RB.mass = _asteroidBaseMass * Mathf.Pow(clusterAsteroid.transform.localScale.x, 2f);
            //clusterAsteroid.HealthData.HullPoints = clusterAsteroid.RB.mass;

            //FlagsHelper.RemoveFlag(ref clusterAsteroid.AsteroidType, AsteroidType.Drifting);
            //FlagsHelper.AddFlag(ref clusterAsteroid.AsteroidType, AsteroidType.Cluster);                        

            //clusterAsteroid.HealthData.ResistanceType = ResistanceType.None;
            //FlagsHelper.AddFlag(ref clusterAsteroid.HealthData.ResistanceType, ResistanceType.None);
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