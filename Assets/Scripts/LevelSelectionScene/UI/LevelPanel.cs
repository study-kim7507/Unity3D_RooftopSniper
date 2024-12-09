using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    public TMP_Text LevelName;
    public Image LevelThumbnail;
    [HideInInspector]
    public string SceneName;


    [HideInInspector]
    public bool IsSeleted = false;
    [HideInInspector]
    public bool CanSelect = true;

    public void SetLevelInformation(string levelName, Sprite thumbnail, string sceneName)
    {
        LevelName.text = levelName;
        LevelThumbnail.sprite = thumbnail;
        SceneName = sceneName;
        if (SceneName == "") CanSelect = false;
    }

    public void LevelSelected()
    {
        if (!CanSelect) return;
        LevelSelectionSceneManager.Instance.CurrentSelectedLevel = gameObject;
    }
}
