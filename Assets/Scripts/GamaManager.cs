using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GamaManager : MonoBehaviour
{
    public static GamaManager Instance;

    [SerializeField]
    private List<GameObject> peopleObjects; // 씬에 존재하는 사람들 (이 중에서 랜덤한 사람이 매 시점 타겟으로 선정됨)
                                            
    private void Awake()
    {
        Instance = this;
        GenerateNewTarget(null);                // 게임 시작 시 타겟을 생성
    }

    public void GenerateNewTarget(GameObject currentTarget)
    {
        if (currentTarget != null)
        { 
            peopleObjects.Remove(currentTarget);
            currentTarget = null;
        }

        if (peopleObjects.Count <= 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, peopleObjects.Count);

        currentTarget = peopleObjects[randomIndex];
        currentTarget.GetComponent<TargetController>().Selected();
    }
}
