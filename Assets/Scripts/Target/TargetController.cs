using NUnit.Framework.Constraints;
using System.Collections;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TargetController : PersonController
{
    [Header("Render Camera")]
    [SerializeField]
    private GameObject renderCamera;

    private TargetAnimatorController targetAnimatorController;
    private TargetMovementController targetMovementController;

    protected override void Awake()
    {
        base.Awake();

        targetAnimatorController = GetComponent<TargetAnimatorController>();
        targetMovementController = GetComponent<TargetMovementController>();

        NavMeshSurface = GameManager.Instance.NavMeshSurfacesForPeople[UnityEngine.Random.Range(0, GameManager.Instance.NavMeshSurfacesForPeople.Count)];
        gameObject.transform.position = GetRandomPositionInNavMeshSurface();
    }

    public void Selected()  // ���� ������Ʈ�� Ÿ������ �����Ǿ���
    {
        IsSelect = true;
        renderCamera.SetActive(true);
    }
}
