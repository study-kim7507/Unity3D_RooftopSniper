using NUnit.Framework.Constraints;
using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    private GameObject renderCamera;

    [HideInInspector]
    public bool IsSelect = false;

    [HideInInspector]
    public NavMeshSurface NavMeshSurface;      // 현재 속한 navMeshSurface

    public bool IsPolice = false;

    private Renderer[] renderers;
    
    private TargetAnimatorController targetAnimatorController;
    private TargetMovementController targetMovementController;
    private NavMeshAgent navMeshAgent;
    

    private void Awake()
    {
        if (renderCamera != null)
        {
            renderCamera.SetActive(false);
        }

        renderers = GetComponentsInChildren<Renderer>();
        targetAnimatorController = GetComponent<TargetAnimatorController>();
        targetMovementController = GetComponent<TargetMovementController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

   

    public void Selected()  // 현재 오브젝트가 타겟으로 선정되었음
    {
        IsSelect = true;
        renderCamera.SetActive(true);
    }

    public void OnDamaged() // 피격을 당했을 경우
    {
        // TODO: 점수 관리 -> 잘못된 타겟을 죽였을 경우, 올바른 타겟을 죽였을 경우

        if (!IsSelect && !IsPolice)
        {
            // 타겟이 아닌 사람을 죽임
            // 사라질 때 붉은색 이펙트
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor("_EffectColor", Color.red);
            }
        }
        else
        {
            // 올바른 타겟을 죽임 
            // 사라질 때 초록색 이펙트
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor("_EffectColor", Color.green);
            }
        }

        StartCoroutine(DestroyRoutine());
    }


    private IEnumerator DestroyRoutine()
    { 
        float x = 1.0f;
        while(x >= -1.0f)
        {
            x -= Time.deltaTime * 0.7f;
            
            foreach (var render in renderers)
            {
                render.material.SetFloat("_AppearAmount", x);
            }

            yield return null;
        }

        Destroy(gameObject);
        
        GameManager.Instance.AssignNewTargetIfCorrect(gameObject);   // 새로운 타겟 생성  
    }

    private Vector3 GetRandomPositionInNavMeshSurface()
    {
        // NavMeshSurface의 사이즈와 센터를 사용하여 랜덤 포지션 계산
        Vector3 center = NavMeshSurface.transform.position + NavMeshSurface.center;
        Vector3 size = NavMeshSurface.size;

        // 랜덤 포지션 계산 (사이즈의 절반을 빼서 범위를 맞춤)
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Y축은 현재 오브젝트의 Y축을 유지하도록 설정
        float randomY = transform.position.y;

        return new Vector3(randomX, randomY, randomZ);
    }
}
