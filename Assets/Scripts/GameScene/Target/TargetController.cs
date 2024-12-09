using NUnit.Framework.Constraints;
using System;
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
    private float timeSpent = 0f; // 목적지 도달에 소요된 시간
    private const float maxTimeToReachDestination = 10f; // 10초

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfacesForPeople[UnityEngine.Random.Range(0, GameManager.Instance.NavMeshSurfacesForPeople.Count)];
        NavMeshAgent.areaMask = 1 << NavMeshSurface.defaultArea;

        gameObject.transform.position = new Vector3(0, 0, 0);
        NavMeshAgent.Warp(GetRandomPositionInNavMeshSurface());

        targetAnimatorController = GetComponent<TargetAnimatorController>();
        PerformRandomActionIdleOrWalkOrJog();

        GameManager.TargetRunAway += RunAway;
    }

    private void Update()
    {
        // 도착 여부 확인
        if (!hasArrived && NavMeshAgent.pathPending == false)
        {
            timeSpent += Time.deltaTime; // 경과 시간 증가

            if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
            {
                // 도착한 경우
                hasArrived = true;
                timeSpent = 0f; // 시간 초기화
                if (GameManager.Instance.IsPlayerExposure) RunAway();
                else PerformRandomActionIdleOrWalkOrJog();
            }
            else if (timeSpent >= maxTimeToReachDestination)
            {
                // 10초 이상 도착하지 못한 경우
                if (GameManager.Instance.IsPlayerExposure) RunAway();
                else PerformRandomActionIdleOrWalkOrJog();
                timeSpent = 0f; // 시간 초기화
            }
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
        if (!isAlive) return;
        Vector3 RandomPosition = GetRandomPositionInNavMeshSurface();
        NavMeshAgent.speed = 3.5f;
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
            NavMeshAgent.speed = 0.0f;
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
                NavMeshAgent.speed = 0.75f;
                targetAnimatorController.Walk();
            }
            else if (action == TargetAnimState.Jog)
            {
                NavMeshAgent.speed = 2.0f;
                targetAnimatorController.Jog();
            }
        }   
    }

    private void OnDestroy()
    {
        GameManager.TargetRunAway -= RunAway;
    }
}
