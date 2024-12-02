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
        // 도착 여부 확인
        if (!hasArrived && NavMeshAgent.pathPending == false && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
    
            // 플레이어가 발각되었을 때 플레이어가 경찰을 죽이거나, 플레이어가 경찰에게 잡히기 전까지 계속 도망 다니도록
            if (GameManager.Instance.IsPlayerExposure) RunAway();
            else PerformRandomActionIdleOrWalk();
        }
    }

    public void Selected()  // 현재 오브젝트가 타겟으로 선정되었음
    {
        IsSelect = true;

        SetRenderTargetLayerAndSetRenderCameraCullingMask("Target");
        renderCamera.SetActive(true);
    }

    protected override Vector3 GetRandomPositionInNavMeshSurface()
    {
        // NavMeshSurface의 사이즈와 센터를 사용하여 랜덤 포지션 계산
        Vector3 center = NavMeshSurface.transform.position + NavMeshSurface.center;
        Vector3 size = NavMeshSurface.size;

        // 랜덤 포지션 계산 (사이즈의 절반을 빼서 범위를 맞춤)
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Y축은 현재 오브젝트의 Y축을 유지하도록 설정
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
        string randomAction = UnityEngine.Random.Range(0, 2) == 0 ? "Idle" : "Walk"; // 0 또는 1을 랜덤으로 선택

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
