namespace Modules.Animation
{
    using UnityEngine;
    using UnityEngine.Events;

    public class SMBOnEnter : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animatorExt = animator.GetComponent<IStateMachineBehaviourEnter>();
            animatorExt.OnAnimStateEnter(animator, stateInfo, layerIndex);
        }
    }
}