using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneCanvasController : MonoBehaviour
{
    [SerializeField]
    private GameObject mainPopup;
    [SerializeField]
    private GameObject gameDescriptionPopup;
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
        gameDescriptionPopup.SetActive(false);
        shortcutKeysDescriptionPopup.SetActive(false);
        mainPopup.SetActive(true);
    }

    public void GameStartButtonPressed()
    {
        // 씬 전환
        SceneManager.LoadScene("LevelSelectionScene");
    }

    public void ShowGameDescriptionButtonPressed()
    {
        // 게임 설명 보기
        mainPopup.SetActive(false);
        gameDescriptionPopup.SetActive(true);
    }

    public void ShowShortcutKeysDescriptionButtonPressed()
    {
        // 단축키 설명 보기
        mainPopup.SetActive(false);
        shortcutKeysDescriptionPopup.SetActive(true);
        
    }

    public void QuitButtonPressed()
    {
        // 게임 종료
        Application.Quit(); 
    }
}
