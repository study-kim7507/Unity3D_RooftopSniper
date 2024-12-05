using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
            titleText.text = "���������� �Ƿڸ� �ذ��Ͽ����ϴ�.";
            subTitleText.text = "���� �ð��� ����Ǿ����ϴ�.";
            StartCoroutine(ActivateResults());
        }
        else
        {
            titleText.text = "�Ƿڸ� �ذ����� ���Ͽ����ϴ�.";
            subTitleText.text = "�������� ���������ϴ�.";
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
        // TODO: �� ��ȯ
        Application.Quit();
    }
}
