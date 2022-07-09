using UnityEngine;

public class UFOSpawner : MonoBehaviour
{
    [SerializeField] private UFO _ufo;

    [Tooltip("Минимальное расстояние до горизонтальных границ (в процентах)")]
    [SerializeField] private float _minDistanceToScreenBorders = 20f;

    [Header("Spawn Time")]
    [SerializeField] private float _minTimeToSpawn;
    [SerializeField] private float _maxTimeToSpawn;

    private void Awake()
    {
        UFO.DiedEvent += SpawnUfo;
        SpawnUfo();
    }

    private void OnDestroy()
    {
        UFO.DiedEvent -= SpawnUfo;
    }

    private void SpawnUfo()
    {
        float timeToSpawn = Random.Range(_minTimeToSpawn, _maxTimeToSpawn);
        Invoke("Spawn", timeToSpawn);
    }

    private void Spawn()
    {
        Vector2 screenSise = new Vector2(Screen.width, Screen.height);
        bool spawnToLeft = (Random.value > 0.5f);

        float screenProcent = Screen.height / 100 * _minDistanceToScreenBorders;

        float verticalPosition = Random.Range(screenProcent, Screen.height - screenProcent);

        Vector2 spawnPosition = Vector2.zero;

        if (spawnToLeft)
        {
            spawnPosition = new Vector2(0, verticalPosition);
        }
        else
        {
            spawnPosition = new Vector2(Screen.width, verticalPosition);
        }

        _ufo.gameObject.SetActive(true);

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(spawnPosition);

        _ufo.transform.position = worldPosition;
        _ufo.leftToRight = spawnToLeft;
    }
}
