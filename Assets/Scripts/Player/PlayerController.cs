using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsZoomed = false;

    [Header("Input KeyCodes")]
    [SerializeField]
    private KeyCode keyCodeRun = KeyCode.LeftShift;                     // 달리기 키

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip audioClipWalk;                                    // 걷기 사운드
    [SerializeField]
    private AudioClip audioClipRun;                                     // 달리기 사운드

    [Header("Weapon")]
    [SerializeField]
    private GameObject weapon;

    private RotateToMouse rotateToMouse;                                // 마우스 이동으로 카메라 회전
    private PlayerMovementController playerMovementController;          // 키보드 입력으로 플레이어 이동, 점프 등
    private Status status;                                              // 이동속도 등의 플레이어 정보 
    private PlayerAnimatorController playerAnimatorController;          // 애니메이션 재생 제어
    private AudioSource audioSource;                                    // 사운드 재생 제어

    private bool canFire = true;                                        // 현재 총을 발사할 수 있는지 (쿨타임)

    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정
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

        UpdateSnipingStatus();
        UpdateRotate();
        UpdateMove();
        FireWithProjectile();
        UpdateWeaponFOV();
    }

    // 마우스 입력 (캐릭터 회전을 담당)
    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotateToMouse.UpdateRotate(mouseX, mouseY);
    }

    // 키보드 입력 (앞/뒤/좌/우 캐릭터 이동을 담당)
    private void UpdateMove()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 이동 중
        if ( x != 0 || z != 0)
        {
            bool isRun = false;

            // 옆이나 뒤로 이동할 때는 달릴 수 없도록
            if (z > 0) isRun = Input.GetKey(keyCodeRun);

            playerMovementController.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
            playerAnimatorController.MoveSpeed = isRun == true ? 1.0f : 0.5f;       // animator의 파라미터 값을 조절하여 Blend 애니메이션 재생 조절
            audioSource.clip = isRun == true ? audioClipRun : audioClipWalk;

            // 방향키 입력 여부는 매 프레임 확인하기 때문에
            // 재생중일 때는 다시 재생하지 않도록 isPlaying으로 체크해서 재생
            if (audioSource.isPlaying == false)
            {
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        // 제자리에 멈춰 있을 때
        else
        {
            playerMovementController.MoveSpeed = 0.0f;
            playerAnimatorController.MoveSpeed = 0.0f;

            // 멈췄을 때 사운드가 재생중이면 정지
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        playerMovementController.MoveTo(new Vector3(x, 0, z));
    }

    // 플레이어의 저격 상태 업데이트
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

            // 코루틴을 통한 총 발사 쿨타임을 적용
            canFire = false;
            StartCoroutine(ResetFireCooldown());        

            if (!IsZoomed)
            {
                playerAnimatorController.Fire();        // 줌 상태가 아니면 총 쏘는 애니메이션 재생
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
            // 휠 업
            weapon.GetComponent<WeaponSniperRifle>().SetFOV(true);
        }
        else if (wheelInput < 0)
        {
            // 휠 다운
            weapon.GetComponent<WeaponSniperRifle>().SetFOV(false);
        }
    }
}
