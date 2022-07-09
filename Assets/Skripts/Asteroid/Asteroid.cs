using UnityEngine;
using UnityEngine.Events;

public enum TypeOfAsteroid
{
    big = 0,
    average = 1,
    small = 2
}

public class Asteroid : MonoBehaviour
{
    [Tooltip("Размер астероида")]
    [SerializeField] private TypeOfAsteroid _asteroidType;

    [Tooltip("Награда за уничтожение")]
    [SerializeField] private int _reward;

    private GameManager _gameManager;
    private AsteroidSpawner _asteroidSpawner;
    private float _speed;

    public static event UnityAction<int> ExplosionEvent;

    public float speed
    {
        set { _speed = value; }
    }

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _asteroidSpawner = transform.parent.GetComponent<AsteroidSpawner>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        transform.position += transform.up * _speed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out UFO ufo))
        {
            ufo.OnHit();
        }
        else if (collision.TryGetComponent(out SpaceShip spaceShip))
        {
            spaceShip.Hit();
            Deactivate();
        }
    }

    public void OnBulletHit()
    {
        _gameManager.UpdateNumberOfPoints(_reward);

        if (_asteroidType == TypeOfAsteroid.small)
        {
            Deactivate();
            return;
        }

        float angle = transform.eulerAngles.z;
        _asteroidSpawner.AsteroidCrushing(_asteroidType, transform.position, angle);
        Deactivate();
    }

    private void Deactivate()
    {
        ExplosionEvent((int)_asteroidType);
        _asteroidSpawner.AsteroidDestroyed();
        gameObject.SetActive(false);
    }
}
