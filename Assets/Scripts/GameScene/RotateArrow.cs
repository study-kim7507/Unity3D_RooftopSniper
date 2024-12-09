using UnityEngine;

public class RotateArrow : MonoBehaviour
{
    // 회전 속도
    public float rotationSpeed = 50f;

    void Update()
    {
        // y축을 중심으로 회전
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
