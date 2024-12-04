/*
 * TODO: �� ������ �� UI ������
 * TODO: ���� ����Ʈ
 * TODO: TargetController GetRandomPositionInNavMeshSurface ���� �ʿ�
 * TODO: �ٸ� ������Ʈ��� Destination�� ��ġ�� ���� ���� �ʿ� -> ���� �ð� ���� ���� ���ϸ� ��� ��Ű�� ������
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.WSA;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static event Action TargetRunAway;
    public static event Action PoliceChasePlayer;

    public List<NavMeshSurface> NavMeshSurfacesForPeople;
    public NavMeshSurface NavMeshSurfaceForPolice;

    [HideInInspector]
    public bool IsBulletCameraActive = false;
    [HideInInspector]
    public bool IsPoliceCameraActive = false;

    [HideInInspector]
    public bool IsPlayerExposure = false;

    [Header("Person Settings")]
    [SerializeField]
    private List<GameObject> personPrefabs;
    [SerializeField]
    private List<GameObject> personObjects;                     // ���� �����ϴ� ����� (�� �߿��� ������ ����� �� ���� Ÿ������ ������)

    [Header("Police Settings")]
    [SerializeField]
    private List<GameObject> policePrefabs;
    [SerializeField]
    private GameObject policeObject;

    [Header("Player")]
    [SerializeField]
    private GameObject player;

    [Header("Game Settings")]
    public int Life = 3;                                            // �÷��̾��� ���� ��� ��
    public int NumberOfDetections = 0;                              // �÷��̾� �߰� Ƚ��
    public float RemainingTime = 180.0f;                            // ���� �ð�
    public float Reward = 0.0f;                                     // ������
    [SerializeField]
    [UnityEngine.Range(0.0f, 100.0f)]
    private float probabilityOfDetection = 50.0f;               // �߰��� Ȯ��
    [SerializeField]
    [UnityEngine.Range(3.0f, 30.0f)]
    private float timeIntervalForGenerateNewPeople = 10.0f;
    public Transform PoliceGoalTransform;

    private Camera playerCamera;
    private float timerForGenerateNewPeople = 0.0f;
    private GameObject currentTarget = null;
    
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        playerCamera = Camera.main;
    }

    private void Update()
    {
        timerForGenerateNewPeople += Time.deltaTime;

        if (timerForGenerateNewPeople >= timeIntervalForGenerateNewPeople)
        {
            GenerateNewPerson();
            timerForGenerateNewPeople = 0.0f;
        }

        InitializeAndAssignNewTarget();
    }

    // ���� ���� �� �ʱ� Ÿ�� ���� �� �ƹ��� ����� ���� �� ���������� Ȯ���Ͽ� ���ο� ����� �����ϸ� Ÿ������ ����
    private void InitializeAndAssignNewTarget()
    {
        if (personObjects.Count > 0 && currentTarget == null)
        {
            int randomIndex = UnityEngine.Random.Range(0, personObjects.Count);
            GameObject newTarget = personObjects[randomIndex];
            newTarget.GetComponent<TargetController>().Selected();
            currentTarget = newTarget;
        }
    }

    public void AssignNewTargetIfCorrect(GameObject who)
    {
        bool isPlayerKilledCorrectTarget = false;
        bool isPlayerKilledPolice = false;

        if (who != null)
        {
            isPlayerKilledCorrectTarget = who.GetComponent<PersonController>().IsSelect;
            isPlayerKilledPolice = who.GetComponent<PersonController>().IsPolice;

            if (personObjects.IndexOf(who) != -1)
                personObjects.Remove(who);
        }

        if (isPlayerKilledCorrectTarget)
        {
            if (personObjects.Count <= 0) currentTarget = null;
            else
            {
                int randomIndex = UnityEngine.Random.Range(0, personObjects.Count);

                GameObject newTarget = personObjects[randomIndex];
                newTarget.GetComponent<TargetController>().Selected();
                currentTarget = newTarget;
            }
        }

        EvaluateAndHandleExposure(isPlayerKilledCorrectTarget, isPlayerKilledPolice);
    }

    public void ActivateBulletCamera(GameObject hitObject)
    {
        // �÷��̾��� ī�޶�(����ī�޶�)�� ��Ȱ��ȭ
        // �Ѿ˿� �پ��ִ� ī�޶� ����ī�޶� �ǵ����Ͽ� �Ѿ��� ���ư��� ����� ������

        if (playerCamera != null)
        {
            IsBulletCameraActive = true;
            playerCamera.gameObject.SetActive(false);

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            Time.timeScale = hitObject != null ? 0.025f : 0.3f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public void DeactivateBulletCamera()
    {
        // �Ѿ˿� �پ��ִ� ī�޶�(����ī�޶�)�� ��Ȱ��ȭ
        // �÷��̾��� ī�޶� ����ī�޶� �ǵ����Ͽ� �ٽ� �÷��̾ ��������
        if (playerCamera != null)
        {
            IsBulletCameraActive = false;
            playerCamera.gameObject.SetActive(true);

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = Time.timeScale;
        }
    }

    // �߰� ���� ����
    private void EvaluateAndHandleExposure(bool isPlayerKilledCorrectTarget, bool isPlayerKilledPolice)
    {
        if (isPlayerKilledCorrectTarget || isPlayerKilledPolice)
        {   
            // ���� �÷��̾ ���� ������ ���� ��� ���ο� ���� ������ �����ϵ���
            if (isPlayerKilledPolice)
            {
                GenerateNewPolice();
            }
        }
        else
        {
            // �÷��̾ Ÿ���� �ƴ� �ٸ� ���� ���� �Ѿ��� �߻��� (Ÿ���� �ƴ� ����� ����)
            // ���� Ȯ���� ���� �߰� ���θ� ����

            if (UnityEngine.Random.Range(0.0f, 100.0f) <= probabilityOfDetection)
            {
                // �߰� ����
                NumberOfDetections++;                   // �߰� Ƚ�� ����
                
                if (!IsPlayerExposure)
                {
                    IsPlayerExposure = true;
                    ActivatePoliceCamera();
                    StartCoroutine(policeObject.GetComponent<PoliceController>().CamouflagePoliceAppearRoutine());
                }
                PoliceChasePlayer?.Invoke();            // ������ �÷��̾ ������ �̵���

                TargetRunAway?.Invoke();                // ������� �ڽ��� ���� NavMeshSurface �󿡼� �����ٴ�
                
            }
            else
            {
                // �÷��̾ Ÿ���� �ƴ� �ùٸ��� ���� ������ �Ѿ��� �߻� ������, ���� ���� �߰����� ����
                // �÷��̾��� ī�޶� Ȱ��ȭ
                playerCamera.gameObject.SetActive(true);
            }
        }
    }

    private void ActivatePoliceCamera()
    {
        if (playerCamera != null)
        {
            IsPoliceCameraActive = true;
            playerCamera.gameObject.SetActive(false);

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }
        }
    }

    public void DeactivatePoliceCamera()
    {
        if (playerCamera != null)
        {
            IsPoliceCameraActive = false;
            playerCamera.gameObject.SetActive(true);

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }
        }
    }

    private void GenerateNewPerson()
    {
        Debug.Log("���ο� ����� �����Ǿ����ϴ�");

        int random = (int)UnityEngine.Random.Range(0, personPrefabs.Count);
        GameObject newPerson = Instantiate(personPrefabs[random]);
        personObjects.Add(newPerson);
    }

    private void GenerateNewPolice()
    {
        Debug.Log("���ο� ���� ������ �����Ǿ����ϴ�.");

        int random = (int)UnityEngine.Random.Range(0, policePrefabs.Count);
        GameObject newPolice = Instantiate(policePrefabs[random]);
        policeObject = newPolice;
    }
}
