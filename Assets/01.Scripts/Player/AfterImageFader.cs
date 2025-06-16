using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImageFader : MonoBehaviour
{
    [Tooltip("잔상이 남아 사라지는데 걸리는 시간(초)")]
    public float fadeDuration = 0.3f;

    private SpriteRenderer sr;
    private Color initialColor;
    private float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        initialColor = sr.color;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / fadeDuration;

        // 알파만 1→0 로 보간
        Color c = initialColor;
        c.a = Mathf.Lerp(initialColor.a, 0f, t);
        sr.color = c;

        if (t >= 1f)
            Destroy(gameObject);
    }
}