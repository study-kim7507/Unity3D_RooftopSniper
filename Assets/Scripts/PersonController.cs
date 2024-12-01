using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class PersonController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshSurface NavMeshSurface;      // 현재 속한 navMeshSurface

    public bool IsSelect = false;
    public bool IsPolice = false;

    protected Renderer[] renderers;
    protected NavMeshAgent navMeshAgent;


    protected virtual void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void OnDamaged()
    {
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

    protected IEnumerator DestroyRoutine()
    {
        float x = 1.0f;
        while (x >= -1.0f)
        {
            x -= Time.deltaTime * 0.7f;

            foreach (Renderer render in renderers)
            {
                render.material.SetFloat("_AppearAmount", x);
            }

            yield return null;
        }

        Destroy(gameObject);
        GameManager.Instance.AssignNewTargetIfCorrect(gameObject);   
    }


    protected Vector3 GetRandomPositionInNavMeshSurface()
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
