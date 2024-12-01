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
    public NavMeshSurface NavMeshSurface;      // ���� ���� navMeshSurface

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

   

    public void Selected()  // ���� ������Ʈ�� Ÿ������ �����Ǿ���
    {
        IsSelect = true;
        renderCamera.SetActive(true);
    }

    public void OnDamaged() // �ǰ��� ������ ���
    {
        // TODO: ���� ���� -> �߸��� Ÿ���� �׿��� ���, �ùٸ� Ÿ���� �׿��� ���

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
        
        GameManager.Instance.AssignNewTargetIfCorrect(gameObject);   // ���ο� Ÿ�� ����  
    }

    private Vector3 GetRandomPositionInNavMeshSurface()
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
