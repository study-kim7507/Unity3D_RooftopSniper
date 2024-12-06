using UnityEngine;

public class PoliceGoalPoint : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Police"))
        {
            GameManager.Instance.Life--;                        // 플레이어가 경찰에게 붙잡힘 목숨 1개 감소
            GameManager.Instance.IsPlayerCaught = true;
            collision.gameObject.GetComponentInParent<PersonController>().OnDamaged();
        }
    }
}
