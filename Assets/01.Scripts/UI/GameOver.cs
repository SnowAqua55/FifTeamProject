using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverText;
    public GameObject gameOverMenu;

    public void ShowGameOver()
    {
        gameOverText.SetActive(true);
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }
}