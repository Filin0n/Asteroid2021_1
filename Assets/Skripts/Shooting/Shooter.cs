using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    [Tooltip("Префаб снаряда")]
    [SerializeField] private Bullet _bulletPrefab;

    [Header ("Object pool")]
    [Tooltip("Количестово объектов в пуле")]
    [SerializeField] private int _poolCount = 5;

    [Tooltip("Пул может увеличится если будет такая потребность")]
    [SerializeField] private bool _autoExpand = true;

    private PoolMono<Bullet> _pool;

    public static event UnityAction ShootEvent;

    private void Awake()
    {
        _pool = new PoolMono<Bullet>(_bulletPrefab,_poolCount);
        _pool.autoExpand = _autoExpand;
    }

    public void Shoot()
    {
        ShootEvent();
        Bullet bullet = _pool.GetFreeElement();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
    }

    public void ShootTo(Vector2 direction)
    {
        ShootEvent();
        float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;

        Bullet bullet = _pool.GetFreeElement();
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
