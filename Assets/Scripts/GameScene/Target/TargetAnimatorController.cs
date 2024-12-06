using System.Collections.Generic;
using UnityEngine;

public class TargetAnimatorController : MonoBehaviour
{
    public enum TargetAnimState
    {
        Idle,
        Walk,
        Jog,
        Run
    }

    private TargetAnimState CurrentState;
    private Animator animator;
    private AnimatorControllerParameter[] parameters;
    private List<AnimatorControllerParameter> idleParameters = new List<AnimatorControllerParameter>();
    private List<AnimatorControllerParameter> walkParameters = new List<AnimatorControllerParameter>();
    private List<AnimatorControllerParameter> jogParameters = new List<AnimatorControllerParameter>();
    private List<AnimatorControllerParameter> runParameters = new List<AnimatorControllerParameter>();

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        parameters = animator.parameters;

        foreach (var param in parameters)
        {
            if (param.name.Contains("Idle")) idleParameters.Add(param);
            else if (param.name.Contains("Walk")) walkParameters.Add(param);
            else if (param.name.Contains("Jog")) jogParameters.Add(param);
            else if (param.name.Contains("Run")) runParameters.Add(param);
        }

        // For Debugging
        //Debug.Log("idle : " + idleParameters.Count);
        //Debug.Log("walk : " + walkParameters.Count);
        //Debug.Log("jog : " + jogParamters.Count);
        //Debug.Log("run : " + runParameters.Count);

    }

    public void Idle()
    {
        CurrentState = TargetAnimState.Idle;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    public void Walk()
    {
        CurrentState = TargetAnimState.Walk;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    public void Jog()
    {
        CurrentState = TargetAnimState.Jog;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    public void Run()
    {
        CurrentState = TargetAnimState.Run;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    private string RandomSelection(TargetAnimState currentAnimState)
    {
        AnimatorControllerParameter selected = new AnimatorControllerParameter();
        int random = 0;
        switch(currentAnimState)
        {
            case TargetAnimState.Idle:
                random = UnityEngine.Random.Range(0, idleParameters.Count);
                selected = idleParameters[random];
                break;
            case TargetAnimState.Walk:
                random = (int)UnityEngine.Random.Range(0, walkParameters.Count);
                selected = walkParameters[random];
                break;
            case TargetAnimState.Jog:
                random = (int)UnityEngine.Random.Range(0, jogParameters.Count);
                selected = jogParameters[random];
                break;
            case TargetAnimState.Run:
                random = (int)UnityEngine.Random.Range(0, runParameters.Count);
                selected = runParameters[random];
                break;
            default:
                selected = null;
                break;
        }

        // Debug.Log(selected.name);

        return selected.name;
    }
}
