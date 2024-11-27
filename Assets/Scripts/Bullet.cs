using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed;        // �Ѿ��� ���ư��� �ӵ�

    [HideInInspector]
    public Vector3 Direction;   // �Ѿ��� ���ư��� ����

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        // ���� ���͸� ����ȭ�ؼ� �ӵ��� ���� �Ѿ��� �̵��ϵ���
        transform.position += Direction.normalized * speed * Time.deltaTime;
    }
    
}
