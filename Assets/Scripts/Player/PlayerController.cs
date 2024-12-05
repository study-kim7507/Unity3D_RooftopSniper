using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsZoomed = false;

    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift;                     // �޸��� Ű

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                                    // �ȱ� ����
    [SerializeField]
    private AudioClip audioClipRun;                                     // �޸��� ����

    [Header("Weapon")]
    [SerializeField]
    private GameObject weapon;

    private float mouseSensitivity = 2.0f;
    private RotateToMouse rotateToMouse;                                // ���콺 �̵����� ī�޶� ȸ��
    private PlayerMovementController playerMovementController;          // Ű���� �Է����� �÷��̾� �̵�, ���� ��
    private Status status;                                              // �̵��ӵ� ���� �÷��̾� ���� 
    private PlayerAnimatorController playerAnimatorController;          // �ִϸ��̼� ��� ����
    private AudioSource audioSource;                                    // ���� ��� ����

    private bool canFire = true;                                        // ���� ���� �߻��� �� �ִ��� (��Ÿ��)
    
    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� ����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>();
        playerMovementController = GetComponent<PlayerMovementController>();
        status = GetComponent<Status>();
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsBulletCameraActive || GameManager.Instance.IsPoliceCameraActive)
        {
            audioSource.Stop();
            return;
        }

        
        if (GameManager.Instance.IsGameEnded) return;
        PlayerPauseGame();

        if (GameManager.Instance.IsGamePaused) return;
        UpdateSnipingStatus();
        UpdateMouseSensitivity();
        UpdateRotate();
        UpdateMove();
        FireWithProjectile();
        UpdateWeaponFOV();
    }

    private void PlayerPauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.Instance.IsGamePaused) GameManager.Instance.GamePause();
        else if (Input.GetKeyDown(KeyCode.Escape) && GameManager.Instance.IsGamePaused) GameManager.Instance.GameResume();
    }

    private void UpdateMouseSensitivity()
    {
        // Ű �Է¿� ���� ���콺 ���� ����
        if (Input.GetKeyDown(KeyCode.LeftBracket)) mouseSensitivity -= 0.1f; // ������ �� �����ϰ� ����
        else if (Input.GetKeyDown(KeyCode.RightBracket)) mouseSensitivity += 0.1f;

        // ������ 0.1 ~ 5.0 ������ ����
        mouseSensitivity = Mathf.Clamp(mouseSensitivity, 0.1f, 5.0f);
    }

    // ���콺 �Է� (ĳ���� ȸ���� ���)
    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // ���콺 ���� ����
        mouseX *= mouseSensitivity;
        mouseY *= mouseSensitivity;

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    // Ű���� �Է� (��/��/��/�� ĳ���� �̵��� ���)
    private void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // �̵� ��
        if ( x != 0 || z != 0)
        {
            bool isRun = false;

            // ���̳� �ڷ� �̵��� ���� �޸� �� ������
            if (z > 0) isRun = Input.GetKey(keyCodeRun);

            playerMovementController.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            playerAnimatorController.MoveSpeed = isRun == true ? 1.0f : 0.5f;       // animator�� �Ķ���� ���� �����Ͽ� Blend �ִϸ��̼� ��� ����
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            // ����Ű �Է� ���δ� �� ������ Ȯ���ϱ� ������
            // ������� ���� �ٽ� ������� �ʵ��� isPlaying���� üũ�ؼ� ���
            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        // ���ڸ��� ���� ���� ��
        else
        {
            playerMovementController.MoveSpeed = 0.0f;
            playerAnimatorController.MoveSpeed = 0.0f;

            // ������ �� ���尡 ������̸� ����
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        playerMovementController.MoveTo(new Vector3(x, 0, z));
    }

    // �÷��̾��� ���� ���� ������Ʈ
    private void UpdateSnipingStatus()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsZoomed = !IsZoomed;
            weapon.GetComponent<WeaponSniperRifle>().ToggleMode();
        }
    }

    private void FireWithProjectile()
    {
        if (Input.GetMouseButtonDown(0) && canFire)
        { 
            weapon.GetComponent<WeaponSniperRifle>().Fire();

            // �ڷ�ƾ�� ���� �� �߻� ��Ÿ���� ����
            canFire = false;
            StartCoroutine(ResetFireCooldown());        

            if (!IsZoomed)
            {
                playerAnimatorController.Fire();        // �� ���°� �ƴϸ� �� ��� �ִϸ��̼� ���
            }
        }
    }

    private IEnumerator ResetFireCooldown()
    {
        yield return new WaitForSecondsRealtime(3.0f);

        canFire = true;
    }

    private void UpdateWeaponFOV()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput > 0)
        {
            // �� ��
            weapon.GetComponent<WeaponSniperRifle>().SetFOV(true);
        }
        else if (wheelInput < 0)
        {
            // �� �ٿ�
            weapon.GetComponent<WeaponSniperRifle>().SetFOV(false);
        }
    }
}
