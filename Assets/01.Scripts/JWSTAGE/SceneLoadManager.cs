using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoadManager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("JW_Stage"); // 추후 씬이름 교체
    }

    public void GameOver()
    {
        SceneManager.LoadScene("JW_IntroScene");
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
