using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

public abstract class PersonController : MonoBehaviour
{
    [HideInInspector]
    public NavMeshSurface NavMeshSurface;      // 현재 속한 navMeshSurface
    [HideInInspector]
    public NavMeshAgent NavMeshAgent;

    public bool IsSelect = false;
    public bool IsPolice = false;

    [Header("Render Camera")]
    [SerializeField]
    protected GameObject renderCamera;
    [SerializeField]
    protected GameObject renderTarget;

    protected Renderer[] renderers;

    protected virtual void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.speed = 0.5f;
    }

    public void OnDamaged()
    {
        if (!IsSelect && !IsPolice)
        {
            // 사라질 때 붉은색 이펙트
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor("_EffectColor", Color.red);
            }
        }
        else
        {
            // 올바른 타겟을 죽임
            if (IsSelect) GameManager.Instance.TargetKillCount++;
            if (!GameManager.Instance.IsPlayerExposure && IsPolice) GameManager.Instance.PoliceKillCount++;

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
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 50.0f; // 거리 줄이기
            randomDirection += gameObject.transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 75.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return new Vector3(0, 1, 0);
    }

    protected void SetRenderTargetLayerAndSetRenderCameraCullingMask(string nameOfLayer)
    {
        int renderLayer = LayerMask.NameToLayer(nameOfLayer);

        renderTarget.layer = renderLayer;
        foreach (Transform renderTargetChildObjectTransform in renderTarget.transform)
        {
            renderTargetChildObjectTransform.gameObject.layer = renderLayer;
        }

        int cullingMask = 1 << renderLayer;
        if (nameOfLayer == "Police")
        {
            cullingMask |= (1 << LayerMask.NameToLayer("Ground"));
            cullingMask |= (1 << LayerMask.NameToLayer("Building"));
        }
        
        renderCamera.GetComponent<Camera>().cullingMask = cullingMask; 
    }
}
