/*
 * TODO: �� ������ �� UI ������
 * TODO: ���� ����Ʈ
 * TODO: TargetController GetRandomPositionInNavMeshSurface ���� �ʿ�
 * TODO: �ٸ� ������Ʈ��� Destination�� ��ġ�� ���� ���� �ʿ� -> ���� �ð� ���� ���� ���ϸ� ��� ��Ű�� ������
 */
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
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
    [HideInInspector]
    public bool IsPlayerCaught = false;

    [HideInInspector]
    public bool IsGameCleared = false;
    [HideInInspector]
    public bool IsGameEnded = false;
    [HideInInspector]
    public bool IsGamePaused = false;

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
    public int TargetKillCount = 0;
    public int PoliceKillCount = 0;
    [SerializeField]
    [UnityEngine.Range(0.0f, 100.0f)]
    private float probabilityOfDetection = 50.0f;                   // �߰��� Ȯ��
    [SerializeField]
    [UnityEngine.Range(3.0f, 30.0f)]
    private float timeIntervalForGenerateNewPeople = 10.0f;
    public Transform PoliceGoalTransform;

    [Header("UI")]
    public TMP_Text RemainingTimeText;
    public GameObject Tooltip;
    [SerializeField]
    private GameObject playerUI;
    [SerializeField]
    private GameObject GameOverPopupCanvas;
    [SerializeField]
    private GameObject PauseMenuPopupCanvas;

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
        // ���� ���� Ȥ�� ���� �Ͻ����� ��
        if (IsGameEnded || IsGamePaused) return;


        timerForGenerateNewPeople += Time.deltaTime;
        if (timerForGenerateNewPeople >= timeIntervalForGenerateNewPeople)
        {
            GenerateNewPerson();
            timerForGenerateNewPeople = 0.0f;
        }

        InitializeAndAssignNewTarget();


        // UI
        if (Time.timeScale == 1.0f && RemainingTime >= 0.0f) RemainingTime -= Time.deltaTime;
        else if (RemainingTime < 0.0f) GameClear();
        RemainingTimeText.text =  "Remaining Time : " + Mathf.FloorToInt(RemainingTime / 60) + "m " + Mathf.FloorToInt(RemainingTime % 60) + "s";

        if (Life < 0) GameOver();
    }

    public void GameClear()
    {
        IsGameEnded = true;
        IsGameCleared = true;

        // ���콺 Ŀ���� ���̰�
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0.0f;

        playerUI.SetActive(false);
        GameOverPopupCanvas.SetActive(true);
    }

    public void GameOver()
    {
        IsGameEnded = true;
        IsGameCleared = false;

        // ���콺 Ŀ���� ���̰�
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0.0f;

        playerUI.SetActive(false);
        GameOverPopupCanvas.SetActive(true);
    }

    public void GamePause()
    {
        IsGamePaused = true;

        // ���콺 Ŀ���� ���̰�
        UnityEngine.Cursor.visible = true;
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0.0f;

        playerUI.SetActive(false);
        PauseMenuPopupCanvas.SetActive(true);
    }

    public void GameResume()
    {
        IsGamePaused = false;

        // ���콺 Ŀ���� ������ �ʰ�
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;

        PauseMenuPopupCanvas.SetActive(false);
        playerUI.SetActive(true);
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
            
            if (isPlayerKilledPolice && !IsPlayerExposure && !IsPlayerCaught)
            {
                // ���� �÷��̾ ���� ������ ���� ���
                Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("���� ������ �����Ͽ����ϴ�. �߰����� ������ ���� �� �ְ� �˴ϴ�.");
                
                GenerateNewPolice();
            }
            else if (isPlayerKilledPolice && IsPlayerExposure && !IsPlayerCaught)
            {
                Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("�����Դϴ�. �������� ���� ������ �����Ͽ����ϴ�.");

                GenerateNewPolice();
            }
            else if (isPlayerKilledPolice && IsPlayerExposure && IsPlayerCaught)
            {
                // ���� �÷��̾ �߰��ǰ� �����κ��� ������ ���
                Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("�������� ���������ϴ�. ����� �ϳ� �����մϴ�.");

                GenerateNewPolice();
            }
            else
            {
                Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("�Ƿڹ��� Ÿ���� �����Ͽ����ϴ�.");
                Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("���ο� Ÿ���� �����˴ϴ�.");
            }
        }
        else
        {
            // �÷��̾ Ÿ���� �ƴ� �ٸ� ���� ���� �Ѿ��� �߻��� (Ÿ���� �ƴ� ����� ����)
            // ���� Ȯ���� ���� �߰� ���θ� ����

            if (UnityEngine.Random.Range(0.0f, 100.0f) <= probabilityOfDetection)
            {
                if (!IsPlayerExposure)
                {
                    NumberOfDetections++;                   // �߰� Ƚ�� ����
                    IsPlayerExposure = true;
                    Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("�߰� �Ǿ����ϴ�. ������ ����� �巯���ϴ�");
                    Tooltip.GetComponent<GameStatusTooltipDisplayer>().DisplayTooltip("������ �켱������ �����Ͽ� ������ �ʵ��� �ϼ���!");
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
        // Debug.Log("���ο� ����� �����Ǿ����ϴ�");

        int random = (int)UnityEngine.Random.Range(0, personPrefabs.Count);
        GameObject newPerson = Instantiate(personPrefabs[random]);
        personObjects.Add(newPerson);
    }

    private void GenerateNewPolice()
    {
        // Debug.Log("���ο� ���� ������ �����Ǿ����ϴ�.");

        int random = (int)UnityEngine.Random.Range(0, policePrefabs.Count);
        GameObject newPolice = Instantiate(policePrefabs[random]);
        policeObject = newPolice;
    }
}
