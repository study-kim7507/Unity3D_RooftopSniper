using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5.0f;
    [SerializeField]
    private float speed;        // �Ѿ��� ���ư��� �ӵ�
    [HideInInspector]
    public Vector3 Direction;   // �Ѿ��� ���ư��� ����

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void Move()
    {
        Vector3 moveDirection = Direction.normalized * speed * Time.deltaTime;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, moveDirection.magnitude))
        {
            if (hit.collider.CompareTag("Target"))
            {
                // �ǰ��� �Ծ��� ���
                hit.collider.gameObject.GetComponentInParent<TargetController>().OnDamaged();
            }
            Destroy(gameObject); 
        }
        transform.position += moveDirection;
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }

}
