using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioClip _smallExplosion;
    [SerializeField] private AudioClip _mediumExplosion;
    [SerializeField] private AudioClip _largeExplosion;
    [SerializeField] private AudioClip _thrustSound;
    [SerializeField] private AudioClip _shootSound;

    private AudioSource _audioSource;

    private bool _canPlayThrust = true;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        Asteroid.ExplosionEvent += AsteroidExplosion;
        SpaceShip.OnMoveEvent += Thrust;
        SpaceShip.OnHitEvent += LargeExplosion;
        UFO.DiedEvent += LargeExplosion;
        Shooter.ShootEvent += Shoot;
    }

    private void OnDestroy()
    {
        Asteroid.ExplosionEvent -= AsteroidExplosion;
        SpaceShip.OnMoveEvent -= Thrust;
        SpaceShip.OnHitEvent -= LargeExplosion;
        UFO.DiedEvent -= LargeExplosion;
        Shooter.ShootEvent -= Shoot;
    }

    private void AsteroidExplosion(int _asteroidType)
    {
        if (_asteroidType == (int)TypeOfAsteroid.small)
        {
            SmallExplosion();
        }
        else if (_asteroidType == (int)TypeOfAsteroid.average)
        {
            MediumExplosion();
        }
        else if (_asteroidType == (int)TypeOfAsteroid.big)
        {
            LargeExplosion();
        }
    }

    private void Thrust()
    {
        if (_canPlayThrust)
        {
            _canPlayThrust = false;
            _audioSource.PlayOneShot(_thrustSound);
            Invoke("canPlayThrust", _thrustSound.length);
        }
    }

    private void canPlayThrust()
    {
        _canPlayThrust = true;
    }
    private void Shoot()
    {
        _audioSource.PlayOneShot(_shootSound);
    }

    private void SmallExplosion()
    {
        _audioSource.PlayOneShot(_smallExplosion);
    }

    private void MediumExplosion()
    {
        _audioSource.PlayOneShot(_mediumExplosion);
    }

    private void LargeExplosion()
    {
        _audioSource.PlayOneShot(_largeExplosion);
    }
}