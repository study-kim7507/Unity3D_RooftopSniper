using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class LevelInfo
{
    public string LevelName;
    public Sprite LevelThumb;
    public string SceneName;
}
public class LevelSelectionSceneManager : MonoBehaviour
{
    public static LevelSelectionSceneManager Instance;

    public List<LevelInfo> Levels;
    public GameObject LevelPanelPrefab;
    public GameObject ScrollViewContent;
    public GameObject LevelSelectionSceneCanvas;

    [HideInInspector]
    public GameObject PreviousSelectedLevel;
    [HideInInspector]
    public GameObject CurrentSelectedLevel;

    private LevelSelectionSceneCanvasController LevelSelectionSceneCanvasController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LevelSelectionSceneCanvasController = LevelSelectionSceneCanvas.GetComponent<LevelSelectionSceneCanvasController>();
    }
    private void Start()
    {
        for (int idx = 0; idx < Levels.Count; idx++)
        {
            LevelInfo info = Levels[idx];
            GameObject go = Instantiate(LevelPanelPrefab, ScrollViewContent.transform);
            go.GetComponent<LevelPanel>().SetLevelInformation(info.LevelName, info.LevelThumb, info.SceneName);
            LevelSelectionSceneCanvasController.Levels.Add(go);
        }
    }

    private void Update()
    {
        // IntroScene으로 전환
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneTransitioner.Instance.SceneChange("IntroScene");
        }

        if (CurrentSelectedLevel != null && PreviousSelectedLevel != CurrentSelectedLevel)
        {
            PreviousSelectedLevel = CurrentSelectedLevel;
            LevelSelectionSceneCanvasController.ChangeSelectedLevel(CurrentSelectedLevel);
        }
    }
}