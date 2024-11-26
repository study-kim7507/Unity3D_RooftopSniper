using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private RotateToMouse rotateToMouse;
    private MovementCharacterController movementCharacterController;
    private void Awake()
    {
        // 마우스 커서를 보이지 않게 설정
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rotateToMouse = GetComponent<RotateToMouse>(); 
        movementCharacterController = GetComponent<MovementCharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotate();
        UpdateMove();
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

        movementCharacterController.MoveTo(new Vector3(x, 0, z));
    }
}
