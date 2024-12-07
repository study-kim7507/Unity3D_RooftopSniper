using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectionSceneCanvasController : MonoBehaviour
{
    [SerializeField]
    public Button GameStartButton;
    [SerializeField]
    public List<GameObject> Levels = new List<GameObject>();

    [HideInInspector]
    public GameObject PreviousSelectedLevel;
    [HideInInspector]
    public GameObject CurrentSelectedLevel;

    private void Start()
    {
        DeactivateGameStartButton();
    }

    private void Update()
    {
        if (CurrentSelectedLevel == null) DeactivateGameStartButton();
        else ActivateGameStartButton();
    }

    public void ChangeSelectedLevel(GameObject newSelectLevel)
    {
        PreviousSelectedLevel = CurrentSelectedLevel;
        CurrentSelectedLevel = newSelectLevel; 

        foreach (GameObject level in Levels)
        {
            if (level == CurrentSelectedLevel)
            {
                StartCoroutine(LevelPanelScaleAnimation(level, level.gameObject.transform.localScale, level.gameObject.transform.localScale * 1.2f, 0.2f));
            }
            else if (level == PreviousSelectedLevel)
            {
                StartCoroutine(LevelPanelScaleAnimation(level, level.gameObject.transform.localScale, level.gameObject.transform.localScale / 1.2f, 0.2f));
            }
        }
    }

    public IEnumerator LevelPanelScaleAnimation(GameObject levelPanel, Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            levelPanel.transform.localScale = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        levelPanel.transform.localScale = to; // 최종 크기로 설정
    }

    private void ActivateGameStartButton()
    {
        GameStartButton.interactable = true;

        ColorBlock colors = GameStartButton.colors;
        colors.normalColor = Color.white;

        GameStartButton.colors = colors;
    }

    private void DeactivateGameStartButton()
    {
        GameStartButton.interactable = false;

        ColorBlock colors = GameStartButton.colors;
        colors.normalColor = Color.gray;

        GameStartButton.colors = colors;
    }

    public void GameStartButtonPressed()
    {
        SceneTransitioner.Instance.SceneChange(CurrentSelectedLevel.GetComponent<LevelPanel>().SceneName);
        // SceneManager.LoadScene(CurrentSelectedLevel.GetComponent<LevelPanel>().SceneName);
    }
}
    