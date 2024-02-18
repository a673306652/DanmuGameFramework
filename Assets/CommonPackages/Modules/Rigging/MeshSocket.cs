namespace Modules.Rigging
{
    using UnityEngine;

    public class MeshSocket : MonoBehaviour
    {
        public MeshSockets.SocketId SocketId;

        [SerializeField]
        private Transform _AttachPoint;

        public int AttachedCount
        {
            get
            {
                return _AttachPoint.childCount;
            }
        }

        public Transform First
        {
            get
            {
                return _AttachPoint.GetChild(0);
            }
        }

        public void Attach(Transform objectTransform)
        {
            objectTransform.SetParent(_AttachPoint, false);
        }
    }
}
