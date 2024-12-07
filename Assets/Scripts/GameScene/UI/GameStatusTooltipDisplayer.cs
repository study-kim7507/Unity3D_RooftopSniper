using System.Collections;
using TMPro;
using UnityEngine;

public class GameStatusTooltipDisplayer : MonoBehaviour
{
    public TextMeshProUGUI textPrefab;
    public float fadeDuration = 2.0f;
    public float displayDuration = 3.0f;

    public void DisplayTooltip(string message)
    {
        TextMeshProUGUI newText = Instantiate(textPrefab, transform);
        newText.text = message;

        RectTransform rectTransform = newText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition -= new Vector2(0, 50 * transform.childCount);

        StartCoroutine(FadeOutText(newText));
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(displayDuration);

        // ���̵� �ƿ� ����
        float elapsedTime = 0f;
        Color originalColor = text.color;


        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(originalColor.a, 0, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ؽ�Ʈ ����
        Destroy(text.gameObject);
    }
}
