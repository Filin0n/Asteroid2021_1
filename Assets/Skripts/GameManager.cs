using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] public int _playerLivesCount = 5;

    [Header("Interface")]
    [SerializeField] private Transform _helthBar;
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private TMP_Text _scoreText;

    [Header("Main menu")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _continueButton;
    [SerializeField] private TMP_Text _changeInputButtonText;

    private SpaceShip _spaceShip;
    private int _currentScore;
    private int _currentLives;

    private static bool _paused = true;
    private static bool _itsNewGame = true;
    private static bool _keyboardOnly = false;

    private void Awake()
    {
        _spaceShip = FindObjectOfType<SpaceShip>();
        SpaceShip.OnHitEvent += OnHitPlayer;

        ChangeInput(_keyboardOnly);
        _continueButton.SetActive(!_itsNewGame);
        Pause(_paused);
        InstantiateHalthPrefabs();
    }

    private void OnDestroy()
    {
        SpaceShip.OnHitEvent -= OnHitPlayer;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(true);
        }
    }

    private void ChangeInput(bool input)
    {
        if (input == true)
        {
            _changeInputButtonText.text = "Keyboard only";
        }
        else
        {
            _changeInputButtonText.text = "Keyboard + mouse";
        }
        _spaceShip.keyboardOnly = input;
        _keyboardOnly = input;
    }

    private void InstantiateHalthPrefabs()
    {
        _currentLives = _playerLivesCount;

        for (int i = 0; i < _playerLivesCount; i++)
        {
            Instantiate(_heartPrefab, _helthBar);
        }
    }

    public void Pause(bool paused)
    {
        if (paused)
        {
            Time.timeScale = 0;
            _mainMenu.SetActive(true);
            _paused = true;
        }
        else
        {
            Time.timeScale = 1;
            _mainMenu.SetActive(false);
            _paused = false;
        }
    }

    public void OnClick_Continue()
    {
        if(!_itsNewGame) Pause(false);
    }

    public void OnClick_NewGame()
    {
        if (_itsNewGame)
        {
            _itsNewGame = false;
            _continueButton.SetActive(!_itsNewGame);
            Pause(false);
        }
        else
        {
            _paused = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void OnClick_ChangeInput()
    {
        ChangeInput(!_keyboardOnly);
    }

    public void OnClick_Escape()
    {
        Application.Quit();
    }

    public void UpdateNumberOfPoints(int points)
    {
        _currentScore += points;
        _scoreText.text = "Current score: " + _currentScore.ToString();
    }

    public void OnHitPlayer()
    {
        _currentLives --;

        for (int i = 0; i < _playerLivesCount; i++)
        {
            if (_currentLives > i)
            {
                _helthBar.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                _helthBar.GetChild(i).gameObject.SetActive(false);
            }
        }
        if (_currentLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _continueButton.SetActive(false);
        Pause(true);
        _spaceShip.gameObject.SetActive(false);
    }
}
