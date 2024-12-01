using UnityEngine;
using System.Collections;

public class PoliceController : MonoBehaviour
{
    [SerializeField]
    private Mesh policeMesh;

    [SerializeField]
    private GameObject policeCamera;

    private Renderer[] renderers;

    private void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

    }
    
    // 위장 경찰의 등장
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

        // 위장 경찰의 카메라 (메인 카메라)를 비활성화
        policeCamera.SetActive(false);

        // 플레이어 카메라 활성화
        GameManager.Instance.DeactivatePoliceCamera();
    }
}
