/*
 * 
 * TODO : Target(People) 초기 스폰 위치 설정 문제 해결 필요(NavMeshSurface, NavMeshAgent -> AreaMask 문제)
 *        -> GetRandomPositionInNavMeshSurface 함수 수정 필요 
 * TODO : NavMeshAgent의 Destination에 다른 오브젝트가 존재하면 새로운 랜덤 위치를 생성하도록
 * TODO : Police 경찰의 초기 스폰 위치 설정 문제 해결 필요
 * TODO : 경찰이 GoalPoint에 도착했을 때, 플레이어 목숨을 감소시키고 새로운 위장 경찰 등장 시키도록
 * TODO : 이미 발각된 상태에서 또 다시 발각되는 문제 해결 필요
 * TODO : 추가적인 Asset 임포트 필요
 *        -> 다양한 사람, 다양한 Idle, Walk, Run 애니메이션
 * 
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
    private List<GameObject> personObjects;                     // 씬에 존재하는 사람들 (이 중에서 랜덤한 사람이 매 시점 타겟으로 선정됨)

    [Header("Police Settings")]
    [SerializeField]
    private GameObject policePrefab;
    [SerializeField]
    private GameObject policeObject;

    [Header("Player")]
    [SerializeField]
    private GameObject player;
    
    [Header("Game Settings")]
    [SerializeField]
    [UnityEngine.Range(0.0f, 100.0f)]
    private float probabilityOfDetection = 50.0f;               // 발각될 확률
    [SerializeField]
    [UnityEngine.Range(3.0f, 30.0f)]
    private float timeIntervalForGenerateNewPeople = 10.0f;
    public Transform PoliceGoalTransform;

    private Camera playerCamera;
    private float timerForGenerateNewPeople = 0.0f;
    private GameObject currentTarget = null;
    
    
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

    // 게임 시작 후 초기 타겟 설정 및 아무런 사람이 없을 때 지속적으로 확인하여 새로운 사람이 등장하면 타겟으로 설정
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
        // 플레이어의 카메라(메인카메라)를 비활성화
        // 총알에 붙어있는 카메라가 메인카메라가 되도록하여 총알이 날아가는 장면을 보여줌

        if (playerCamera != null)
        {
            IsBulletCameraActive = true;
            playerCamera.gameObject.SetActive(false);

            if (player.GetComponent<PlayerController>().IsZoomed)
            {
                player.GetComponentInChildren<WeaponSniperRifle>().ToggleScopeOverlay();
            }

            Time.timeScale = hitObject != null ? 0.1f : 0.3f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public void DeactivateBulletCamera()
    {
        // 총알에 붙어있는 카메라(메인카메라)를 비활성화
        // 플레이어의 카메라가 메인카메라가 되도록하여 다시 플레이어를 시점으로
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

    // 발각 여부 결정
    private void EvaluateAndHandleExposure(bool isPlayerKilledCorrectTarget, bool isPlayerKilledPolice)
    {
        if (isPlayerKilledCorrectTarget || isPlayerKilledPolice)
        {   
            // 만약 플레이어가 위장 경찰을 죽인 경우 새로운 위장 경찰이 등장하도록
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
                IsPlayerExposure = true;
                
                // TODO: 1. 위장 경찰의 등장 - 이미 발각 되어진 상태라면 코루틴 수행되지 않도록
                ActivatePoliceCamera();
                StartCoroutine(policeObject.GetComponent<PoliceController>().CamouflagePoliceAppearRoutine());
                PoliceChasePlayer?.Invoke();

                // TODO: 2. 씬에 존재하는 사람들의 도망
                TargetRunAway?.Invoke();
                
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
        Debug.Log("새로운 사람이 생성되었습니다.");
        GameObject newPerson = Instantiate(personPrefabs[UnityEngine.Random.Range(0, personPrefabs.Count)]);
        personObjects.Add(newPerson);
    }

    private void GenerateNewPolice()
    {
        Debug.Log("새로운 위장 경찰이 생성되었습니다.");
        GameObject newPolice = Instantiate(policePrefab);
        policeObject = newPolice;
    }
}
