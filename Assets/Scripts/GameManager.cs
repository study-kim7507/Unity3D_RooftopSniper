/*
 * TODO: 씬 디자인 및 UI 디자인
 * TODO: 스폰 포인트
 * TODO: TargetController GetRandomPositionInNavMeshSurface 수정 필요
 * TODO: 다른 오브젝트들과 Destination이 겹치는 문제 수정 필요 -> 일정 시간 동안 도달 못하면 취소 시키는 식으로
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
    private List<GameObject> policePrefabs;
    [SerializeField]
    private GameObject policeObject;

    [Header("Player")]
    [SerializeField]
    private GameObject player;

    [Header("Game Settings")]
    public int Life = 3;                                            // 플레이어의 남은 목숨 수
    public int NumberOfDetections = 0;                              // 플레이어 발각 횟수
    public float RemainingTime = 180.0f;                            // 남은 시간
    public float Reward = 0.0f;                                     // 리워드
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

            Time.timeScale = hitObject != null ? 0.025f : 0.3f;
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
                NumberOfDetections++;                   // 발각 횟수 증가
                
                if (!IsPlayerExposure)
                {
                    IsPlayerExposure = true;
                    ActivatePoliceCamera();
                    StartCoroutine(policeObject.GetComponent<PoliceController>().CamouflagePoliceAppearRoutine());
                }
                PoliceChasePlayer?.Invoke();            // 경찰이 플레이어를 잡으러 이동함

                TargetRunAway?.Invoke();                // 사람들이 자신이 속한 NavMeshSurface 상에서 도망다님
                
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
        Debug.Log("새로운 사람이 생성되었습니다");

        int random = (int)UnityEngine.Random.Range(0, personPrefabs.Count);
        GameObject newPerson = Instantiate(personPrefabs[random]);
        personObjects.Add(newPerson);
    }

    private void GenerateNewPolice()
    {
        Debug.Log("새로운 위장 경찰이 생성되었습니다.");

        int random = (int)UnityEngine.Random.Range(0, policePrefabs.Count);
        GameObject newPolice = Instantiate(policePrefabs[random]);
        policeObject = newPolice;
    }
}
