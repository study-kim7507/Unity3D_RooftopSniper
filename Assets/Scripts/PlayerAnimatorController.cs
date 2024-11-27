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

    public void ToggleSniperMode()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("scoped@sniper_02"))
        {
            animator.SetTrigger("ZoomIn");
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("scoped@sniper_02"))
        {
            animator.SetTrigger("ZoomOut");
        }
    }

    public void Fire()
    {
        animator.SetTrigger("Fire");
    }
}
