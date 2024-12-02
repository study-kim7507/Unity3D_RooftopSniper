using UnityEngine;

public class TargetAnimatorController : MonoBehaviour
{
    private enum State
    {
        Idle,
        Walk,
        Run
    }

    private State CurrentState;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Idle()
    {
        CurrentState = State.Idle;

        int randomIndex = UnityEngine.Random.Range(1, 5);
        animator.SetTrigger("Idle" + randomIndex);
    }

    public void Walk()
    {

        // TODO: Walk�� ���� ���� �ִϸ��̼��� ������ Idle�� �����ϰ� ���� �ʿ�
        CurrentState = State.Walk;

        animator.SetTrigger("Walk");
    }

    public void Run()
    {
        CurrentState = State.Run;

        animator.SetTrigger("Run");
    }

}
