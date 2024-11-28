using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    private GameObject renderCamera;
    private bool isSelect = false;

    private void Awake()
    {
        renderCamera.SetActive(false);
    }

    public void Selected()  // 현재 오브젝트가 타겟으로 선정되었음
    {
        isSelect = true;
        renderCamera.SetActive(true);
    }

    public void OnDamaged() // 피격을 당했을 경우
    {
        // TODO: 셰이더를 이용한 분기에 따른 다른 효과가 나타나도록
        // TODO: 점수 관리 -> 잘못된 타겟을 죽였을 경우, 올바른 타겟을 죽였을 경우

        if (!isSelect)
        {
            // 타겟이 아닌 사람을 죽임
        }
        else
        {
            // 올바른 타겟을 죽임 
            
        }

        GamaManager.Instance.GenerateNewTarget(gameObject);   // 새로운 타겟 생성
        Destroy(gameObject);
    }
}
