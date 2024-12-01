using UnityEngine;
using System.Collections;

public class PoliceController : PersonController
{
    [Header("Police Mesh")]
    [SerializeField]
    private Mesh policeMesh;

    [Header("Police Camera")]
    [SerializeField]
    private GameObject policeCamera;

    private PoliceAnimatorController policeAnimatorController;
    private PoliceMovementController policeMovementController;

    protected override void Awake()
    {
        base.Awake();

        NavMeshSurface = GameManager.Instance.NavMeshSurfaceForPolice;
        gameObject.transform.position = GetRandomPositionInNavMeshSurface();
    }

    // ���� ������ ����
    public IEnumerator CamouflagePoliceAppearRoutine()
    {
        policeCamera.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        StartCoroutine(ChangeMeshRoutine());
    }

    public IEnumerator ChangeMeshRoutine()
    {
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh = policeMesh;

        float x = -1.0f;
        while (x <= 1.0f)
        {
            x += Time.deltaTime * 0.7f;

            foreach (var render in renderers)
            {
                render.material.SetColor("_EffectColor", Color.blue);
                render.material.SetFloat("_AppearAmount", x);
            }

            yield return null;
        }

        // ���� ������ ī�޶� (���� ī�޶�)�� ��Ȱ��ȭ
        policeCamera.SetActive(false);

        // �÷��̾� ī�޶� Ȱ��ȭ
        GameManager.Instance.DeactivatePoliceCamera();
    }
}
