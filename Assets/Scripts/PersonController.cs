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
            // Ÿ���� �ƴ� ����� ����
            GameManager.Instance.Reward -= 50.0f;

            // ����� �� ������ ����Ʈ
            foreach (Renderer renderer in renderers)
            {
                renderer.material.SetColor("_EffectColor", Color.red);
            }
        }
        else
        {
            // �ùٸ� Ÿ���� ����
            if (IsSelect) GameManager.Instance.Reward += 100.0f;
            if (!GameManager.Instance.IsPlayerExposure && IsPolice) GameManager.Instance.Reward += 200.0f;

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


    protected abstract Vector3 GetRandomPositionInNavMeshSurface();

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
