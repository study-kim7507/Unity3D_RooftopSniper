using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private RotateToMouse rotateToMouse;                                // ���콺 �̵����� ī�޶� ȸ��
    private MovementCharacterController movementCharacterController;    // Ű���� �Է����� �÷��̾� �̵�, ���� ��
    private Status status;                                              // �̵��ӵ� ���� �÷��̾� ���� 
    private PlayerAnimatorController playerAnimatorController;          // �ִϸ��̼� ��� ����
    private AudioSource audioSource;                                    // ���� ��� ����

    private bool isZoomed = false;

    private void Awake()
    {
        // ���콺 Ŀ���� ������ �ʰ� ����
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>(); 
        movementCharacterController = GetComponent<MovementCharacterController>();
        status = GetComponent<Status>();
        playerAnimatorController = GetComponent<PlayerAnimatorController>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSnipingStatus();
        UpdateRotate();
        UpdateMove();
        FireWithProjectile();
    }

    // ���콺 �Է� (ĳ���� ȸ���� ���)
    private void UpdateRotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

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

            movementCharacterController.MoveSpeed = isRun == true ? status.RunSpeed : status.WalkSpeed;
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
            movementCharacterController.MoveSpeed = 0.0f;
            playerAnimatorController.MoveSpeed = 0.0f;

            // ������ �� ���尡 ������̸� ����
            if (audioSource.isPlaying == true)
            {
                audioSource.Stop();
            }
        }

        movementCharacterController.MoveTo(new Vector3(x, 0, z));
    }

    // �÷��̾��� ���� ���� ������Ʈ
    private void UpdateSnipingStatus()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isZoomed = !isZoomed;
            playerAnimatorController.ToggleSniperMode();
            weapon.GetComponent<WeaponSniperRifle>().ToggleMode();
        }
    }

    private void FireWithProjectile()
    {
        if (Input.GetMouseButtonDown(0))
        { 
            weapon.GetComponent<WeaponSniperRifle>().Fire();

            if (!isZoomed)
            {
                playerAnimatorController.Fire();        // �� ���°� �ƴϸ� �� ��� �ִϸ��̼� ���
            }
            else
            {
                // TODO: �ѱ� �ݵ�
            }
        }
    }
}
