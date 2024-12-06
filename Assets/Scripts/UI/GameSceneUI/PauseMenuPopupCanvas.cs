using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuPopupCanvas : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPopup;
    [SerializeField]
    private GameObject gameDescriptionPopup;
    [SerializeField]
    private GameObject shortcutKeysPopup;

    [HideInInspector]
    public bool IsChildPopupActive = false;

    public void ShowShortcutKeysDescriptionButtonPressed()
    {
        // 단축키 보기
        mainPopup.SetActive(false);
        shortcutKeysPopup.SetActive(true);
        IsChildPopupActive = true;
    }

    public void ShowGameDescriptionButtonPressed()
    {
        // 게임 설명 보기
        mainPopup.SetActive(false);
        gameDescriptionPopup.SetActive(true);
        IsChildPopupActive = true;
    }

    public void ShowMainPopup()
    {
        // mainPopup을 활성화하고 다른 팝업을 비활성화
        mainPopup.SetActive(true);
        shortcutKeysPopup.SetActive(false);
        gameDescriptionPopup.SetActive(false);
        IsChildPopupActive = false;
    }

    public void QuitButtonPressed()
    {
        // 게임 끝내기, LevelSelectionScene 으로 이동
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale;
        SceneManager.LoadScene("LevelSelectionScene");
    }
}
