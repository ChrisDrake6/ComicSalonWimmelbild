using System;
using UnityEngine;

public class HatGuyWavingDone : StateMachineBehaviour
{
    public static event Action CreationFinished;


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CreationFinished.Invoke();
    }
}
