using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float lifeTime = 5.0f;
    [SerializeField]
    private float speed;        // 총알이 날아가는 속도
    [HideInInspector]
    public Vector3 Direction;   // 총알이 날아가는 방향

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
                // 피격을 입었을 경우
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
