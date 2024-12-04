using System.Collections.Generic;
using UnityEngine;

public class PoliceAnimatorController : MonoBehaviour
{
    public enum PoliceAnimState
    {
        Walk,
        Jog,
        Run
    }

    private PoliceAnimState CurrentState;
    private Animator animator;
    private AnimatorControllerParameter[] parameters;
    private List<AnimatorControllerParameter> walkParameters = new List<AnimatorControllerParameter>();
    private List<AnimatorControllerParameter> jogParamters = new List<AnimatorControllerParameter>();
    private List<AnimatorControllerParameter> runParameters = new List<AnimatorControllerParameter>();

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        parameters = animator.parameters;

        foreach (var param in parameters)
        {
            if (param.name.Contains("Walk")) walkParameters.Add(param);
            else if (param.name.Contains("Jog")) jogParamters.Add(param);
            else if (param.name.Contains("Run")) runParameters.Add(param);
        }
    }

    public void Walk()
    {
        CurrentState = PoliceAnimState.Walk;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    public void Jog()
    {
        CurrentState = PoliceAnimState.Jog;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    public void Run()
    {
        CurrentState = PoliceAnimState.Run;

        animator.SetTrigger(RandomSelection(CurrentState));
    }

    private string RandomSelection(PoliceAnimState currentAnimState)
    {
        AnimatorControllerParameter selected = new AnimatorControllerParameter();
        switch (currentAnimState)
        {
            case PoliceAnimState.Walk:
                selected = walkParameters[UnityEngine.Random.Range(0, walkParameters.Count)];
                break;
            case PoliceAnimState.Jog:
                selected = jogParamters[UnityEngine.Random.Range(0, jogParamters.Count)];
                break;
            case PoliceAnimState.Run:
                selected = runParameters[UnityEngine.Random.Range(0, runParameters.Count)];
                break;
            default:
                selected = null;
                break;
        }

        return selected.name;
    }

}
