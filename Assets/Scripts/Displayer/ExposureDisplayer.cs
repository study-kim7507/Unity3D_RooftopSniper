using System.Collections;
using TMPro;
using UnityEngine;

public class ExposureDisplayer : MonoBehaviour
{
    public TMP_Text ExposureCountText;

    private CanvasGroup canvasGroup;
    private int CurrentNumberOfDetections;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        CurrentNumberOfDetections = GameManager.Instance.NumberOfDetections;
        ExposureCountText.text = CurrentNumberOfDetections.ToString();
    }

    private void Update()
    {
        if (CurrentNumberOfDetections != GameManager.Instance.NumberOfDetections)
        {
            CurrentNumberOfDetections = GameManager.Instance.NumberOfDetections;
            StartCoroutine(FadeBlinkDisplayer());
        }
        
    }
    private IEnumerator FadeBlinkDisplayer()
    {
        // �ؽ�Ʈ ������Ʈ
        ExposureCountText.text = CurrentNumberOfDetections.ToString();

        // ������ ȿ��
        for (int i = 0; i < 3; i++) // 3�� �����̱�
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
            yield return null; // ���� �����ӱ��� ���
        }
        canvasGroup.alpha = 0; // ���� ���� �� ����
    }

    private IEnumerator FadeIn(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, t / duration);
            yield return null; // ���� �����ӱ��� ���
        }
        canvasGroup.alpha = 1; // ���� ���� �� ����
    }

}
