using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("Скорость пули")]
    [SerializeField] private float _speed = 2f;

    [Tooltip("Объекты на этом слое игнорируются")]
    [SerializeField] private LayerMask _ignoreThisLayer;

    private float _lifetime;

    private void OnEnable()
    {
        _lifetime = CalculateLifetime();
        StartCoroutine(LifeRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(LifeRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_ignoreThisLayer & 1 << collision.gameObject.layer) == (1 << collision.gameObject.layer))
        {
            return;
        }

        if (collision.TryGetComponent(out UFO ufo))
        {
            ufo.OnBulletHit();
        }
        else if (collision.TryGetComponent(out SpaceShip spaceShip))
        {
            spaceShip.Hit();
        }
        else if (collision.TryGetComponent(out Asteroid asteroid))
        {
            asteroid.OnBulletHit();
        }
        gameObject.SetActive(false);
    }

    private float CalculateLifetime()
    {
        Vector2 point1 = Camera.main.ScreenToWorldPoint(Vector2.zero);
        Vector2 point2 = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width,0f));
        float distance = Vector2.Distance(point1, point2);
        float lifetime = distance / _speed;
        return lifetime;
    }

    private IEnumerator LifeRoutine()
    {
        float currentLifetime = 0;

        while (currentLifetime < _lifetime)
        {
            currentLifetime += Time.fixedDeltaTime;
            Move();
            yield return new WaitForFixedUpdate();
        }
        Deactivate();
    }

    private void Move()
    {
        transform.position += transform.up * _speed * Time.fixedDeltaTime;
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
