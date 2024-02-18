namespace Modules.Animation
{
    using UnityEngine;
    using UnityEngine.Events;

    public class SimpleSMB : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animatorExt = animator.GetComponent<IStateMachineBehaviour>();
            animatorExt.OnAnimStateEnter(animator, stateInfo, layerIndex);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animatorExt = animator.GetComponent<IStateMachineBehaviour>();
            animatorExt.OnAnimStateExit(animator, stateInfo, layerIndex);
        }

        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var animatorExt = animator.GetComponent<IStateMachineBehaviour>();
            animatorExt.OnAnimStateMove(animator, stateInfo, layerIndex);
        }
    }
}