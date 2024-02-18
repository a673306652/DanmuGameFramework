namespace Modules.Animation
{
    using UnityEngine;
    public interface IStateMachineBehaviour : IStateMachineBehaviourEnter, IStateMachineBehaviourExit, IStateMachineBehaviourMove
    {
    }

    public interface IStateMachineBehaviourEnter
    {
        public void OnAnimStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }

    public interface IStateMachineBehaviourExit
    {
        public void OnAnimStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }

    public interface IStateMachineBehaviourMove
    {
        public void OnAnimStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
    }
}