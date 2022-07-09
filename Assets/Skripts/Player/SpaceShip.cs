using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpaceShip : MonoBehaviour
{
    [Tooltip("Скорость перемещения")]
    [SerializeField] private float _moveSpeed = 5f;

    [Tooltip("Скорость поворота корабля")]
    [SerializeField] private float _rotationSpeed = 3f;

    [Range(0,5)][Tooltip("Скорость разгона корабля")]
    [SerializeField] private float _acceleratioSnpeed = 0.5f;

    [Tooltip("Врнемя неуязвимости после спавна")]
    [SerializeField] private float _invulnerableTime = 2f;

    [Tooltip("Расстояние между курсором и кораблем в пределах которого корабль не риагирует на курсор")]
    [SerializeField] private float _minDistanceToCursor = 0.3f;

    [Header("Shooting")]
    [Tooltip("Частота выстрелов")]
    [SerializeField] private float _shotsFrequency = 0.33f;

    [Tooltip("Спавнер снарядов")]
    [SerializeField] private Shooter _shooter;
    
    private bool _canShot = true;
    private bool _isInvulnerable = false;
    private Vector2 _direction;

    public bool keyboardOnly { get; set; }

    public static event UnityAction OnHitEvent;
    public static event UnityAction OnMoveEvent;

    private void Awake()
    {   
        if (_shooter == null)
        {
            _shooter = GetComponent<Shooter>();
        }
        StartCoroutine(Invulnerable());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;

        if (_shooter != null && ((!keyboardOnly && Input.GetMouseButtonDown(0)) || (keyboardOnly && Input.GetKeyDown(KeyCode.Space ))))
        {
            if (!_canShot) return;
            StartCoroutine(ShotTimer());

            _shooter.Shoot();
        }
        Move();
        Rotation();
    }

    private IEnumerator ShotTimer()
    {
        _canShot = false;
        yield return new WaitForSeconds(_shotsFrequency);
        _canShot = true;
    }

    private void Move()
    {
        if ((Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.UpArrow)) || (!keyboardOnly && Input.GetMouseButton(1))))
        {
            OnMoveEvent();
            _direction = Vector2.Lerp(_direction, transform.up, _acceleratioSnpeed * Time.deltaTime);
        }
        transform.position += (Vector3)_direction * _moveSpeed * Time.deltaTime;
    }

    private void Rotation()
    {
        Vector2 rotateTo = Vector2.zero;

        if (keyboardOnly)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                rotateTo = -transform.right;

                float angle = Mathf.Atan2(-rotateTo.x, rotateTo.y) * Mathf.Rad2Deg;
                Rotate(angle);
            }
            else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                rotateTo = transform.right;

                float angle = Mathf.Atan2(-rotateTo.x, rotateTo.y) * Mathf.Rad2Deg;
                Rotate(angle);
            }
        }
        else
        {
            rotateTo = DirectionToCursor();
            float angle = Mathf.Atan2(-rotateTo.x, rotateTo.y) * Mathf.Rad2Deg;
            Rotate(angle);
        }
    }

    private void Rotate(float angle)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), _rotationSpeed * Time.deltaTime);
    }

    private Vector2 DirectionToCursor()
    {
        Vector3 cameraWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cameraWorldPosition.z = transform.position.z;

        if (Vector3.Distance(cameraWorldPosition,transform.position) < _minDistanceToCursor)
        {
            return _direction;
        }
        return cameraWorldPosition - transform.position;
    }

    private IEnumerator Invulnerable()
    {
        _isInvulnerable = true;
        StartCoroutine(Flicker());

        yield return new WaitForSeconds(_invulnerableTime);

        _isInvulnerable = false;
    }

    private IEnumerator Flicker()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        while (_isInvulnerable)
        {
            yield return new WaitForSeconds(0.25f);
            renderer.enabled = !renderer.enabled;
        }
        renderer.enabled = true;
        yield return 0;
    }

    public void Hit()
    {
        if (_isInvulnerable) return;

        transform.position = Vector3.zero;
        _direction = Vector2.zero;
        StartCoroutine(Invulnerable());
        OnHitEvent();
    }
}