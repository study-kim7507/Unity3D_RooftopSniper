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
    private float timeSpent = 0f; // ������ ���޿� �ҿ�� �ð�
    private const float maxTimeToReachDestination = 10f; // 10��

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfaceForPolice;
        NavMeshAgent.areaMask = 1 << NavMeshSurface.defaultArea;                // NavMeshAgent�� �̵������� NavMeshSurface�� ����

        gameObject.transform.position = new Vector3(0, 0, 0);
        NavMeshAgent.Warp(GetRandomPositionInNavMeshSurface());

        SetRenderTargetLayerAndSetRenderCameraCullingMask("Police");

        policeAnimatorController = GetComponent<PoliceAnimatorController>();
        Patrol();
        
        GameManager.PoliceChasePlayer += ChasePlayer;
    }

    private void Update()
    {
        // ���� ���� Ȯ��
        if (!hasArrived && NavMeshAgent.pathPending == false)
        {
            timeSpent += Time.deltaTime; // ��� �ð� ����

            if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
            {
                // ������ ���
                hasArrived = true;
                timeSpent = 0f; // �ð� �ʱ�ȭ
                if (GameManager.Instance.IsPlayerExposure) ChasePlayer();
                else Patrol();
            }
            else if (timeSpent >= maxTimeToReachDestination)
            {
                // 10�� �̻� �������� ���� ���
                if (GameManager.Instance.IsPlayerExposure) ChasePlayer();
                else Patrol();
                timeSpent = 0f; // �ð� �ʱ�ȭ
            }
        }
    }

    // ���� ������ ����
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

        // ���� ������ ī�޶� (���� ī�޶�)�� ��Ȱ��ȭ
        renderCamera.SetActive(false);

        // �÷��̾� ī�޶� Ȱ��ȭ
        GameManager.Instance.DeactivatePoliceCamera();
    }

    private void ChasePlayer()
    {
        if (!isAlive) return;

        // ���̷� ���
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        // ������� ����
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
        // ������� ����
        GameManager.Instance.AudioSource.resource = GameManager.Instance.GameBGM;
        GameManager.Instance.AudioSource.volume = 0.5f;
        GameManager.Instance.AudioSource.Play();

        GameManager.PoliceChasePlayer -= ChasePlayer;
        GameManager.Instance.IsPlayerExposure = false;
        GameManager.Instance.IsPlayerCaught = false;
    }
}
