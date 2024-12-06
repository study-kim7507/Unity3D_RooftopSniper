using NUnit.Framework.Constraints;
using System.Collections;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static TargetAnimatorController;

public class TargetController : PersonController
{
    private TargetAnimatorController targetAnimatorController;
    private bool hasArrived = true;

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfacesForPeople[UnityEngine.Random.Range(0, GameManager.Instance.NavMeshSurfacesForPeople.Count)];
        NavMeshAgent.areaMask = 1 << NavMeshSurface.defaultArea;

        NavMeshAgent.Warp(GetRandomPositionInNavMeshSurface());

        targetAnimatorController = GetComponent<TargetAnimatorController>();
        PerformRandomActionIdleOrWalkOrJog();

        GameManager.TargetRunAway += RunAway;
    }

    private void Update()
    {
        // ���� ���� Ȯ��
        if (!hasArrived && NavMeshAgent.pathPending == false && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
    
            // �÷��̾ �߰��Ǿ��� �� �÷��̾ ������ ���̰ų�, �÷��̾ �������� ������ ������ ��� ���� �ٴϵ���
            if (GameManager.Instance.IsPlayerExposure) RunAway();
            else PerformRandomActionIdleOrWalkOrJog();
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
        float randomY = 0;

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

    private void PerformRandomActionIdleOrWalkOrJog()
    {
        // Idle, Walk, Jog
        int randomAction = UnityEngine.Random.Range(0, 3);
        TargetAnimState action = (TargetAnimState)randomAction;

        
        if (action == TargetAnimState.Idle)
        {
            targetAnimatorController.Idle();
            hasArrived = true;
        }
        else 
        {
            Vector3 RandomPosition = GetRandomPositionInNavMeshSurface();
            NavMeshAgent.SetDestination(RandomPosition);
            hasArrived = false;

            if (action == TargetAnimState.Walk)
            {
                NavMeshAgent.speed = 0.5f;
                targetAnimatorController.Walk();
            }
            else if (action == TargetAnimState.Jog)
            {
                NavMeshAgent.speed = 1.0f;
                targetAnimatorController.Jog();
            }
        }   
    }

    private void OnDestroy()
    {
        GameManager.TargetRunAway -= RunAway;
    }
}
