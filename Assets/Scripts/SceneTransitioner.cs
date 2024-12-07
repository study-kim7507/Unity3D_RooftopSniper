using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static SceneTransitioner Instance;

    [HideInInspector]
    public string NextSceneName;

    private Texture2D fadeTexture;
    private Color fadeColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        fadeTexture = new Texture2D(1, 1);
        fadeTexture.SetPixel(0, 0, Color.black);
        fadeTexture.Apply();

        fadeColor = new Color(0, 0, 0, 0);
    }

    private void OnGUI()
    {
        GUI.color = fadeColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }
    public void SceneChange(string nextSceneName)
    {
        NextSceneName = nextSceneName;
        StartCoroutine(FadeInCurrentSceneAndFadeOutLoadingScene());
    }

    private IEnumerator FadeInCurrentSceneAndFadeOutLoadingScene()
    {
        // 페이드 인
        yield return StartCoroutine(FadeInScene());

        SceneManager.LoadScene("LoadingScene");

        // 씬 로딩 후 페이드 아웃
        yield return StartCoroutine(FadeOutScene());
    }

    public IEnumerator FadeInLoadingSceneAndFadeOutNextScene()
    {
        yield return new WaitForSecondsRealtime(2.0f);

        AsyncOperation op = SceneManager.LoadSceneAsync(NextSceneName);
        op.allowSceneActivation = false;
      
        // 페이드 인
        yield return StartCoroutine(FadeInScene());

        // 씬 로딩 후 페이드 아웃
        StartCoroutine(FadeOutScene());

        op.allowSceneActivation = true;
    }

    private IEnumerator FadeInScene()
    {
        float duration = 0.75f;
        float startAlpha = 0.0f;
        float endAlpha = 1.0f;
        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }
        fadeColor.a = endAlpha; // 최종 알파 값 설정
    }

    private IEnumerator FadeOutScene()
    {
        float duration = 0.75f;
        float startAlpha = 1.0f;
        float endAlpha = 0.0f;
        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }
        fadeColor.a = endAlpha; // 최종 알파 값 설정
    }
}
