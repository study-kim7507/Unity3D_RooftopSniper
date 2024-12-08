using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Direction;   // �Ѿ��� ���ư��� ����

    [Header("Bullet Properties")]
    [SerializeField]
    public float LifeTime = 3.0f;
    [SerializeField]
    public float Speed;         // �Ѿ��� ���ư��� �ӵ�

    private LayerMask layerMask;

    private void Start()
    {
        Vector3 moveDirection = Direction.normalized * Speed * LifeTime;

        // ������ LayerMask ����
        layerMask = ~(1 << LayerMask.NameToLayer("PlayerBlockWall"));

        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, moveDirection.magnitude, layerMask))
        {
            GameManager.Instance.ActivateBulletCamera(hit.collider.gameObject);
            StartCoroutine(DestroyAfterTime(true, hit.collider.gameObject));
        }
        else
        { 
            GameManager.Instance.ActivateBulletCamera(null);
            StartCoroutine(DestroyAfterTime(false, null));
        }
    }


    private void Update()
    {
        Move();
    }

    public void Move()
    {
        Vector3 moveDirection = Direction.normalized * Speed * Time.fixedDeltaTime;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, moveDirection.magnitude, layerMask))
        {
            if (hit.collider.CompareTag("Target") || hit.collider.CompareTag("Police"))
            {
                // �ǰ��� �Ծ��� ���
                hit.collider.gameObject.GetComponentInParent<PersonController>().OnDamaged();

                GameManager.Instance.DeactivateBulletCamera();
                Destroy(gameObject);
            }
            else
            {
                GameManager.Instance.DeactivateBulletCamera();
                Destroy(gameObject);

                GameManager.Instance.AssignNewTargetIfCorrect(null);
            }
        }
        transform.position += moveDirection;
    }

    private IEnumerator DestroyAfterTime(bool isHit, GameObject hitObject)
    {
        if (isHit) yield return new WaitForSeconds(LifeTime);
        else yield return new WaitForSecondsRealtime(LifeTime);


        GameManager.Instance.DeactivateBulletCamera();
        Destroy(gameObject);

        if ((hitObject != null && !(hitObject.CompareTag("Target") || hitObject.CompareTag("Police"))) || (hitObject == null))
            GameManager.Instance.AssignNewTargetIfCorrect(null);
    }
}
