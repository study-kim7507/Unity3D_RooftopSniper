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

        // TODO: Walk도 여러 개의 애니메이션이 존재함 Idle과 유사하게 변경 필요
        CurrentState = State.Walk;

        animator.SetTrigger("Walk");
    }

    public void Run()
    {
        CurrentState = State.Run;

        animator.SetTrigger("Run");
    }

}
