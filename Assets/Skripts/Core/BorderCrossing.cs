using UnityEngine;

public class BorderCrossing : MonoBehaviour
{
    private Vector2 _screenObjectPosition;
    private Vector2 _screenSize;

    private void FixedUpdate()
    {
        _screenSize = new Vector2(Screen.width, Screen.height);
        _screenObjectPosition = Camera.main.WorldToScreenPoint(transform.position);

        CalculateNewPosition();
    }

    private void CalculateNewPosition()
    {
        Vector2 newPosition;

        if (_screenObjectPosition.x > _screenSize.x)
        {
            newPosition = Camera.main.ScreenToWorldPoint(new Vector2(0, _screenObjectPosition.y));
            transform.position = newPosition;
        }
        else if (_screenObjectPosition.x < 0)
        {
            newPosition = Camera.main.ScreenToWorldPoint(new Vector2(_screenSize.x, _screenObjectPosition.y));
            transform.position = newPosition;
        }
        else if (_screenObjectPosition.y > _screenSize.y)
        {
            newPosition = Camera.main.ScreenToWorldPoint(new Vector2(_screenObjectPosition.x, 0));
            transform.position = newPosition;
        }
        else if (_screenObjectPosition.y < 0)
        {
            newPosition = Camera.main.ScreenToWorldPoint(new Vector2(_screenObjectPosition.x, _screenSize.y));
            transform.position = newPosition;
        }
    }
}
