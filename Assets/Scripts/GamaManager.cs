using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GamaManager : MonoBehaviour
{
    public static GamaManager Instance;

    [SerializeField]
    private List<GameObject> peopleObjects; // ���� �����ϴ� ����� (�� �߿��� ������ ����� �� ���� Ÿ������ ������)
                                            
    private void Awake()
    {
        Instance = this;
        GenerateNewTarget(null);                // ���� ���� �� Ÿ���� ����
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
