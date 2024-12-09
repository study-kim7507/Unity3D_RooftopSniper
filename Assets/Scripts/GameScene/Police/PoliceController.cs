using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;
using static PoliceAnimatorController;
using System;

public class PoliceController : PersonController
{
    [Header("Police Mesh")]
    [SerializeField]
    private Mesh policeMesh;

    [Header("Police Arrow")]
    [SerializeField]
    private GameObject policeArrow;

    private PoliceAnimatorController policeAnimatorController;
    private bool hasArrived = true;
    private AudioSource audioSource;
    private float timeSpent = 0f; // 목적지 도달에 소요된 시간
    private const float maxTimeToReachDestination = 10f; // 10초

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfaceForPolice;
        NavMeshAgent.areaMask = 1 << NavMeshSurface.defaultArea;                // NavMeshAgent가 이동가능한 NavMeshSurface를 한정

        gameObject.transform.position = new Vector3(0, 0, 0);
        NavMeshAgent.Warp(GetRandomPositionInNavMeshSurface());

        SetRenderTargetLayerAndSetRenderCameraCullingMask("Police");

        policeAnimatorController = GetComponent<PoliceAnimatorController>();
        Patrol();
        
        GameManager.PoliceChasePlayer += ChasePlayer;
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
                if (GameManager.Instance.IsPlayerExposure) ChasePlayer();
                else Patrol();
            }
            else if (timeSpent >= maxTimeToReachDestination)
            {
                // 10초 이상 도착하지 못한 경우
                if (GameManager.Instance.IsPlayerExposure) ChasePlayer();
                else Patrol();
                timeSpent = 0f; // 시간 초기화
            }
        }
    }

    // 위장 경찰의 등장
    public IEnumerator CamouflagePoliceAppearRoutine()
    {
        renderCamera.SetActive(true);
        Time.timeScale = 0.7f;
     
        yield return new WaitForSecondsRealtime(1.0f);

        StartCoroutine(ChangeMeshRoutine());
    }

    public IEnumerator ChangeMeshRoutine()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = policeMesh;

        float x = -1.0f;
        while (x <= 1.0f)
        {
            x += Time.deltaTime;

            foreach (var render in renderers)
            {
                render.material.SetColor("_EffectColor", Color.blue);
                render.material.SetFloat("_AppearAmount", x);
            }

            yield return null;
        }

        Time.timeScale = 1.0f;

        // 위장 경찰의 카메라 (메인 카메라)를 비활성화
        renderCamera.SetActive(false);

        // 플레이어 카메라 활성화
        GameManager.Instance.DeactivatePoliceCamera();
    }

    private void ChasePlayer()
    {
        if (!isAlive) return;

        // 사이렌 재생
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        // 배경음악 변경
        GameManager.Instance.AudioSource.resource = GameManager.Instance.CrowdScream;
        GameManager.Instance.AudioSource.volume = 0.3f;
        GameManager.Instance.AudioSource.Play();

        policeArrow.gameObject.SetActive(true);
        Vector3 destination = new Vector3(GameManager.Instance.PoliceGoalTransform.position.x, gameObject.transform.position.y, GameManager.Instance.PoliceGoalTransform.position.z);
        NavMeshAgent.speed = 3.0f;
        NavMeshAgent.SetDestination(destination);
        policeAnimatorController.Run();
        hasArrived = false;
    }

    private void Patrol()
    {
        // Walk, Jog
        int randomAction = UnityEngine.Random.Range(0, 2);
        PoliceAnimState action = (PoliceAnimState)randomAction;

        Vector3 RandomPosition = GetRandomPositionInNavMeshSurface();
        NavMeshAgent.SetDestination(RandomPosition);
        hasArrived = false;

        if (action == PoliceAnimState.Walk)
        {
            NavMeshAgent.speed = 0.75f;
            policeAnimatorController.Walk();
        }
        else if (action == PoliceAnimState.Jog)
        {
            NavMeshAgent.speed = 2.0f;
            policeAnimatorController.Jog();
        }
    }

    private void OnDestroy()
    {
        // 배경음악 변경
        GameManager.Instance.AudioSource.resource = GameManager.Instance.GameBGM;
        GameManager.Instance.AudioSource.volume = 0.5f;
        GameManager.Instance.AudioSource.Play();

        GameManager.PoliceChasePlayer -= ChasePlayer;
        GameManager.Instance.IsPlayerExposure = false;
        GameManager.Instance.IsPlayerCaught = false;
    }
}
