using UnityEngine;

public class PoliceAnimatorController : MonoBehaviour
{
    private enum State
    {
        Walk,
        Run
    }
    private State CurrentState;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Walk()
    {
        CurrentState = State.Walk;

        animator.SetTrigger("Walk");
    }

    public void Run()
    {
        CurrentState = State.Run;

        animator.SetTrigger("Run");
    }

}
