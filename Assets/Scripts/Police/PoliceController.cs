using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting;
using static PoliceAnimatorController;

public class PoliceController : PersonController
{
    [Header("Police Mesh")]
    [SerializeField]
    private Mesh policeMesh;

    private PoliceAnimatorController policeAnimatorController;
    private bool hasArrived = true;

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfaceForPolice;
        NavMeshAgent.areaMask = 1 << NavMeshSurface.defaultArea;                // NavMeshAgent�� �̵������� NavMeshSurface�� ����

        NavMeshAgent.Warp(GetRandomPositionInNavMeshSurface());

        SetRenderTargetLayerAndSetRenderCameraCullingMask("Police");

        policeAnimatorController = GetComponent<PoliceAnimatorController>();
        Patrol();
        
        GameManager.PoliceChasePlayer += ChasePlayer;
    }

    private void Update()
    {
        // ���� ���� Ȯ��
        if (!hasArrived && NavMeshAgent.pathPending == false && NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
        {
            hasArrived = true;
            Patrol();
        }
    }

    // ���� ������ ����
    public IEnumerator CamouflagePoliceAppearRoutine()
    {
        renderCamera.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(ChangeMeshRoutine());
    }

    public IEnumerator ChangeMeshRoutine()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = policeMesh;

        float x = -1.0f;
        while (x <= 1.0f)
        {
            x += Time.deltaTime * 1.5f;

            foreach (var render in renderers)
            {
                render.material.SetColor("_EffectColor", Color.blue);
                render.material.SetFloat("_AppearAmount", x);
            }

            yield return null;
        }

        // ���� ������ ī�޶� (���� ī�޶�)�� ��Ȱ��ȭ
        renderCamera.SetActive(false);

        // �÷��̾� ī�޶� Ȱ��ȭ
        GameManager.Instance.DeactivatePoliceCamera();
    }

    protected override Vector3 GetRandomPositionInNavMeshSurface()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 30.0f;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 30.0f, NavMeshAgent.areaMask);
        return hit.position;
    }


    private void ChasePlayer()
    {
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
            NavMeshAgent.speed = 0.5f;
            policeAnimatorController.Walk();
        }
        else if (action == PoliceAnimState.Jog)
        {
            NavMeshAgent.speed = 1.0f;
            policeAnimatorController.Jog();
        }
    }

    private void OnDestroy()
    {
        GameManager.PoliceChasePlayer -= ChasePlayer;
        GameManager.Instance.IsPlayerExposure = false;
    }
}
