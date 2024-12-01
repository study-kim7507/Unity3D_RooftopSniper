using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class PersonController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshSurface NavMeshSurface;      // ���� ���� navMeshSurface

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
            // Ÿ���� �ƴ� ����� ����
            // ����� �� ������ ����Ʈ
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor("_EffectColor", Color.red);
            }
        }
        else
        {
            // �ùٸ� Ÿ���� ���� 
            // ����� �� �ʷϻ� ����Ʈ
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
        // NavMeshSurface�� ������� ���͸� ����Ͽ� ���� ������ ���
        Vector3 center = NavMeshSurface.transform.position + NavMeshSurface.center;
        Vector3 size = NavMeshSurface.size;

        // ���� ������ ��� (�������� ������ ���� ������ ����)
        float randomX = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float randomZ = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        // Y���� ���� ������Ʈ�� Y���� �����ϵ��� ����
        float randomY = transform.position.y;

        return new Vector3(randomX, randomY, randomZ);
    }
}
