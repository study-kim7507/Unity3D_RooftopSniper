using UnityEngine;

public class PoliceGoalPoint : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Police"))
        {
            GameManager.Instance.Life--;                        // �÷��̾ �������� ������ ��� 1�� ����
            GameManager.Instance.IsPlayerCaught = true;
            collision.gameObject.GetComponentInParent<PersonController>().OnDamaged();
        }
    }
}
