using UnityEngine;

public class PoliceGoalPoint : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Police"))
        {
            GameManager.Instance.Life--;                        // �÷��̾ �������� ������ ��� 1�� ����
            collision.gameObject.GetComponentInParent<PersonController>().OnDamaged();
        }
    }
}
