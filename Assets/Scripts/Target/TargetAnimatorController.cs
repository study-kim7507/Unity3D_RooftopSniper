using UnityEngine;

public class TargetAnimatorController : MonoBehaviour
{
    enum State
    {
        Idle,
        Walk,
        Run
    }

    private State state;
    private Animator animator;

    private void Start()
    {
        state = State.Idle;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Walk:
                break;
            case State.Run:
                break;
        }
    }

}
