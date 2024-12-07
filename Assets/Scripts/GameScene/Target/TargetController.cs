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

        gameObject.transform.position = NavMeshSurface.transform.position;
        NavMeshAgent.Warp(GetRandomPositionInNavMeshSurface());

        targetAnimatorController = GetComponent<TargetAnimatorController>();
        PerformRandomActionIdleOrWalkOrJog();

        GameManager.TargetRunAway += RunAway;
    }

    private void Update()
    {
        // 도착 여부 확인
        if (!hasArrived && NavMeshAgent.pathPending == false && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
    
            // 플레이어가 발각되었을 때 플레이어가 경찰을 죽이거나, 플레이어가 경찰에게 잡히기 전까지 계속 도망 다니도록
            if (GameManager.Instance.IsPlayerExposure) RunAway();
            else PerformRandomActionIdleOrWalkOrJog();
        }
    }

    public void Selected()  // 현재 오브젝트가 타겟으로 선정되었음
    {
        IsSelect = true;

        SetRenderTargetLayerAndSetRenderCameraCullingMask("Target");
        renderCamera.SetActive(true);
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
