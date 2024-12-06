using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    [HideInInspector]
    public TMP_Text LevelName;
    [HideInInspector]
    public Image LevelThumbnail;
    [HideInInspector]
    public string SceneName;


    [HideInInspector]
    public bool IsSeleted = false;

    public void SetLevelInformation(string levelName, Sprite thumbnail, string sceneName)
    {
        LevelName.text = levelName;
        LevelThumbnail.sprite = thumbnail;
        SceneName = sceneName;
    }

    public void LevelSelected()
    {
        LevelSelectionSceneManager.Instance.CurrentSelectedLevel = gameObject;
    }
}
