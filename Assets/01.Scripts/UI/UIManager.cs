using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ingleton instance
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("UIManager");
                    DontDestroyOnLoad(obj);
                    _instance = obj.AddComponent<UIManager>();
                }
            }
            return _instance;
        }
    }

    Coroutine Fade;
    public Image fadeImage;

    public ConditionUI condition;

    private void Awake()
    {
        if (_instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Fade Fuction
    private IEnumerator FadeOut(float duration = 1f)
    {
        float elapsedTime = 0f;
        fadeImage.enabled = true;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;
    }

    private IEnumerator FadeIn(float duration = 1f)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.enabled = false;
    }

    public IEnumerator FadeFlash(float fadeOutDuration = 1f, float waitDuration = 3f, float fadeInDuration = 1f)
    {
        StartCoroutine(FadeOut(fadeOutDuration));
        yield return new WaitForSeconds(waitDuration);
        yield return StartCoroutine(FadeIn(fadeInDuration));
        yield return new WaitForSeconds(fadeInDuration);
        Fade = null;
    }

    public void FadeFlashTest(float fadeOutDuration = 1f, float waitDuration = 3f, float fadeInDuration = 1f)
    {
        if (Fade != null)
        {
            Debug.Log("이미 코루틴이 진행 중이므로 아무것도 안하겠습니다 ㅅㄱ");
            return;
        }
        Fade = StartCoroutine(FadeFlash(fadeOutDuration, waitDuration, fadeInDuration));
    }

    // Heart Management
}