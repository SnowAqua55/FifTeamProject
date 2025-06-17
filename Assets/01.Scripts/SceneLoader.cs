using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        DontDestroyOnLoad(GameManager.Instance.gameObject);
        DontDestroyOnLoad(UIManager.Instance.gameObject);
        SceneManager.LoadScene(sceneName);
    }
}