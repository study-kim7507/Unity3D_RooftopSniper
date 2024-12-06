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
        // ����Ű ����
        mainPopup.SetActive(false);
        shortcutKeysPopup.SetActive(true);
        IsChildPopupActive = true;
    }

    public void ShowGameDescriptionButtonPressed()
    {
        // ���� ���� ����
        mainPopup.SetActive(false);
        gameDescriptionPopup.SetActive(true);
        IsChildPopupActive = true;
    }

    public void ShowMainPopup()
    {
        // mainPopup�� Ȱ��ȭ�ϰ� �ٸ� �˾��� ��Ȱ��ȭ
        mainPopup.SetActive(true);
        shortcutKeysPopup.SetActive(false);
        gameDescriptionPopup.SetActive(false);
        IsChildPopupActive = false;
    }

    public void QuitButtonPressed()
    {
        // ���� ������, LevelSelectionScene ���� �̵�
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale;
        SceneManager.LoadScene("LevelSelectionScene");
    }
}
