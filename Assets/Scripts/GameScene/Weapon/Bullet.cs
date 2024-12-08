using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public Vector3 Direction;   // 총알이 날아가는 방향

    [Header("Bullet Properties")]
    [SerializeField]
    public float LifeTime = 3.0f;
    [SerializeField]
    public float Speed;         // 총알이 날아가는 속도

    private LayerMask layerMask;

    private void Start()
    {
        Vector3 moveDirection = Direction.normalized * Speed * LifeTime;

        // 무시할 LayerMask 설정
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
                // 피격을 입었을 경우
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
