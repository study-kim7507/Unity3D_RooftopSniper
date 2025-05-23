using System.Collections;
using TMPro;
using UnityEngine;

public class TargetKillCountDisplayer : MonoBehaviour
{
    public TMP_Text TargetKillCountText;

    private CanvasGroup canvasGroup;
    private int CurrentTargetKillCount;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        CurrentTargetKillCount = GameManager.Instance.TargetKillCount;
        TargetKillCountText.text = CurrentTargetKillCount.ToString();

    }

    private void Update()
    {
        if (CurrentTargetKillCount != GameManager.Instance.TargetKillCount)
        {
            CurrentTargetKillCount = GameManager.Instance.TargetKillCount;
            StartCoroutine(FadeBlinkDisplayer());
        }
    }

    private IEnumerator FadeBlinkDisplayer()
    {
        // 텍스트 업데이트
        TargetKillCountText.text = CurrentTargetKillCount.ToString();

        // 깜빡임 효과
        for (int i = 0; i < 3; i++) // 3번 깜빡이기
        {
            yield return StartCoroutine(FadeOut(0.5f));
            yield return StartCoroutine(FadeIn(0.5f));
        }
    }

    private IEnumerator FadeOut(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / duration);
            yield return null; // 다음 프레임까지 대기
        }
        canvasGroup.alpha = 0; // 최종 알파 값 설정
    }

    private IEnumerator FadeIn(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t / duration);
            yield return null; // 다음 프레임까지 대기
        }
        canvasGroup.alpha = 1; // 최종 알파 값 설정
    }
}
