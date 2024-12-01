using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public float LifeTime = 3.0f;
    [SerializeField]
    public float Speed;        // 총알이 날아가는 속도
    [HideInInspector]
    public Vector3 Direction;   // 총알이 날아가는 방향

    private void Start()
    {
        Vector3 moveDirection = Direction.normalized * Speed * LifeTime;
        RaycastHit hit;

        bool isPlayerKillCorrectTarget = false;
        bool isPlayerKillPolice = false;
        if (Physics.Raycast(transform.position, moveDirection, out hit, moveDirection.magnitude))
        {
            /* For Debugging */
            Debug.DrawLine(transform.position, hit.point, Color.green, LifeTime);

            if (hit.collider.CompareTag("Target") || hit.collider.CompareTag("Police"))
            {
                isPlayerKillCorrectTarget = hit.collider.gameObject.GetComponentInParent<TargetController>().IsSelect ? true : false;
                isPlayerKillPolice = hit.collider.gameObject.GetComponentInParent<TargetController>().IsPolice ? true : false;
                GameManager.Instance.ActivateBulletCameraAndSetResult(isPlayerKillCorrectTarget, isPlayerKillPolice);   
            }
            else
            { 
                GameManager.Instance.ActivateBulletCameraAndSetResult(isPlayerKillCorrectTarget, isPlayerKillPolice);
            }
            
        }
        else
        { 
            GameManager.Instance.ActivateBulletCameraAndSetResult(isPlayerKillCorrectTarget, isPlayerKillPolice);
        }

        StartCoroutine(DestroyAfterTime(isPlayerKillCorrectTarget, isPlayerKillPolice));
    }


    private void Update()
    {
        Move();
    }

    public void Move()
    {
        Vector3 moveDirection = Direction.normalized * Speed * Time.fixedDeltaTime;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, moveDirection, out hit, moveDirection.magnitude))
        {
            if (hit.collider.CompareTag("Target") || hit.collider.CompareTag("Police"))
            {
                // 피격을 입었을 경우
                hit.collider.gameObject.GetComponentInParent<TargetController>().OnDamaged();
            }

            GameManager.Instance.DeactivateBulletCamera();
            Destroy(gameObject); 

        }
        transform.position += moveDirection;
    }

    private IEnumerator DestroyAfterTime(bool isPlayerKillCorrectTarget, bool isPlayerKillPolice)
    {
        if (isPlayerKillCorrectTarget || isPlayerKillPolice)
        {
            yield return new WaitForSeconds(LifeTime);
        }
        else
        {
            yield return new WaitForSecondsRealtime(LifeTime);
        }
        
        GameManager.Instance.DeactivateBulletCamera();
        Destroy(gameObject);
    }
}
