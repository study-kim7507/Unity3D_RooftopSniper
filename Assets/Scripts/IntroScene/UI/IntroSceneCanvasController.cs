using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneCanvasController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPopup;
    [SerializeField]
    private GameObject gameDescriptionPopup1;
    [SerializeField] 
    private GameObject gameDescriptionPopup2;  
    [SerializeField]
    private GameObject shortcutKeysDescriptionPopup;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ShowMainPopup();
        }
    }

    private void ShowMainPopup()
    {
        if (gameDescriptionPopup1.activeSelf)
            StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup1, mainPopup));
        else if (gameDescriptionPopup2.activeSelf)
            StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup2, mainPopup));
        else if (shortcutKeysDescriptionPopup.activeSelf)
            StartCoroutine(ChangePopupWithFadeInOut(shortcutKeysDescriptionPopup, mainPopup));
    }

    public void GameStartButtonPressed()
    {
        // �� ��ȯ
        SceneTransitioner.Instance.SceneChange("LevelSelectionScene");
    }

    public void ShowGameDescriptionButtonPressed()
    {
        // ���� ���� ����
        StartCoroutine(ChangePopupWithFadeInOut(mainPopup, gameDescriptionPopup1));
    }

    public void ShowShortcutKeysDescriptionButtonPressed()
    {
        // ����Ű ���� ����
        StartCoroutine(ChangePopupWithFadeInOut(mainPopup, shortcutKeysDescriptionPopup));
    }

    public void QuitButtonPressed()
    {
        // ���� ����
        Application.Quit(); 
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
            time += Time.deltaTime;
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
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void GameDescriptionPopupNextButton()
    {
        StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup1, gameDescriptionPopup2));
    }
    public void GameDescriptionPopupPrevButton()
    {
        StartCoroutine(ChangePopupWithFadeInOut(gameDescriptionPopup2, gameDescriptionPopup1));
    }
}
