using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [Header("Camera Rotation Speeds")]
    [SerializeField]
    private float rotCamXAxisSpeed = 5.0f;  // ī�޶� x�� ȸ���ӵ�
    [SerializeField]
    private float rotCamYAxisSpeed = 3.0f;  // ī�޶� y�� ȸ���ӵ�

    private float limitMinX = -90.0f;   // ī�޶� x�� ȸ�� ���� (�ּ�)
    private float limitMaxX = 90.0f;    // ī�޶� x�� ȸ�� ���� (�ִ�)
    private float eulerAngleX;
    private float eulerAngleY;

    public void UpdateRotate(float mouseX, float mouseY)
    {
        eulerAngleY += mouseX * rotCamYAxisSpeed;   // ���콺 ��/�� �̵����� ī�޶� y�� ȸ��
        eulerAngleX -= mouseY * rotCamXAxisSpeed;   // ���콺 ��/�Ʒ� �̵����� ī�޶� x�� ȸ��

        // ī�޶� x�� ȸ���� ��� ȸ�� ������ ����
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
