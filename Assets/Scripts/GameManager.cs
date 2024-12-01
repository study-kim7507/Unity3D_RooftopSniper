

/*
 * TOOD: �̹� ���� �����ϴ� ������� �׺�޽� ������? 
 * TODO: ���, ���� �پ��� ����
 * TODO: �׺� �޽� �������� �̵� �� �ִϸ��̼�
 * TODO: ������ ��� ������ �׺�޽� ������ ��� ó���� ���ΰ�? -> Police Controller���� TargetController�� ������ ���� �޾Ƽ� ó���ؾ���
 *       -> �� ������Ʈ���� ��� ó���ϰ� �Ǹ� �浹�� �Ͼ ����
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.WSA;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private List<GameObject> peoplePrefabs;
    
    [SerializeField]
    private List<GameObject> peopleObjects; // ���� �����ϴ� ����� (�� �߿��� ������ ����� �� ���� Ÿ������ ������)

    [SerializeField]
    private GameObject policePrefab;
    [SerializeField]
    private GameObject policeObject;

    [SerializeField]
    private GameObject player;
    private Camera playerCamera;

    [SerializeField]
    [UnityEngine.Range(0.0f, 100.0f)]
    private float probabilityOfDetection = 50.0f;   // �߰��� Ȯ��

    [HideInInspector]
    public bool IsBulletCameraActive = false;
    [HideInInspector]
    public bool IsPoliceCameraActive = false;
    
    private bool isPlayerKilledCorrectTarget = true;
    private bool isPlayerKilledPolice = false;

    private float timerForGenerateNewPeople = 0.0f;

    [SerializeField]
    [UnityEngine.Range(3.0f, 15.0f)]
    private float timeIntervalForGenerateNewPeople = 10.0f;

    [SerializeField]
    private List<NavMeshSurface> navMeshSurfacesForPeople;

    [SerializeField]
    private NavMeshSurface navMeshSurfaceForPolice;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        playerCamera = Camera.main;

        AssignNewTargetIfCorrect(null);                // ���� ���� �� Ÿ���� ����
    }


    private void Update()
    {
        timerForGenerateNewPeople += Time.deltaTime;

        if (timerForGenerateNewPeople >= timeIntervalForGenerateNewPeople)
        {
            GenerateNewPeople();
            timerForGenerateNewPeople = 0.0f;
        }
    }

    public void AssignNewTargetIfCorrect(GameObject deadPeople)
    {
        if (deadPeople != null && (peopleObjects.IndexOf(deadPeople) != -1))
        { 
            peopleObjects.Remove(deadPeople);
        }

        if (deadPeople != null)
        {
            deadPeople = null;
        }

        if (peopleObjects.Count <= 0)
        {
            return;
        }

        // Ÿ���� �׿��� ����, ���ο� Ÿ���� ����
        if (isPlayerKilledCorrectTarget)
        {
            int randomIndex = UnityEngine.Random.Range(0, peopleObjects.Count);

            GameObject newTarget = peopleObjects[randomIndex];
            newTarget.GetComponent<TargetController>().Selected();
        }
    }

    public void ActivateBulletCameraAndSetResult(bool isPlayerKillCorrectTarget, bool isPlayerKillPolice)
    {
        // �÷��̾��� ī�޶�(����ī�޶�)�� ��Ȱ��ȭ
        // �Ѿ˿� �پ��ִ� ī�޶� ����ī�޶� �ǵ����Ͽ� �Ѿ��� ���ư��� ����� ������
        // ���Ҿ�, ���� �÷��̾ ���� ����� Ÿ������ Ȥ�� ���� �������� ����� ����
        if (playerCamera != null)
        {
            IsBulletCameraActive = true;
            
            // �÷��̾ ���� ����� Ÿ������ Ȥ�� ���� �������� ����� ����
            isPlayerKilledCorrectTarget = isPlayerKillCorrectTarget;    
            isPlayerKilledPolice = isPlayerKillPolice;

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            playerCamera.gameObject.SetActive(false);
            
            if (isPlayerKillCorrectTarget || isPlayerKillPolice)
            {
                Time.timeScale = 0.0075f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
            }
            else
            {
                Time.timeScale = 0.0175f;
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
            }
        }
    }

    public void DeactivateBulletCamera()
    {
        // �Ѿ˿� �پ��ִ� ī�޶�(����ī�޶�)�� ��Ȱ��ȭ
        // �÷��̾��� ī�޶� ����ī�޶� �ǵ����Ͽ� �ٽ� �÷��̾ ��������
        if (playerCamera != null)
        {
            IsBulletCameraActive = false;
            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            // �߰� ���θ� �����ϰ� �߰� ���ο� ���� ���� ���� (ī�޶� ����)
            EvaluateDiscoveryAndChangeCamera();
        }
    }

    // �߰� ���� ����
    private void EvaluateDiscoveryAndChangeCamera()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale;

        if (isPlayerKilledCorrectTarget || isPlayerKilledPolice)
        {
            // �÷��̾ Ÿ�� Ȥ�� ���� ������ ���̸� �߰����� ���� �÷��̾��� ī�޶� Ȱ��ȭ
            playerCamera.gameObject.SetActive(true);

            // TODO: ���� �÷��̾ ���� ������ ���� ��� ���ο� ���� ������ �����ϵ���
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
                // 1. ���� ������ ����
                ActivatePoliceCamera();
                StartCoroutine(policeObject.GetComponent<PoliceController>().CamouflagePoliceAppearRoutine());

                // TODO: 2. ���� �����ϴ� ������� ����
                
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

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            playerCamera.gameObject.SetActive(false);
        }
    }

    public void DeactivatePoliceCamera()
    {
        if (playerCamera != null)
        {
            IsPoliceCameraActive = false;

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            playerCamera.gameObject.SetActive(true);
        }
    }

    private void GenerateNewPeople()
    {
        Debug.Log("���ο� ����� �����Ǿ����ϴ�.");

        NavMeshSurface navMeshSurface = navMeshSurfacesForPeople[UnityEngine.Random.Range(0, navMeshSurfacesForPeople.Count)];
        Vector3 spawnPosition = GetRandomPositionInNavMeshSurface(navMeshSurface);

        GameObject newPerson = Instantiate(peoplePrefabs[UnityEngine.Random.Range(0, peoplePrefabs.Count)], spawnPosition, Quaternion.identity);
        newPerson.GetComponent<TargetController>().NavMeshSurface = navMeshSurface;

        AddNewPeople(newPerson);
    }

    private void GenerateNewPolice()
    {
        Debug.Log("���ο� ���� ������ �����Ǿ����ϴ�.");
        GameObject newPolice = Instantiate(policePrefab);
        AddNewPolice(newPolice);
    }

    private void AddNewPeople(GameObject newPerson)
    {
        peopleObjects.Add(newPerson);
    }

    private void AddNewPolice(GameObject newPolice)
    {
        policeObject = newPolice;
    }

    private Vector3 GetRandomPositionInNavMeshSurface(NavMeshSurface surface)
    {
        // NavMeshSurface�� ������� ���͸� ����Ͽ� ���� ������ ���
        Vector3 center = surface.transform.position + surface.center;
        Vector3 size = surface.size;

        // ���� ������ ��� (�������� ������ ���� ������ ����)
        float randomX = UnityEngine.Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = UnityEngine.Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Y���� ���� ������Ʈ�� Y���� �����ϵ��� ����
        float randomY = transform.position.y;

        return new Vector3(randomX, randomY, randomZ);
    }
}
