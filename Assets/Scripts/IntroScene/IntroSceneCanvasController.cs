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
        // �� ��ȯ
        SceneManager.LoadScene("LevelSelectionScene");
    }

    public void ShowGameDescriptionButtonPressed()
    {
        // ���� ���� ����
        mainPopup.SetActive(false);
        gameDescriptionPopup.SetActive(true);
    }

    public void ShowShortcutKeysDescriptionButtonPressed()
    {
        // ����Ű ���� ����
        mainPopup.SetActive(false);
        shortcutKeysDescriptionPopup.SetActive(true);
        
    }

    public void QuitButtonPressed()
    {
        // ���� ����
        Application.Quit(); 
    }
}
