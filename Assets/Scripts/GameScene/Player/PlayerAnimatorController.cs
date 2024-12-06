using System.Collections;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private void Awake()
    {
        // "Player" ������Ʈ �������� �ڽ� ������Ʈ�� 
        // "arms_SniperRifle" ������Ʈ�� Animator ������Ʈ�� ����.
        animator = GetComponentInChildren<Animator>();
    }

    public float MoveSpeed
    {
        set => animator.SetFloat("MovementSpeed", value);
        get => animator.GetFloat("MovementSpeed");
    }

    public void Fire()
    {
        animator.SetTrigger("Fire");
    }
}
