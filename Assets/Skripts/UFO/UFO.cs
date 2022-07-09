using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (Shooter))]
public class UFO : MonoBehaviour
{
    [Tooltip("Скорость перемещения")]
    [SerializeField] private float _speed = 2;

    [Tooltip("Награда за уничтожение")]
    [SerializeField] private int _reward;

    [Header ("Shooting")]
    [SerializeField] private float _minTimeRange = 2f;
    [SerializeField] private float _maxTimeRange = 5f;

    private GameManager _gameManager;
    private SpaceShip _playerShip;
    private Shooter _shooter;
    private bool _canShoot = false;

    [HideInInspector] public bool leftToRight;

    public static event UnityAction DiedEvent;

    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _shooter = GetComponent<Shooter>();
        _playerShip = FindObjectOfType<SpaceShip>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShoottingTimer());
    }

    private void Update()
    {
        Move();
        if (_canShoot)
        {
            _shooter.ShootTo(_playerShip.transform.position - transform.position);
            StartCoroutine(ShoottingTimer());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out SpaceShip spaceShip))
        {
            spaceShip.Hit();
            OnHit();
        }
    }

    private void Move()
    {
        Vector3 direction = Vector3.left;

        if (leftToRight)
        {
            direction = Vector3.right;
        }
        transform.position += direction * _speed * Time.deltaTime;
    }

    private IEnumerator ShoottingTimer()
    {
        float timeToShoot = Random.Range(_minTimeRange, _maxTimeRange);
        _canShoot = false;
       
        yield return new WaitForSeconds(timeToShoot);
        _canShoot = true;
    }

    public void OnBulletHit()
    {
        _gameManager.UpdateNumberOfPoints(_reward);
        OnHit();
    }

    public void OnHit()
    {
        DiedEvent();
        gameObject.SetActive(false);
    }
}
