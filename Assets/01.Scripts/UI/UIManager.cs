using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject ui;
    private Transform uiCanvas;

    private GameObject introUI;
    private GameObject playerUI;
    private GameObject optionUI;
    private GameObject gameOverUI;

    private void Awake()
    {
        if (_instance == this)
        {
            DontDestroyOnLoad(gameObject);
        }

        DontDestroyOnLoad(ui);
        uiCanvas = ui.transform;

        introUI = uiCanvas.Find("Intro").gameObject;
        playerUI = uiCanvas.Find("Player").gameObject;
        optionUI = uiCanvas.Find("Option").gameObject;
        gameOverUI = uiCanvas.Find("GameOverPanel").gameObject;

        Init();


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
            elapsedTime += Time.unscaledDeltaTime;
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
            elapsedTime += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.enabled = false;
    }

    public void FadeInStart(float duration = 1f)
    {
        if (Fade == null)
            Fade = StartCoroutine(FadeIn(1f));
        else
            return;
        Fade = null;
    }

    public void FadeOutStart(float duration = 1f)
    {
        if (Fade == null)
            Fade = StartCoroutine(FadeOut(duration));
        else
            return;
        Fade = null;
    }

    private IEnumerator FadeFlash(float fadeOutDuration, float waitDuration, float fadeInDuration)
    {
        StartCoroutine(FadeOut(fadeOutDuration));
        yield return new WaitForSeconds(waitDuration);
        yield return StartCoroutine(FadeIn(fadeInDuration));
        yield return new WaitForSeconds(fadeInDuration);
        Fade = null;
    }

    public void FadeFlashStart(float fadeOutDuration = 1f, float waitDuration = 3f, float fadeInDuration = 1f)
    {
        if (Fade == null)
        {
            Fade = StartCoroutine(FadeFlash(fadeOutDuration, waitDuration, fadeInDuration));
        }
        else
            return;
        Fade = null;
    }

    // UI Initialization
    private void Init()
    {
        ui.SetActive(true);
        uiCanvas.Find("Intro").gameObject.SetActive(true);
        uiCanvas.Find("Player").gameObject.SetActive(false);
        uiCanvas.Find("Option").gameObject.SetActive(false);
        uiCanvas.Find("GameOverPanel").gameObject.SetActive(false);

        GameObject FadeImageObj = uiCanvas.Find("FadeImage").gameObject;
        fadeImage = FadeImageObj.GetComponent<Image>();
        FadeImageObj.SetActive(true);
        FadeImageObj.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        FadeImageObj.GetComponent<Image>().enabled = false;

    }

    // Scene Change Initialization
    public void SceneInit(string sceneName)
    {
        switch (sceneName)
        {
            case "MainScene":
                uiCanvas.Find("Intro").gameObject.SetActive(false);
                uiCanvas.Find("Player").gameObject.SetActive(true);
                uiCanvas.Find("Option").gameObject.SetActive(false);
                uiCanvas.Find("GameOverPanel").gameObject.SetActive(false);
                condition.GenerateHearts();
                FadeInStart(1f);
                GameManager.Instance.ChangeScene(sceneName);
                break;
            case "IntroScene":
                uiCanvas.Find("Intro").gameObject.SetActive(true);
                uiCanvas.Find("Player").gameObject.SetActive(false);
                uiCanvas.Find("Option").gameObject.SetActive(false);
                uiCanvas.Find("GameOverPanel").gameObject.SetActive(false);
                FadeInStart(1f);
                GameManager.Instance.ChangeScene(sceneName);
                break;
            default:
                break;
        }
    }

    public void GameOverUI()
    {
        gameOverUI.gameObject.SetActive(true);
    }
}