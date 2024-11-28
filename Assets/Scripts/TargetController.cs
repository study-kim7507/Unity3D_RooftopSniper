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

    public void Selected()  // ���� ������Ʈ�� Ÿ������ �����Ǿ���
    {
        isSelect = true;
        renderCamera.SetActive(true);
    }

    public void OnDamaged() // �ǰ��� ������ ���
    {
        // TODO: ���̴��� �̿��� �б⿡ ���� �ٸ� ȿ���� ��Ÿ������
        // TODO: ���� ���� -> �߸��� Ÿ���� �׿��� ���, �ùٸ� Ÿ���� �׿��� ���

        if (!isSelect)
        {
            // Ÿ���� �ƴ� ����� ����
        }
        else
        {
            // �ùٸ� Ÿ���� ���� 
            
        }

        GamaManager.Instance.GenerateNewTarget(gameObject);   // ���ο� Ÿ�� ����
        Destroy(gameObject);
    }
}
