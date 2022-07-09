using System.Collections;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Asteroid _bigAsteroidPrefab;
    [SerializeField] private Asteroid _averageAsteroidPrefab;
    [SerializeField] private Asteroid _smallAsteroidPrefab;

    [Header("Asteroids")]
    [Tooltip("Минимальная скорость астероида")]
    [SerializeField] private float _minSpeed = 1f;

    [Tooltip("Минимальная скорость астероида")]
    [SerializeField] private float _maxSpeed = 4f;

    [Tooltip("Количество обломков")]
    [SerializeField] private int _countOfFragment = 2;

    [Tooltip("Угол разлета меньших астероидов")]
    [SerializeField] float _angleOfExpansion = 45f;

    [Header("Wave settings")]
    [Tooltip("Количестово астероидов в начале игры")]
    [SerializeField] private int _startWaveSize = 2;

    [Tooltip("Увеличение количества астероидов с каждой волной")]
    [SerializeField] private int _increase = 1;

    [Tooltip("Задержка между волнами")]
    [SerializeField] private float _delayBetweenWawes = 2f;

    [Tooltip("Задержка между спавном астероидов")]
    [SerializeField] private float _delayBetweenSpawn = 1f;

    [Header("Object pool")]
    [Tooltip("Количестово объектов в пуле")]
    [SerializeField] private int _poolCount = 5;

    [Tooltip("Пул может увеличится если будет такая потребность")]
    [SerializeField] private bool _autoExpand = true;

    private PoolMono<Asteroid> _bigAsteroidPool;
    private PoolMono<Asteroid> _averageAsteroidPool;
    private PoolMono<Asteroid> _smallAsteroidPool;

    private int _numberOfAsteroidsInScene;
    private int _nextWaveSize;
    private bool _canStartSpawning = true;

    private void Awake()
    {
        _bigAsteroidPool = new PoolMono<Asteroid>(_bigAsteroidPrefab, _poolCount,transform);
        _bigAsteroidPool.autoExpand = _autoExpand;

        _averageAsteroidPool = new PoolMono<Asteroid>(_averageAsteroidPrefab, _poolCount, transform);
        _averageAsteroidPool.autoExpand = _autoExpand;

        _smallAsteroidPool = new PoolMono<Asteroid>(_smallAsteroidPrefab, _poolCount, transform);
        _smallAsteroidPool.autoExpand = _autoExpand;
    }

    private void Start()
    {
        _nextWaveSize = _startWaveSize;
        SpawnWave();
    }

    private void SpawnWave()
    {
        if (_canStartSpawning)
        {
            StartCoroutine(SpawnAsteroidInWawe());
        }
    }

    public IEnumerator SpawnAsteroidInWawe()
    {
        _canStartSpawning = false;

        for (int i = 0; i < _nextWaveSize; i++)
        {
            yield return new WaitForSeconds(_delayBetweenSpawn);

            _numberOfAsteroidsInScene++;

            float angle = Random.Range(0, 360);
            float speed = Random.Range(_minSpeed, _maxSpeed);
            Asteroid asteroid = _bigAsteroidPool.GetFreeElement();

            asteroid.transform.position = SpawnPosition();
            asteroid.transform.rotation = Quaternion.Euler(0, 0, angle);
            asteroid.speed = speed;
        }
        _nextWaveSize += _increase;
        _canStartSpawning = true;
    }

    private Vector2 SpawnPosition()
    {
        Vector2 screenSise = new Vector2(Screen.width, Screen.height);
        bool spawnToHorizontal = (Random.value > 0.5f);

        Vector2 spawnPosition = Vector2.zero;

        if (spawnToHorizontal)
        {
            spawnPosition.y = Random.Range(0, screenSise.y);
        }
        else
        {
            spawnPosition.x = Random.Range(0, screenSise.x);
        }

        return Camera.main.ScreenToWorldPoint(spawnPosition);
    }

    public void AsteroidCrushing(TypeOfAsteroid asteroidType, Vector3 position, float parentAngle)
    {
        float speed = Random.Range(_minSpeed, _maxSpeed);
        Asteroid asteroid;

        for (int i = 0; i < _countOfFragment; i++)
        {
            if (asteroidType == TypeOfAsteroid.big)
            {
                asteroid = SpawnAverageAsteroid();
            }
            else
            {
                asteroid = SpawnSmallAsteroid();
            }
            asteroid.speed = speed;
            asteroid.transform.position = position;
            asteroid.transform.rotation = Quaternion.Euler(0, 0, parentAngle + Random.Range(-_angleOfExpansion, _angleOfExpansion));
        }
    }

    private Asteroid SpawnAverageAsteroid()
    {
        _numberOfAsteroidsInScene++;
        return _averageAsteroidPool.GetFreeElement();
    }

    private Asteroid SpawnSmallAsteroid()
    {
        _numberOfAsteroidsInScene++;
        return _smallAsteroidPool.GetFreeElement();
    }

    public void AsteroidDestroyed()
    {
        _numberOfAsteroidsInScene--;

        if (_numberOfAsteroidsInScene == 0)
        {
            Invoke("SpawnWave", _delayBetweenWawes);
        }
    }
}
