using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeDisplayer : MonoBehaviour
{
    public List<GameObject> LifeImages;

    private CanvasGroup canvasGroup;
    private int previousLifeCount;             // 이전 생명 수를 저장

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        previousLifeCount = GameManager.Instance.Life;

        for (int i = 0; i < LifeImages.Count; i++)
        {
            if (i < GameManager.Instance.Life)
                LifeImages[i].SetActive(true);
            else
                LifeImages[i].SetActive(false);
        }

    }

    private void Update()
    {
        UpdateLifeCount();
    }
    private void UpdateLifeCount()
    {
        int currentLifeCount = GameManager.Instance.Life;       // 현재 생명 수

        if (previousLifeCount != currentLifeCount)
        {
            previousLifeCount = currentLifeCount;
            StartCoroutine(FadeBlinkDisplayer());
        }

        
    }

    private IEnumerator FadeBlinkDisplayer()
    {
        for (int i = 0; i < LifeImages.Count; i++)
        {
            if (i < GameManager.Instance.Life)
                LifeImages[i].SetActive(true);
            else
                LifeImages[i].SetActive(false);
        }

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
