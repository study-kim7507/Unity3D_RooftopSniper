using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed;        // 총알이 날아가는 속도

    [HideInInspector]
    public Vector3 Direction;   // 총알이 날아가는 방향

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        // 방향 벡터를 정규화해서 속도에 곱해 총알이 이동하도록
        transform.position += Direction.normalized * speed * Time.deltaTime;
    }
    
}
