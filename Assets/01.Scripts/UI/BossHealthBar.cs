using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class BossHealthBar : MonoBehaviour
{
    [Header("보스 체력 바 관련")] [Tooltip("체력 변동을 볼 bossBase 스크립트")]
    public BossBase boss;

    [Tooltip("채워지는 바 이미지")] public Image fillImage;

    [Header("등장/퇴장 딜레이")] [Tooltip("보스 등장 후 UI가 나타날 때까지 지연(초)")]
    public float showDelay = 2f;

    [Tooltip("보스 죽음 후 UI가 사라질 때까지 지연(초)")] public float hideDelay = 1f;

    [Header("페이드 속도")] [Tooltip("lerp 속도")]
    public float fadeSpeed = 3f;

    CanvasGroup cg;

    bool isShown = false;
    bool isHiding = false;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f; // 처음엔 투명
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void Start()
    {
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(showDelay);
        isShown = true; // 이제 페이드 인 시작
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(hideDelay);
        isHiding = true; // 이제 페이드 아웃 시작
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void Update()
    {
        if (!isShown || boss == null || fillImage == null) return;

        // 페이드 인/아웃 처리
        if (isShown && !isHiding)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 1f, Time.deltaTime * fadeSpeed);
        }
        else if (isHiding)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 0f, Time.deltaTime * fadeSpeed);
        }

        // 0~1 사이 비율 계산
        if (cg.alpha > 0.01f && boss != null && fillImage != null)
        {
            float target = boss.CurrentHP / boss.MaxHP;
            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, target, Time.deltaTime * 8f);

            // 보스 사망 감지 후 숨기기 시작
            if (!isHiding && boss.IsDead)
            {
                isHiding = true;
                StartCoroutine(HideAfterDelay());
            }
        }
    }
}