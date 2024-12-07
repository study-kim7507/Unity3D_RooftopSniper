using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text tip;
    [SerializeField]
    private string[] tipList;

    private void Start()
    {
        tip.text = tipList[UnityEngine.Random.Range(0, tipList.Length)];
        StartCoroutine(SceneTransitioner.Instance.FadeInLoadingSceneAndFadeOutNextScene());
    }
}
