

/*
 * TOOD: 이미 씬에 존재하는 사람들의 네브메시 설정은? 
 * TODO: 사람, 경찰 다양한 색상
 * TODO: 네브 메시 내에서의 이동 및 애니메이션
 * TODO: 경찰의 경우 스폰과 네브메시 설정을 어떻게 처리할 것인가? -> Police Controller에서 TargetController가 할일을 위임 받아서 처리해야함
 *       -> 두 컴포넌트에서 모두 처리하게 되면 충돌이 일어날 것임
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
    private List<GameObject> peopleObjects; // 씬에 존재하는 사람들 (이 중에서 랜덤한 사람이 매 시점 타겟으로 선정됨)

    [SerializeField]
    private GameObject policePrefab;
    [SerializeField]
    private GameObject policeObject;

    [SerializeField]
    private GameObject player;
    private Camera playerCamera;

    [SerializeField]
    [UnityEngine.Range(0.0f, 100.0f)]
    private float probabilityOfDetection = 50.0f;   // 발각될 확률

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

        AssignNewTargetIfCorrect(null);                // 게임 시작 시 타겟을 생성
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

        // 타겟을 죽였을 때만, 새로운 타겟을 지정
        if (isPlayerKilledCorrectTarget)
        {
            int randomIndex = UnityEngine.Random.Range(0, peopleObjects.Count);

            GameObject newTarget = peopleObjects[randomIndex];
            newTarget.GetComponent<TargetController>().Selected();
        }
    }

    public void ActivateBulletCameraAndSetResult(bool isPlayerKillCorrectTarget, bool isPlayerKillPolice)
    {
        // 플레이어의 카메라(메인카메라)를 비활성화
        // 총알에 붙어있는 카메라가 메인카메라가 되도록하여 총알이 날아가는 장면을 보여줌
        // 더불어, 현재 플레이어가 죽인 사람이 타겟인지 혹은 위장 경찰인지 결과를 저장
        if (playerCamera != null)
        {
            IsBulletCameraActive = true;
            
            // 플레이어가 죽인 사람이 타겟인지 혹은 위장 경찰인지 결과를 저장
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
        // 총알에 붙어있는 카메라(메인카메라)를 비활성화
        // 플레이어의 카메라가 메인카메라가 되도록하여 다시 플레이어를 시점으로
        if (playerCamera != null)
        {
            IsBulletCameraActive = false;
            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            // 발각 여부를 결정하고 발각 여부에 따라 시점 변경 (카메라 변경)
            EvaluateDiscoveryAndChangeCamera();
        }
    }

    // 발각 여부 결정
    private void EvaluateDiscoveryAndChangeCamera()
    {
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale;

        if (isPlayerKilledCorrectTarget || isPlayerKilledPolice)
        {
            // 플레이어가 타겟 혹은 위장 경찰을 죽이면 발각되지 않음 플레이어의 카메라를 활성화
            playerCamera.gameObject.SetActive(true);

            // TODO: 만약 플레이어가 위장 경찰을 죽인 경우 새로운 위장 경찰이 등장하도록
            if (isPlayerKilledPolice)
            {
                GenerateNewPolice();
            }
        }
        else
        {
            // 플레이어가 타겟이 아닌 다른 곳을 향해 총알을 발사함 (타겟이 아닌 사람을 죽임)
            // 일정 확률에 의해 발각 여부를 결정

            if (UnityEngine.Random.Range(0.0f, 100.0f) <= probabilityOfDetection)
            {
                // 발각 로직
                // 1. 위장 경찰의 등장
                ActivatePoliceCamera();
                StartCoroutine(policeObject.GetComponent<PoliceController>().CamouflagePoliceAppearRoutine());

                // TODO: 2. 씬에 존재하는 사람들의 도망
                
            }
            else
            {
                // 플레이어가 타겟이 아닌 올바르지 못한 곳으로 총알을 발사 했으나, 운이 좋아 발각되지 않음
                // 플레이어의 카메라를 활성화
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
        Debug.Log("새로운 사람이 생성되었습니다.");

        NavMeshSurface navMeshSurface = navMeshSurfacesForPeople[UnityEngine.Random.Range(0, navMeshSurfacesForPeople.Count)];
        Vector3 spawnPosition = GetRandomPositionInNavMeshSurface(navMeshSurface);

        GameObject newPerson = Instantiate(peoplePrefabs[UnityEngine.Random.Range(0, peoplePrefabs.Count)], spawnPosition, Quaternion.identity);
        newPerson.GetComponent<TargetController>().NavMeshSurface = navMeshSurface;

        AddNewPeople(newPerson);
    }

    private void GenerateNewPolice()
    {
        Debug.Log("새로운 위장 경찰이 생성되었습니다.");
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
        // NavMeshSurface의 사이즈와 센터를 사용하여 랜덤 포지션 계산
        Vector3 center = surface.transform.position + surface.center;
        Vector3 size = surface.size;

        // 랜덤 포지션 계산 (사이즈의 절반을 빼서 범위를 맞춤)
        float randomX = UnityEngine.Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = UnityEngine.Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Y축은 현재 오브젝트의 Y축을 유지하도록 설정
        float randomY = transform.position.y;

        return new Vector3(randomX, randomY, randomZ);
    }
}
