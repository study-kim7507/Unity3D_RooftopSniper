using NUnit.Framework.Constraints;
using System.Collections;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class TargetController : PersonController
{
    private TargetAnimatorController targetAnimatorController;
    private bool hasArrived = true;

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfacesForPeople[UnityEngine.Random.Range(0, GameManager.Instance.NavMeshSurfacesForPeople.Count)];
        NavMeshAgent.areaMask = 1 << NavMeshSurface.defaultArea;

        targetAnimatorController = GetComponent<TargetAnimatorController>();
        PerformRandomActionIdleOrWalk();

        GameManager.TargetRunAway += RunAway;
    }

    private void Update()
    {
        // ���� ���� Ȯ��
        if (!hasArrived && NavMeshAgent.pathPending == false && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
    
            // �÷��̾ �߰��Ǿ��� �� �÷��̾ ������ ���̰ų�, �÷��̾ �������� ������ ������ ��� ���� �ٴϵ���
            if (GameManager.Instance.IsPlayerExposure) RunAway();
            else PerformRandomActionIdleOrWalk();
        }
    }

    public void Selected()  // ���� ������Ʈ�� Ÿ������ �����Ǿ���
    {
        IsSelect = true;

        SetRenderTargetLayerAndSetRenderCameraCullingMask("Target");
        renderCamera.SetActive(true);
    }

    protected override Vector3 GetRandomPositionInNavMeshSurface()
    {
        // NavMeshSurface�� ������� ���͸� ����Ͽ� ���� ������ ���
        Vector3 center = NavMeshSurface.transform.position + NavMeshSurface.center;
        Vector3 size = NavMeshSurface.size;

        // ���� ������ ��� (�������� ������ ���� ������ ����)
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Y���� ���� ������Ʈ�� Y���� �����ϵ��� ����
        float randomY = transform.position.y;

        return new Vector3(randomX, randomY, randomZ);
    }

    private void RunAway()
    {
        Vector3 RandomPosition = GetRandomPositionInNavMeshSurface();
        NavMeshAgent.speed = 2.5f;
        NavMeshAgent.SetDestination(RandomPosition);
        targetAnimatorController.Run();
        hasArrived = false;
    }

    private void PerformRandomActionIdleOrWalk()
    { 
        string randomAction = UnityEngine.Random.Range(0, 2) == 0 ? "Idle" : "Walk"; // 0 �Ǵ� 1�� �������� ����

        if (randomAction == "Idle")
        {
            targetAnimatorController.Idle();
            hasArrived = true;
        }
        else
        {
            Vector3 RandomPosition = GetRandomPositionInNavMeshSurface();
            NavMeshAgent.speed = 0.5f;
            NavMeshAgent.SetDestination(RandomPosition);
            targetAnimatorController.Walk();
            hasArrived = false;
        }   
    }

    private void OnDestroy()
    {
        GameManager.TargetRunAway -= RunAway;
    }
}
