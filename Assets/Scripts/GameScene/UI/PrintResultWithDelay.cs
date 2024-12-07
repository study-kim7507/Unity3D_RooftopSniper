using System.Collections;
using TMPro;
using UnityEngine;

public class PrintResultWithDelay : MonoBehaviour
{
    public TMP_Text Text;
    public enum ResultType
    {
        TargetCount,
        PoliceCount,
        DetectionCount,
        Entire,
    }


    public ResultType Type;

    private int TargetCountScore;
    private int PoliceCountScore;
    private int DetectionCountScore;
    private int EntireScore;

    private string StringForPrint;

    private void Start()
    {
        TargetCountScore = GameManager.Instance.TargetKillCount * 100;
        PoliceCountScore = GameManager.Instance.PoliceKillCount * 200;
        DetectionCountScore = GameManager.Instance.NumberOfDetections * -50;
        EntireScore = TargetCountScore + PoliceCountScore + DetectionCountScore;

        switch (Type)
        {
            case ResultType.TargetCount:
                StringForPrint = GameManager.Instance.TargetKillCount.ToString() + " x 100 = " + TargetCountScore.ToString();
                break;
            case ResultType.PoliceCount:
                StringForPrint = GameManager.Instance.PoliceKillCount.ToString() + " x 200 = " + PoliceCountScore.ToString();
                break;
            case ResultType.DetectionCount:
                StringForPrint = GameManager.Instance.NumberOfDetections.ToString() + " x -50 = " + DetectionCountScore.ToString();
                break;
            case ResultType.Entire:
                StringForPrint = "최종 점수 : " + EntireScore.ToString();
                break;
        }

        StartCoroutine(textPrint(0.125f));
    }

    private IEnumerator textPrint(float delay)
    {
        Text.text = "";
        int count = 0;

        while (count != StringForPrint.Length)
        {
            if (count < StringForPrint.Length)
            {
                Text.text += StringForPrint[count].ToString();
                count++;
            }

            yield return new WaitForSecondsRealtime(delay);
        }
    }
}
