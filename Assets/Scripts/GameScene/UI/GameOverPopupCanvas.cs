using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPopupCanvas : MonoBehaviour
{
    [Header("Main Texts")]
    [SerializeField]
    private TMP_Text titleText;
    [SerializeField]
    private TMP_Text subTitleText;

    [Header("Result List")]
    [SerializeField]
    private List<GameObject> resultList;

    private void OnEnable()
    {
        if (GameManager.Instance.IsGameCleared)
        {
            titleText.text = "성공적으로 의뢰를 해결하였습니다.";
            subTitleText.text = "제한 시간이 종료되었습니다.";
            StartCoroutine(ActivateResults());
        }
        else
        {
            titleText.text = "의뢰를 해결하지 못하였습니다.";
            subTitleText.text = "경찰에게 붙잡혔습니다.";
        }

    }

    private IEnumerator ActivateResults()
    {
        foreach (GameObject result in resultList)
        {
            result.SetActive(true);
            yield return new WaitForSecondsRealtime(1.5f);
        }
    }

    public void QuitButtonPressed()
    {
        // 게임 끝내기, IntroScene 으로 이동
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale;
        SceneTransitioner.Instance.SceneChange("IntroScene");
    }
}
