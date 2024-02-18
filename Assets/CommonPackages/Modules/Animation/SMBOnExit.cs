namespace Modules.Animation
{
    using UnityEngine;
    using UnityEngine.Events;

    public class SMBOnExit : StateMachineBehaviour
    {
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animatorExt = animator.GetComponent<IStateMachineBehaviourExit>();
            animatorExt.OnAnimStateExit(animator, stateInfo, layerIndex);
        }
    }
}