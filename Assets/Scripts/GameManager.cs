/*
 * TODO: 사람, 경찰 다양한 색상
 * TODO: 네브 메시 내에서의 이동 및 애니메이션
 *        - 발각 시 사람들은 도망가고, 위장 경찰은 현재 내가 있는 건물을 향해 이동하도록
 * TODO: 아무런 사람이 없는 상태에서 새로운 사람이 등장하면 타겟으로 지정되지 않는 문제 수정해야함
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

    public List<NavMeshSurface> NavMeshSurfacesForPeople;
    public NavMeshSurface NavMeshSurfaceForPolice;

    [HideInInspector]
    public bool IsBulletCameraActive = false;
    [HideInInspector]
    public bool IsPoliceCameraActive = false;

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
    [UnityEngine.Range(3.0f, 15.0f)]
    private float timeIntervalForGenerateNewPeople = 10.0f;

    private Camera playerCamera;
    private float timerForGenerateNewPeople = 0.0f;
    
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


        // 게임 시작 시 최초 타겟 생성
        int randomIndex = UnityEngine.Random.Range(0, personObjects.Count);
        GameObject newTarget = personObjects[randomIndex];
        newTarget.GetComponent<TargetController>().Selected();
    }

    private void Update()
    {
        timerForGenerateNewPeople += Time.deltaTime;

        if (timerForGenerateNewPeople >= timeIntervalForGenerateNewPeople)
        {
            GenerateNewPerson();
            timerForGenerateNewPeople = 0.0f;
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

        if (personObjects.Count <= 0) return;

        if (isPlayerKilledCorrectTarget)
        {
            int randomIndex = UnityEngine.Random.Range(0, personObjects.Count);

            GameObject newTarget = personObjects[randomIndex];
            newTarget.GetComponent<TargetController>().Selected();
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

            Time.timeScale = hitObject != null ? 0.0125f : 0.3f;
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
                // TODO: 1. 위장 경찰의 등장 - 이미 발각 되어진 상태라면 코루틴 수행되지 않도록
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
