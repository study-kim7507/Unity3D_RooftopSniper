using UnityEngine;

public class RotateArrow : MonoBehaviour
{
    // ȸ�� �ӵ�
    public float rotationSpeed = 50f;

    void Update()
    {
        // y���� �߽����� ȸ��
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
