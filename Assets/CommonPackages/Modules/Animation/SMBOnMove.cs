namespace Modules.Animation
{
    using UnityEngine;
    using UnityEngine.Events;

    public class SMBOnMove : StateMachineBehaviour
    {
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animatorExt = animator.GetComponent<IStateMachineBehaviourMove>();
            animatorExt.OnAnimStateMove(animator, stateInfo, layerIndex);
        }
    }
}