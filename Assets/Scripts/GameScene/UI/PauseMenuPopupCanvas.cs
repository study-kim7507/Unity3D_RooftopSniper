using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuPopupCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPopup;
    [SerializeField]
    private GameObject gameDescriptionPopup1;
    [SerializeField]
    private GameObject gameDescriptionPopup2;
    [SerializeField]
    private GameObject shortcutKeysDescriptionPopup;

    [HideInInspector]
    public bool IsChildPopupActive = false;

    public void ShowGameDescriptionButtonPressed()
    {
        // ���� ���� ����
        StartCoroutine(ChangePopupWithFadeInOut(mainPopup, gameDescriptionPopup1));
        IsChildPopupActive = true;
    }

    public void ShowShortcutKeysDescriptionButtonPressed()
    {
        // ����Ű ����
        StartCoroutine(ChangePopupWithFadeInOut(mainPopup, shortcutKeysDescriptionPopup));
        IsChildPopupActive = true;
    }

    public void ShowMainPopup()
    {
        // mainPopup�� Ȱ��ȭ�ϰ� �ٸ� �˾��� ��Ȱ��ȭ
        if (gameDescriptionPopup1.activeSelf)
            StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup1, mainPopup));
        else if (gameDescriptionPopup2.activeSelf)
            StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup2, mainPopup));
        else if (shortcutKeysDescriptionPopup.activeSelf)
            StartCoroutine(ChangePopupWithFadeInOut(shortcutKeysDescriptionPopup, mainPopup));
        IsChildPopupActive = false;
    }

    public void QuitButtonPressed()
    {
        // ���� ������, LevelSelectionScene ���� �̵�
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale;
        SceneTransitioner.Instance.SceneChange("LevelSelectionScene");
    }

    public void GameDescriptionPopupNextButton()
    {
        StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup1, gameDescriptionPopup2));
    }
    public void GameDescriptionPopupPrevButton()
    {
        StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup2, gameDescriptionPopup1));
    }

    private IEnumerator ChangePopupWithFadeInOut(GameObject currentPopup, GameObject nextPopup)
    {
        CanvasGroup canvasGroup = currentPopup.GetComponent<CanvasGroup>();

        float duration = 1.0f;
        float startAlpha = 1.0f;
        float endAlpha = 0.0f;
        float time = 0.0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime * 1.5f;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha; // ���� ���� �� ����
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        currentPopup.SetActive(false);

        StartCoroutine(FadeIn(nextPopup));
    }

    private IEnumerator FadeIn(GameObject target)
    {
        target.SetActive(true);

        CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();

        float duration = 1.0f;
        float startAlpha = 0.0f;
        float endAlpha = 1.0f;
        float time = 0.0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime * 1.5f;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

}
